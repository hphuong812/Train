using System.Collections.Generic;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Rail;
using IsoMatrix.Scripts.TileMap;
using IsoMatrix.Scripts.Train;
using IsoMatrix.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace IsoMatrix.Scripts.Level
{
    public class LevelManager : MonoBehaviour, IEventListener<LevelEvent>
    {
        [SerializeField]
        private GridChecker gridChecker;

        [SerializeField] private Camera isoMatrixCamera;
        public List<GameObject>  ListTrain { get; protected set; }
        public bool CanReset
        {
            get => canReset;
            set => canReset = value;
        }
        [FormerlySerializedAs("SpawnCardMissionManager")] public SpawnCardOrderManager spawnCardOrderManager;

        private GameObject pathContainerGO;
        private Transform exitPosition;
        public UnityEvent GamePause;
        public UnityEvent GameWon;
        public UnityEvent GameLosed;

        private LevelItemData levelItemData;
        private CardOrderManager _cardOrderManager;

        private bool canReset = true;
        private bool isWon;
        public bool IsWon => isWon;
        private float _timeStart;
        private List<RailCreator> listRailCreator = new List<RailCreator>();
        private List<CardOrderManager> listCardOrder= new List<CardOrderManager>();

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

        public void AddItemData(LevelItemData levelItemData)
        {
            this.levelItemData = levelItemData;
            gridChecker.AddMaxRail(this.levelItemData.MaxRail);
            SpawnCardOrder();
        }

        private void SpawnCardOrder()
        {
            RemoveCard();
            listCardOrder.Clear();
            for (int i = 0; i < levelItemData.TrainOrder.Count; i++)
            {Debug.Log("adads");
                _cardOrderManager = spawnCardOrderManager.SpawnCard();
                _cardOrderManager.SetUpMission(levelItemData.TrainOrder[i], i+1);
                listCardOrder.Add(_cardOrderManager);
            }
        }

        public void RemoveCard()
        {
            Debug.Log("calll");
            foreach (var card in listCardOrder)
            {
                Destroy(card.gameObject);
            }
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
            // RemoveCard();
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

            if (e.type == LevelEventType.NextLevel)
            {

            }
        }

        public void TrainAction()
        {
            TrainActionEvent.Trigger(TrainActionEventType.Run);
            // foreach (var railCreator in listRailCreator)
            // {
            //     railCreator.DragStopped();
            // }
        }

        public void TrainRevert()
        {
            TrainActionEvent.Trigger(TrainActionEventType.Reset);
            // foreach (var railCreator in listRailCreator)
            // {
            //     railCreator.RespawnTrain();
            // }
        }

        public void AddListRailCreator(List<RailCreator> levelControllerListTrain)
        {
            listRailCreator = levelControllerListTrain;
        }

        public void OnLevelDestroyed()
        {
            ScreenEvent.Trigger(ScreenEventType.ScreenOut);
        }

        public void UpdateCameraSize(float levelControllerCameraSize, Vector3 levelControllerCameraPos)
        {
            isoMatrixCamera.orthographicSize = levelControllerCameraSize;
            isoMatrixCamera.transform.position = levelControllerCameraPos;
        }
    }
}