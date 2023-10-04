using System;
using System.Collections;
using System.Collections.Generic;
using IsoMatrix.Scripts.Data;
using IsoMatrix.Scripts.Level;
using IsoMatrix.Scripts.Train;
using IsoMatrix.Scripts.UI;
using UnityEngine;

public class LocomotiveManager : MonoBehaviour
{
    [SerializeField] private LayerMask exitLayerMask;
    [SerializeField] private LayerMask completeLayerMask;
    [SerializeField] private TrainController _trainController;
    [SerializeField] private Transform trainContainer;
    [SerializeField] private int numTrain = 1;
    [SerializeField] private List<Transform> posTrain;
    private int countTrain = 0;

    public void OnTrainCollider(GameObject train)
    {
        train.transform.parent = trainContainer;
    }

    private void LateUpdate()
    {
        countTrain = trainContainer.childCount;
        if (countTrain == numTrain)
        {
            _trainController.canRun = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, exitLayerMask))
        {
            LevelEvent.Trigger(LevelEventType.NextLevel, GameConfig.Instance.CurrentLevel++);
        }
        if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, completeLayerMask))
        {
            ScreenEvent.Trigger(ScreenEventType.ScreenIn);
        }
    }
}
