using System;
using System.Collections.Generic;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Rail;
using IsoMatrix.Scripts.Train;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IsoMatrix.Scripts.Level
{
    public class LevelController : MonoBehaviour, IEventListener<LevelEvent>
    {
        public float CammeraSize = 7.8f;
        public Vector3 CameraPos = new Vector3(-0.8f, 7.8f, -2.1f);
        public LocomotiveManager locomotiveManager;
        public List<GameObject> listItem;
        public List<TrainController> listTrain;
        public GameObject PathContainer;
        public event Action DataLoaded;
        public event Action LevelDestroyed;
        public UnityEvent CurrentLevelCompleted;

        private List<GameObject> listEnemyCurrentPos = new List<GameObject>();
        public void Start()
        {
            EventManager.Subscribe(this);
        }
        public void LoadMapCompleted()
        {
            DataLoaded?.Invoke();
        }

        public void DestroyLevel()
        {
            LevelDestroyed?.Invoke();
            Destroy(gameObject);
        }

        public void ChangeStateItem(bool state)
        {
            foreach (var getItem in listItem)
            {
                if (getItem)
                {
                    getItem.SetActive(state);
                }
            }
        }

        public void OnEventTriggered(LevelEvent e)
        {
            switch (e.type)
            {
                case LevelEventType.Failed :
                    CurrentLevelCompleted?.Invoke();
                    break;
                case LevelEventType.NextLevel:
                    CurrentLevelCompleted?.Invoke();
                    break;
            }
        }
    }
}