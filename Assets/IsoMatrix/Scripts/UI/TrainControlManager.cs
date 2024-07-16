using System;
using System.Collections;
using System.Collections.Generic;
using IsoMatrix.Scripts.Train;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrainControlManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject runText;
    [SerializeField] private GameObject stopText;
    private int countItem;
    private TrainController _trainController;
    private TrainManager _trainManager;

    public void SetUpTrainControl(TrainController train, int index)
    {
        _trainController = train;
        _trainManager = train.gameObject.GetComponent<TrainManager>();
        string name = _trainManager.TrainName.ToString();
        Addressables.LoadAssetAsync<Sprite>(name).Completed += handle =>
        {
            iconImage.sprite = handle.Result;
        };
        indexText.text = index.ToString();
        countItem = index;
    }

    public void ControlTrain()
    {
        _trainController.canRun = !_trainController.canRun; 
        _trainManager.StartRun();
    }

    private void Update()
    {
        runText.gameObject.SetActive(false);
        stopText.gameObject.SetActive(true);
        if (_trainController.canRun)
        {
            runText.gameObject.SetActive(true);
            stopText.gameObject.SetActive(false);
        }
    }
}
