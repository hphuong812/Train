using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IsoMatrix.Scripts.Rail
{
    public enum RailOption
    {
        edge,
        angle
    }
    public class RailManager : MonoBehaviour
    {
        public RailType railType;
        public RailOption railOption;
        public bool isFix;
        public UnityEvent Replaced;
        public UnityEvent BeforeDestroyed;

        private void OnDestroy()
        {
            Replaced?.Invoke();
        }

        public void BeforeDestroy()
        {
            BeforeDestroyed?.Invoke();
        }

        public void DestroyRail()
        {
            Destroy(gameObject);
        }
    }
}