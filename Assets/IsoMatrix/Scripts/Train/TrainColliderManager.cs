using System;
using ADN.Meta.Core;
using UnityEngine;

namespace IsoMatrix.Scripts.Train
{
    public class TrainColliderManager : MonoBehaviour,  IEventListener<TrainActionEvent>
    {
        [SerializeField]
        private LayerMask trainLayerMask;

        [SerializeField] private TrainController _trainController;
        [SerializeField] private TrainManager _trainManager;
        [SerializeField] private LayerMask locomotiveLayerMask;

        private bool locomotiveMatched;
        public bool LocomotiveMatched => locomotiveMatched;

        private void Start()
        {
            EventManager.Subscribe(this);
        }
        private void OnDisable()
        {
            EventManager.Unsubscribe(this);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, trainLayerMask))
            {
                if (_trainController)
                {
                    _trainController.canRun = false;
                }

                TrainColliderManager colliderTrainColliderManager = other.gameObject.GetComponent<TrainColliderManager>();
                if (colliderTrainColliderManager)
                {
                    if (colliderTrainColliderManager.LocomotiveMatched)
                    {
                        gameObject.transform.parent = other.gameObject.transform.parent.transform;
                    }
                }
            }

            if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, locomotiveLayerMask))
            {
                LocomotiveManager locomotiveManager = other.gameObject.GetComponent<LocomotiveManager>();
                if (locomotiveManager)
                {
                    if (_trainController)
                    {
                        _trainController.canRun = false;
                    }

                    locomotiveMatched = true;
                    locomotiveManager.OnTrainCollider(gameObject);
                }
            }
        }

        public void OnEventTriggered(TrainActionEvent e)
        {
            if (e.type == TrainActionEventType.Reset)
            {
                locomotiveMatched = false;
            }
        }
    }
}
