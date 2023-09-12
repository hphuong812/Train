using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class GridChecker : MonoBehaviour
{
        [SerializeField]
        private GameObject prefabtest;
        [SerializeField]
        private GameObject prefabContainert;

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
                            var cout = 0;
                            for (int i = 0; i < listRail.Count; i++)
                            {
                                if (listRail[i].gameObject.transform.localPosition == new Vector3(tileManager.GridLocation.x,1,tileManager.GridLocation.y))
                                {
                                    cout++;
                                    break;
                                }
                            }

                            if (cout == 0)
                            {
                                string type = GetRailNameNear(tileManager);
                                Addressables.LoadAssetAsync<GameObject>("rail_"+type).Completed += handle =>
                                {
                                    var Check2 = Instantiate(handle.Result, prefabContainert.transform);
                                    if (Check2)
                                    {
                                        Check2.transform.localPosition =new Vector3(firstTileManager.GridLocation.x,1, firstTileManager.GridLocation.y);
                                        RailManager railManager = Check2.GetComponent<RailManager>();
                                            listRail.Add(railManager);

                                    }
                                };
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
                        MoveCharacter();
                    }
                }


            }
            timer += Time.deltaTime;
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
            return "right";
        }

        private void Update()
        {

        }

        private void MoveCharacter()
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
                    if (railOptionTileCheck != null && index != -1)
                    {
                        if (raiOption == RailOption.angle && railOptionTileCheck ==RailOption.edge )
                        {
                            nameRail = _railGenerate.CheckAroundRail(listRail, pos, railDir);
                        }
                        Destroy(listRail[index].gameObject);
                        listRail.RemoveAt(index);
                    }
                    Addressables.LoadAssetAsync<GameObject>(nameRail).Completed += handle =>
                    {
                        var Check = Instantiate(handle.Result, prefabContainert.transform);
                        if (Check)
                        {
                            RailManager railManager = Check.GetComponent<RailManager>();
                            listRail.Add(railManager);
                            Check.transform.localPosition =pos;
                        }
                    };
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
                    if (railOptionTileCheck != null && index != -1)
                    {
                        if (raiOption == RailOption.angle && railOptionTileCheck ==RailOption.edge )
                        {
                            nameRail = _railGenerate.CheckAroundRail(listRail, posChange, railDirbefore);
                        }
                        Destroy(listRail[index].gameObject);
                        listRail.RemoveAt(index);
                    }
                    Addressables.LoadAssetAsync<GameObject>(nameRail).Completed += handle =>
                    {
                        var Check2 = Instantiate(handle.Result, prefabContainert.transform);
                        if (Check2)
                        {
                            Check2.transform.localPosition =posChange;
                            RailManager railManager = Check2.GetComponent<RailManager>();
                            listRail.Add(railManager);

                        }
                    };
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
                foreach (var rail in listRail)
                {
                    // Debug.Log(rail.name);
                }
            }
            //     if (listRail.Count>0)
            //     {
            //         var futureTileBeforeStart = path.Count>0 ? path[0] : null;
            //         var railDirbefore = _railGenerate.RailDirection(beforeFirstTileManager, firstTileManager, futureTileBeforeStart);
            //         var posChange = new Vector3(firstTileManager.GridLocation.x, 1,firstTileManager.GridLocation.y );
            //         if (railDirbefore != RailType.none)
            //         {
            //             foreach (var rail in listRail)
            //             {
            //                 if (rail)
            //                 {
            //                     if (rail.transform.localPosition == new Vector3(firstTileManager.GridLocation.x, 1, firstTileManager.GridLocation.y))
            //                     {
            //                         GameObject railD = rail;
            //                         Destroy(railD);
            //                         listRail.Remove(rail);
            //                         return;
            //                     }
            //                 }
            //             }
            //             Addressables.LoadAssetAsync<GameObject>("rail_" + railDirbefore.ToString()).Completed += handle =>
            //             {
            //                 var Check2 = Instantiate(handle.Result, prefabContainert.transform);
            //                 if (Check2)
            //                 {
            //                     Check2.transform.localPosition = posChange;
            //                     listRail.Add(Check2);
            //                 }
            //             };
            //         }
            //     }
            // }
        }
}
