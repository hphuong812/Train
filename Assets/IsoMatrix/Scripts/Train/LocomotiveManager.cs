using System;
using System.Collections;
using System.Collections.Generic;
using IsoMatrix.Scripts.Train;
using UnityEngine;

public class LocomotiveManager : MonoBehaviour
{
    [SerializeField] private TrainController _trainController;
    [SerializeField] private Transform trainContainer;
    [SerializeField] private List<Transform> posTrain;
    [SerializeField] private int numTrain = 1;
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
}
