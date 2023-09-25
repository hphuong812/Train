using System;
using System.Collections.Generic;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Rail;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IsoMatrix.Scripts.Level
{
    public class LevelController : MonoBehaviour, IEventListener<LevelEvent>
    {

        public List<GameObject> listTrain;
        public List<GameObject> listDestroyItem;
        public List<RailCreator> listRail;
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
            foreach (var getItem in listTrain)
            {
                if (getItem)
                {
                    getItem.SetActive(state);
                }
            }
            foreach (var destroyItem in listDestroyItem)
            {
                if (destroyItem)
                {
                    destroyItem.SetActive(state);
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