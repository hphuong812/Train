using System.Collections.Generic;
using CatSimulate.Level;
using IsoMatrix.Scripts.Rail;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GridChecker : MonoBehaviour
{
        public GameObject prefabContainer;

        public LayerMask TileLayerMask;

        [SerializeField]
        private float timeForNextRay = 0.05f;
        [SerializeField]
        private float speed = 10f;
        private int foodDistance = 10;

        private float timer = 0;

        private bool hasFirst;
        private bool isCreate;
        private TileManager firstTileManager;
        private TileManager beforeFirstTileManager = null;
        private PathFinder _pathFinder;
        private List<TileManager> path = new List<TileManager>();
        private List<RailManager> listRail = new List<RailManager>();
        private List<Vector2> listPos = new List<Vector2>();
        private RailGenerate _railGenerate;
        void Start()
        {
            _pathFinder = new PathFinder();
            _railGenerate = new RailGenerate();
        }

        public void GetFixPath()
        {
            foreach (Transform child in prefabContainer.transform)
            {
                RailManager railManager = child.GetComponent<RailManager>();
                if (railManager)
                {
                    listRail.Add(railManager);
                }
            }
        }


        public void DragStarted(InputAction.CallbackContext ctx)
        {

            if (ctx.phase == InputActionPhase.Performed)
            {
                var touchPosition = ctx.ReadValue<Vector2>();
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);

                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, maxDistance: 200f))
                {
                    if (LayerMarkChecker.LayerInLayerMask(hit.transform.gameObject.layer, TileLayerMask))
                    {
                        TileManager tileManager = hit.transform.gameObject.GetComponent<TileManager>();
                        if (!hasFirst)
                        {
                            var index = -1;
                            firstTileManager = tileManager;
                            firstTileManager.previous = tileManager.previous;
                            Vector3 firstRailPos = new Vector3(firstTileManager.GridLocation.x, 1,
                                firstTileManager.GridLocation.y);
                            var cout = 0;
                            RailType typeTapRail = RailType.none;
                            RailType typeChange = RailType.none;
                            for (int i = 0; i < listRail.Count; i++)
                            {
                                if (listRail[i].gameObject.transform.localPosition == new Vector3(tileManager.GridLocation.x,1,tileManager.GridLocation.y))
                                {
                                    typeTapRail = listRail[i].railType;
                                    index = i;
                                    cout++;
                                    break;
                                }
                            }

                            if (cout>0 && !listRail[index].isFix)
                            {
                                typeChange = CheckTouchGroupRail(typeTapRail);
                                if (typeChange != RailType.none && index != -1)
                                {
                                    string typeChangeString = "rail_" + typeChange;
                                    Destroy(listRail[index].gameObject);
                                    listRail.RemoveAt(index);
                                    AddNewRail(typeChangeString, firstRailPos);
                                }
                            }else if (cout == 0)
                            {
                                string type = "rail_"+GetRailNameNear(tileManager);

                                AddNewRail(type, firstRailPos);

                            }
                            hasFirst = true;
                        }
                        else
                        {
                            if (path.Count ==0)
                            {
                                path = _pathFinder.FindPath(firstTileManager, tileManager);
                            }
                        }
                    }
                    if (path.Count>0)
                    {
                        ChangePathType();
                    }
                }


            }
            timer += Time.deltaTime;
        }

        public void AddNewRail(string type, Vector3 pos)
        {
            Addressables.LoadAssetAsync<GameObject>(type).Completed += handle =>
            {
                var Check2 = Instantiate(handle.Result, prefabContainer.transform);
                if (Check2)
                {
                    Check2.transform.localPosition = pos;
                    RailManager railManager = Check2.GetComponent<RailManager>();
                    listRail.Add(railManager);

                }
            };
        }

        public RailType CheckTouchGroupRail(RailType typeRail)
        {
            if (typeRail != RailType.none)
            {
                switch (typeRail)
                {
                    case RailType.right_bottom_left:
                        return RailType.right_bottom_right;
                    case RailType.right_bottom_right:
                        return RailType.right_bottom_left ;
                    case RailType.right_top_left:
                        return RailType.right_top_right;
                    case RailType.right_top_right:
                        return RailType.right_top_left;
                    case RailType.top_bottom_left:
                        return RailType.top_top_left;
                    case RailType.top_bottom_right:
                        return RailType.top_top_right;
                    case RailType.top_top_left:
                        return RailType.top_bottom_left;
                    case RailType.top_top_right:
                        return RailType.top_bottom_right;
                }
            }

            return typeRail;
        }

        public string GetRailNameNear(TileManager tileCheck)
        {
            var map = MapManager.Instance.map;
            foreach (var item in map)
            {
                if (Vector2.Distance(item.Key, tileCheck.GridLocation) == 1)
                {
                    foreach (var rail in listRail)
                    {
                        if (rail.gameObject.transform.localPosition == new Vector3(item.Key.x, 1, item.Key.y) && rail.railOption == RailOption.edge)
                        {
                            return rail.railType.ToString();
                        }
                    }
                }
            }
            return "top";
        }

        private void ChangePathType()
        {
            for (int i = 0; i < path.Count; i++)
            {

                var previousTile = i > 0 ? path[i - 1] : firstTileManager;
                var futureTile = i < path.Count - 1 ? path[i + 1] : null;
                var railDir = _railGenerate.RailDirection(previousTile, path[i], futureTile);
                var raiOption = _railGenerate.GetOption(railDir);
                RailOption? railOptionTileCheck = null;
                RailType railCurrentType = RailType.none;
                var cout = 0;
                var pos = new Vector3(path[i].GridLocation.x, 1, path[i].GridLocation.y);
                var index = -1;
                if (listRail.Count>0)
                {
                    for (int j = 0; j < listRail.Count; j++)
                    {
                        if (listRail[j].gameObject.transform.localPosition == new Vector3(path[i].GridLocation.x, 1, path[i].GridLocation.y))
                        {
                            railOptionTileCheck = listRail[j].railOption;
                            railCurrentType = listRail[j].railType;
                            index = j;
                            // Destroy(listRail[index].gameObject);
                            // listRail.RemoveAt(index);
                            break;
                        }
                    }
                }

                if (railDir!= RailType.none)
                {
                    var nameRail = "rail_"+railDir.ToString();
                    if (railOptionTileCheck != null && index != -1 && !listRail[index].isFix)
                    {
                        if (raiOption == RailOption.angle && railOptionTileCheck ==RailOption.edge )
                        {
                            nameRail = _railGenerate.CheckAroundRail(listRail, pos, railDir);
                        }
                        Destroy(listRail[index].gameObject);
                        listRail.RemoveAt(index);
                        AddNewRail(nameRail, pos);
                    }
                    else if(index == -1)
                    {
                        AddNewRail(nameRail, pos);
                    }

                }
            }
            CheckStart();
            if (path.Count>1)
            {
                beforeFirstTileManager = path[path.Count-2];
            }
            else
            {
                beforeFirstTileManager = firstTileManager;
            }
            firstTileManager = path[path.Count-1];
            path.Clear();
        }

        private void CheckStart()
        {
            if (listRail.Count>0 && firstTileManager)
            {
                var futureTileBeforeStart = path.Count>0 ? path[0] : null;
                var beforeTile = beforeFirstTileManager ? beforeFirstTileManager : firstTileManager.previous;
                var railDirbefore = _railGenerate.RailDirection(beforeTile, firstTileManager, futureTileBeforeStart);
                var raiOption = _railGenerate.GetOption(railDirbefore);
                RailOption? railOptionTileCheck = null;
                RailType railCurrentType = RailType.none;
                var posChange = new Vector3(firstTileManager.GridLocation.x, 1,firstTileManager.GridLocation.y );
                if (railDirbefore != RailType.none)
                {
                    var index = -1;
                    for (int i = 0; i < listRail.Count; i++)
                    {
                        if (listRail[i].gameObject.transform.localPosition == new Vector3(firstTileManager.GridLocation.x, 1, firstTileManager.GridLocation.y))
                        {
                            railOptionTileCheck = listRail[i].railOption;
                            railCurrentType = listRail[i].railType;
                            index = i;
                            break;
                        }
                    }
                    var nameRail = "rail_"+railDirbefore.ToString();
                    if (railOptionTileCheck != null && index != -1 && !listRail[index].isFix)
                    {
                        if (raiOption == RailOption.angle && railOptionTileCheck ==RailOption.edge )
                        {
                            nameRail = _railGenerate.CheckAroundRail(listRail, posChange, railDirbefore);
                        }
                        Destroy(listRail[index].gameObject);
                        listRail.RemoveAt(index);
                        AddNewRail(nameRail, posChange);
                    }
                    else if(index == -1)
                    {
                        AddNewRail(nameRail, posChange);
                    }

                }
            }
        }

        private void CheckEnd()
        {
            // if (listRail.Count>0 && beforeFirstTileManager)
            // {
            //     var reviousTile = beforeFirstTileManager.previous? beforeFirstTileManager.previous : null;
            //     var railDirbefore = _railGenerate.RailDirection(reviousTile, beforeFirstTileManager, firstTileManager);
            //     var posChange = new Vector3(beforeFirstTileManager.GridLocation.x, 1,beforeFirstTileManager.GridLocation.y );
            //     if (railDirbefore != RailType.none)
            //     {
            //         var index = -1;
            //         for (int i = 0; i < listRail.Count; i++)
            //         {
            //             if (listRail[i].transform.localPosition == new Vector3(beforeFirstTileManager.GridLocation.x, 1, beforeFirstTileManager.GridLocation.y))
            //             {
            //                 index = i;
            //                 Destroy(listRail[i]);
            //                 listRail.RemoveAt(i);
            //                 break;
            //             }
            //         }
            //         Addressables.LoadAssetAsync<GameObject>("rail_" + railDirbefore.ToString()).Completed += handle =>
            //         {
            //             var Check2 = Instantiate(handle.Result, prefabContainert.transform);
            //             if (Check2)
            //             {
            //                 Check2.transform.localPosition =posChange;
            //
            //                     listRail.Add(Check2);
            //
            //             }
            //         };
            //     }
            // }
        }

        public void DragStopped(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Canceled)
            {
                hasFirst = false;
                beforeFirstTileManager = null;
                firstTileManager = null;
                Debug.Log("cancel");
            }
        }
}
