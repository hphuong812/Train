using System.Collections;
using System.Collections.Generic;
using IsoMatrix.Scripts.Level;
using UnityEngine;

namespace CatSimulate.Level
{
    public class LevelEventTrigger : MonoBehaviour
    {
        public void TriggerLoadCurrentLevel()
        {
            // var currentLevel = PlayerData.Instance.CurrentLevelKey;
            var currentLevel = 0;
            LevelEvent.Trigger(LevelEventType.Loaded, currentLevel);
        }
    }
}