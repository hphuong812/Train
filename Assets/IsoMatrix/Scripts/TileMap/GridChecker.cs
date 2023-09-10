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
        private List<GameObject> listRail = new List<GameObject>();
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
                            firstTileManager = tileManager;
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
                }
                if (path.Count>0)
                {
                    MoveCharacter();
                }

            }
            timer += Time.deltaTime;
        }

        private void Update()
        {

        }

        private void MoveCharacter()
        {
            var map = MapManager.Instance.map;
            if (listRail.Count>0)
            {
                var futureTileBeforeStart = path.Count>0 ? path[0] : null;
                var railDirbefore = _railGenerate.RailDirection(beforeFirstTileManager, firstTileManager, futureTileBeforeStart);
                var posChange = new Vector3(firstTileManager.GridLocation.x, 1,firstTileManager.GridLocation.y );
                if (railDirbefore != RailType.none)
                {
                    foreach (var rail in listRail)
                    {
                        if (rail)
                        {
                            if (rail.transform.localPosition == new Vector3(firstTileManager.GridLocation.x, 1, firstTileManager.GridLocation.y))
                            {
                                GameObject railD = rail;
                                Destroy(railD);
                                listRail.Remove(rail);
                                break;
                            }
                        }
                    }
                    Addressables.LoadAssetAsync<GameObject>("rail_" + railDirbefore.ToString()).Completed += handle =>
                    {
                        var Check2 = Instantiate(handle.Result, prefabContainert.transform);
                        if (Check2)
                        {
                            Check2.transform.localPosition =posChange;
                            listRail.Add(Check2);
                        }
                    };
                }
            }
            for (int i = 0; i < path.Count; i++)
            {
                var previousTile = i > 0 ? path[i - 1] : firstTileManager;
                var futureTile = i < path.Count - 1 ? path[i + 1] : null;
                var railDir = _railGenerate.RailDirection(previousTile, path[i], futureTile);
                var cout = 0;
                var pos = new Vector3(path[i].GridLocation.x, 1, path[i].GridLocation.y);
                if (listRail.Count>0)
                {
                    foreach (var rail in listRail)
                    {
                        if (rail)
                        {
                            if (rail.transform.localPosition == new Vector3(path[i].GridLocation.x, 1, path[i].GridLocation.y))
                            {
                                GameObject railD = rail;
                                Destroy(railD);
                                listRail.Remove(rail);
                                break;
                            }
                        }
                    }
                }

                if (cout ==0)
                {
                    Addressables.LoadAssetAsync<GameObject>("rail_" + railDir.ToString()).Completed += handle =>
                    {
                        var Check = Instantiate(handle.Result, prefabContainert.transform);
                        if (Check)
                        {
                            listRail.Add(Check);
                            Check.transform.localPosition =pos;
                        }
                    };
                }
            }

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

        public void DragStopped(InputAction.CallbackContext ctx)
        {
            // if (ctx.phase == InputActionPhase.Canceled)
            // {
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
