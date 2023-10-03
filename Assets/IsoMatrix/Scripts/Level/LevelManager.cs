using System.Collections.Generic;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Rail;
using IsoMatrix.Scripts.TileMap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace IsoMatrix.Scripts.Level
{
    public class LevelManager : MonoBehaviour, IEventListener<LevelEvent>
    {
        [SerializeField]
        private GridChecker gridChecker;
        public List<GameObject>  ListTrain { get; protected set; }
        public bool CanReset
        {
            get => canReset;
            set => canReset = value;
        }

        private GameObject pathContainerGO;
        private Transform exitPosition;
        public UnityEvent GamePause;
        public UnityEvent GameWon;
        public UnityEvent GameLosed;



        private bool canReset = true;
        private bool isWon;
        public bool IsWon => isWon;
        private float _timeStart;
        private List<RailCreator> listRailCreator = new List<RailCreator>();

        private void OnEnable()
        {
            EventManager.Subscribe(this);
        }

        // public void Start()
        // {
        //     RespawnPlayer();
        // }

        public void AddListTrain(List<GameObject> listItem)
        {
            ListTrain = new List<GameObject> (listItem);
        }

        public void AddMaxRail(int maxRail)
        {
            gridChecker.AddMaxRail(maxRail);
        }

        public void PauseGame()
        {
            ReloadData();
            GamePause?.Invoke();
        }

        public void NextLevel()
        {
            ReloadData();
            // LevelEvent.Trigger(LevelEventType.NextLevel, PlayerData.Instance.CurrentLevelKey += 1);
            var i = 0;
            LevelEvent.Trigger(LevelEventType.NextLevel, i += 1);
        }

        public void ReloadLevel()
        {
            if (canReset)
            {
                canReset = false;
                // IsLose = false;
                ReloadData();
                // LevelEvent.Trigger(LevelEventType.ReloadCurLevel,PlayerData.Instance.CurrentLevelKey);
            }
        }

        public void OnLose()
        {
            ReloadData();
            GameLosed.Invoke();
        }

        public void ReloadData()
        {
        }

        public void GetPathContainer(GameObject pathContainer)
        {
            pathContainerGO = pathContainer;
            if (pathContainerGO)
            {
                gridChecker.prefabContainer = pathContainerGO;
                gridChecker.GetFixPath();
            }
        }

        public void ChangeTypeAction(bool type)
        {
            gridChecker.IsDestroy = type;
        }

        public void OnEventTriggered(LevelEvent e)
        {

        }

        public void TrainAction()
        {
            foreach (var railCreator in listRailCreator)
            {
                railCreator.DragStopped();
            }
        }

        public void AddListRailCreator(List<RailCreator> levelControllerListTrain)
        {
            listRailCreator = levelControllerListTrain;
        }
    }
}