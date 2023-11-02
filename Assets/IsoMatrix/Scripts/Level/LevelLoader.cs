using System;
using System.Collections.Generic;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace IsoMatrix.Scripts.Level
{
    public class LevelLoader : MonoBehaviourEventListener<LevelEvent>
    {
        [SerializeField]
        protected List<GameObject> levelPrefabList;
        [SerializeField]
        protected GameObject levelTutorial;

        [SerializeField]
        protected LevelManager levelManager;
        [SerializeField]
        private int curLevelId;
        public int CurLevelId
        {
            get => curLevelId;
            set => curLevelId = value;
        }
        public UnityEvent Loaded;
        private GameObject levelGO;
        private LevelController levelController;
        private LevelItemData levelItemData;
        private void OnEnable()
        {
            EventManager.Subscribe(this);
        }

        private void Start()
        {
            //hardcode load level data
            GameConfigLoader.Instance.Load();
            LevelEvent.Trigger(LevelEventType.Loaded, CurLevelId);
        }

        public virtual void LoadData(string typeLoad)
        {

            int levelId;
            // if (PlayerData.Instance.CurrentLevelKey>=curLevelId)
            // {
            //     levelId = PlayerData.Instance.CurrentLevelKey;
            // }
            // else
            // {
                levelId =  GameConfig.Instance.CurrentLevel;
            // }
            levelItemData = GameConfig.Instance.LevelItemList.Find((item) => item.Id.Equals(levelId));
            // var levelIndex = GameConfig.Instance.LevelItemList.FindIndex((item) => item.Id.Equals(levelId));
            LoadLevel(levelItemData.LevelKey, typeLoad);

        }

        public virtual void LoadLevel(string index, string typeLoad)
        {
            GameObject levelPrefab ;
            var levelAssets = Addressables.LoadAssetAsync<GameObject>(index);
            levelPrefab = levelAssets.WaitForCompletion();


            if (!levelPrefab)
            {
                Debug.LogError($"Missing Level Prefab: {index}");
                return;
            }

            if (levelGO)
            {
                if (levelController)
                {
                    levelController.CurrentLevelCompleted.Invoke();
                    levelController.DataLoaded -= LoadDataComplete;
                    levelController.DataLoaded -= ReloadLevel;
                }
            }

            levelGO = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
            levelController = levelGO.GetComponent<LevelController>();
            levelController.locomotiveManager.ListTrainConrect = levelItemData.TrainOrder;
            levelManager.AddListTrain(levelController.listTrain);
            levelManager.AddListRailCreator(levelController.listRail);
            levelManager.GetPathContainer(levelController.PathContainer);
            levelManager.AddMaxRail(levelItemData.MaxRail);
            levelManager.UpdateCameraSize(levelController.CammeraSize, levelController.CameraPos);
            // levelManager.GamePause.AddListener(()=>
            // {
            //     levelController.CurrentLevelCompleted.Invoke();
            // });
            levelController.LevelDestroyed += () =>
            {
                levelManager.CanReset = true;
                levelManager.OnLevelDestroyed();
            };
            levelController.DataLoaded += LoadDataComplete;
            // if(typeLoad == GameConstant.LEVEL_WIN|| typeLoad == GameConstant.LEVEL_LOAD)
            // {
                levelController.DataLoaded += ReloadLevel;
                // }
        }
        public void LoadDataComplete()
        {
            Loaded?.Invoke();
        }
        public void ReloadLevel()
        {
        }
        public void OnReloadCurLevel()
        {
            levelManager.AddListTrain(levelController.listTrain);
            levelManager.CanReset = true;
            ReloadLevel();
            // if (levelManager.Players)
            // {
            //     levelManager.GamePlay();
            // }
        }

        public override void OnEventTriggered(LevelEvent e)
        {
            switch (e.type)
            {
                case LevelEventType.Loaded :
                    this.CurLevelId = e.currentLevel;
                    LoadData(GameConstant.LEVEL_LOAD);
                    break;
                case LevelEventType.Failed :
                    this.CurLevelId = e.currentLevel;
                    levelManager.OnLose();
                    LoadData(GameConstant.LEVEL_WIN);
                    break;
                case LevelEventType.NextLevel:
                    this.CurLevelId = e.currentLevel;
                    LoadData(GameConstant.LEVEL_WIN);
                    break;
                case LevelEventType.ReloadCurLevel:
                    this.CurLevelId = e.currentLevel;
                    LoadData(GameConstant.LEVEL_RELOAD);
                    break;
            }
        }
    }
}