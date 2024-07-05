using System.Collections.Generic;
using System.Linq;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Level;
using IsoMatrix.Scripts.Rail;
using IsoMatrix.Scripts.Train;
using IsoMatrix.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace IsoMatrix.Scripts.TileMap
{
    public class GridChecker : MonoBehaviour,IEventListener<LevelEvent>
    {
        [SerializeField] private UIController _uiController;
        private int maxLevelRail;
        public int MaxLevelRail
        {
            get => maxLevelRail;
            set => maxLevelRail = value;
        }

        public GameObject prefabContainer;
        public LayerMask TileLayerMask;
        public LayerMask TrainLayerMask;

        private bool hasFirst;
        private bool isCreate;
        private TileManager firstTileManager;
        private TileManager beforeFirstTileManager = null;
        private PathFinder _pathFinder;
        private List<TileManager> path = new List<TileManager>();
        private List<RailManager> listRail = new List<RailManager>();
        private RailGenerate _railGenerate;
        private TrainManager trainCheck;
        private int numRail = 0;
        private bool isDestroy;
        public bool IsDestroy
        {
            get => isDestroy;
            set => isDestroy = value;
        }

        private bool isWon;
        private Dictionary<Vector2, TileManager> map = new Dictionary<Vector2, TileManager>();

        void Start()
        {
            EventManager.Subscribe(this);
            _pathFinder = new PathFinder();
            _railGenerate = new RailGenerate();
        }

        public void AddMaxRail(int maxRail)
        {
            maxLevelRail = maxRail;
            _uiController.UpdateCountRail(maxLevelRail);
            _uiController.UpdateLevel();
        }

        public void GetFixPath()
        {
            listRail.Clear();
            foreach (Transform child in prefabContainer.transform)
            {
                RailManager railManager = child.GetComponent<RailManager>();
                if (railManager)
                {
                    listRail.Add(railManager);
                }
            }

            isWon = false;
        }


        public void DragStarted(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                numRail = listRail.FindAll(rail => rail.isFix == false).Count;
                _uiController.UpdateCountRail(maxLevelRail - numRail);
                var touchPosition = ctx.ReadValue<Vector2>();
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maxDistance: 200f))
                {
                    if (LayerMarkChecker.LayerInLayerMask(hit.transform.gameObject.layer, TrainLayerMask))
                    {
                        trainCheck = hit.transform.gameObject.GetComponent<TrainManager>();
                    }
                }
                if(Physics.Raycast(ray, out hit, maxDistance: 200f, TileLayerMask))
                {
                    HandleTileInteraction(hit);
                }
            }
        }

        public void HandleTileInteraction(RaycastHit hit)
        {
            if (LayerMarkChecker.LayerInLayerMask(hit.transform.gameObject.layer, TileLayerMask))
            {
                TileManager tileManager = hit.transform.gameObject.GetComponent<TileManager>();

                if (!hasFirst && !tileManager.isBlock)
                {
                    firstTileManager = tileManager;
                    firstTileManager.previous = tileManager.previous;
                    hasFirst = true;
                }
                else
                {
                    if (path.Count ==0)
                    {
                        path = _pathFinder.FindPath(firstTileManager, tileManager);
                    }
                }
                map = MapManager.Instance.map;
                tileManager.ChangeSprite(TileSpriteName.tex_tile_select);
                
                foreach (KeyValuePair<Vector2, TileManager> tileManagerInMap in map)
                {
                    if (tileManagerInMap.Key != tileManager.GridLocation)
                    {
                        tileManagerInMap.Value.OnUnSelect();
                    }
                }
                tileManager.OnSelect();
            }
            if (path.Count>0)
            {
                ChangePathType();
            }
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

        public void AddTrainToRailGroup(RailType type, Vector3 pos)
        {
            GetFixPath();
            foreach (var rail in listRail)
            {
                if(rail.gameObject.transform.localPosition == pos)
                {
                    RailGroupManager railGroupManager = rail.GetComponent<RailGroupManager>();
                    if (railGroupManager)
                    {
                        if (trainCheck)
                        {
                            railGroupManager.AddTrainToRail(trainCheck,type);
                        }
                    }
                }
            }
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
                    case RailType.top:
                        return RailType.right;
                    case RailType.right:
                        return RailType.top;
                    case RailType.bottom_right:
                        return RailType.bottom_left;
                    case RailType.bottom_left:
                        return RailType.bottom_right;
                    case RailType.top_right:
                        return RailType.top_left;
                    case RailType.top_left:
                        return RailType.top_right;
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
                var index = -1;
                if (listRail.Count>0)
                {
                    for (int j = 0; j < listRail.Count; j++)
                    {
                        if (listRail[j].gameObject.transform.localPosition == new Vector3(path[i].GridLocation.x, 1, path[i].GridLocation.y))
                        {
                            index = j;
                            listRail.RemoveAt(index);
                            break;
                        }
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

        private void LateUpdate()
        {
            if (!isWon)
            {
                CheckSameRail();
            }
        }

        public void CheckSameRail()
        {
            restart:
            foreach (var item in listRail) {
                if (listRail.Count(i => i.gameObject.transform.localPosition == item.gameObject.transform.localPosition) > 1)
                {
                    var railDr = listRail.FindLast(i =>
                        i.gameObject.transform.localPosition == item.gameObject.transform.localPosition);
                    listRail.Remove(railDr);
                    goto restart;
                }
            }
        }

        private void CheckStart()
        {
            if (listRail.Count>0 && firstTileManager)
            {
                var futureTileBeforeStart = path.Count>0 ? path[0] : null;
                var beforeTile = beforeFirstTileManager ? beforeFirstTileManager : firstTileManager.previous;
                var railDirbefore = _railGenerate.RailDirection(beforeTile, firstTileManager, futureTileBeforeStart);
                var posChange = new Vector3(firstTileManager.GridLocation.x, 1,firstTileManager.GridLocation.y );
                if (railDirbefore != RailType.none)
                {
                    AddTrainToRailGroup(railDirbefore, posChange);
                }
            }
        }

        public void DragStopped(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Canceled)
            {
                trainCheck = null;
                hasFirst = false;
                beforeFirstTileManager = null;
                firstTileManager = null;
                map = MapManager.Instance.map;
                foreach (KeyValuePair<Vector2, TileManager> tileManagerInMap in map)
                {
                    tileManagerInMap.Value.OnUnSelect();
                }
                TrainActionEvent.Trigger(TrainActionEventType.Update);
            }
        }

        public void ResetData()
        {
            hasFirst = false;
            beforeFirstTileManager = null;
            firstTileManager = null;
        }

        public void OnEventTriggered(LevelEvent e)
        {
            switch (e.type)
            {
                case LevelEventType.Failed :
                    ResetData();
                    break;
                case LevelEventType.NextLevel:
                    ResetData();
                    isWon = true;
                    break;
            }
        }
    }
}
