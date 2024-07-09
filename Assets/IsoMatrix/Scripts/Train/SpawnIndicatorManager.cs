using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IsoMatrix.Scripts.Train
{
    public class SpawnIndicatorManager : MonoBehaviour
    {
        [SerializeField] private ArrowIndicatorController arrowIndicatorPrefab;
        [SerializeField] private int coutArrow = 3;
        [SerializeField] private TrainController _trainController;

        private void Awake()
        {
            for (int i = 0; i < coutArrow; i++)
            {
                ArrowIndicatorController arrow = SpawnArrowIndicator();
                _trainController.listArrowIndicator.Add(arrow);
            }
        }

        public ArrowIndicatorController SpawnArrowIndicator()
        {
            var indicator = Instantiate(arrowIndicatorPrefab, transform);
            ArrowIndicatorController arrowIndicatorController = indicator.GetComponent<ArrowIndicatorController>();
            return arrowIndicatorController;
        }
    }
}