using System;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Rail;
using UnityEngine;
using UnityEngine.Events;

namespace IsoMatrix.Scripts.Train
{
    public class TrainColliderManager : MonoBehaviour,  IEventListener<TrainActionEvent>
    {
        [SerializeField]
        private LayerMask trainLayerMask;
        [SerializeField] private LayerMask locomotiveLayerMask;
        [SerializeField] private LayerMask deadZoneLayerMask;
        [SerializeField] private LayerMask speedUpZoneLayerMask;
        [SerializeField] private LayerMask railLayerMask;

        [SerializeField] private TrainController _trainController;
        [SerializeField] private TrainManager _trainManager;

        private bool locomotiveMatched;
        public bool LocomotiveMatched => locomotiveMatched;
        public UnityEvent TrainReset;
        public UnityEvent TrainMatch;

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
                if (locomotiveMatched)
                {
                    return;
                }
                if (_trainController)
                {
                    _trainController.canRun = false;
                    TrainMatch?.Invoke();
                }

                TrainColliderManager colliderTrainColliderManager = other.gameObject.GetComponent<TrainColliderManager>();
                if (colliderTrainColliderManager)
                {
                    if (colliderTrainColliderManager.LocomotiveMatched)
                    {
                        gameObject.transform.parent = other.gameObject.transform.parent.transform;
                        locomotiveMatched = true;
                    }
                }
            }

            if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, locomotiveLayerMask))
            {
                LocomotiveManager locomotiveManager = other.gameObject.GetComponent<LocomotiveManager>();
                if (locomotiveManager)
                {
                    if (_trainManager.TrainName == TrainName.TNT)
                    {
                        _trainController.canRun = false;
                        TrainMatch?.Invoke();
                        return;
                    }
                    if (_trainController)
                    {
                        _trainController.canRun = false;
                        TrainMatch?.Invoke();
                    }

                    locomotiveMatched = true;
                    locomotiveManager.OnTrainCollider(gameObject);
                }
            }
            
            if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, deadZoneLayerMask))
            {
                if (_trainManager.TrainName == TrainName.TNT)
                {
                    return;
                }

                ResetTrain();
            }
            
            if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, speedUpZoneLayerMask))
            {
                _trainController.m_Speed = 4;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, railLayerMask))
            {
                RailManager railManager = other.gameObject.GetComponent<RailManager>();
                railManager.isFix = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, railLayerMask))
            {
                RailManager railManager = other.gameObject.GetComponent<RailManager>();
                railManager.isFix = false;
            }
        }

        public void ResetTrain()
        {
            TrainActionEvent.Trigger(TrainActionEventType.Reset);
        }

        public void OnEventTriggered(TrainActionEvent e)
        {
            if (e.type == TrainActionEventType.Reset)
            {
                locomotiveMatched = false;
                TrainReset.Invoke();
                if (_trainManager.TrainName == TrainName.TNT)
                {
                    _trainController.RespawnDefault();
                    _trainController.m_Speed = 1;
                    _trainController.canRun = true;
                }
            }
        }

        public void MoveTrain()
        {
            _trainController.canRun = true;
        }
    }
}
