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
    private int countItem;
    private TrainController _trainController;

    public void SetUpTrainControl(TrainController train, int index)
    {
        _trainController = train;
        TrainManager trainManager = train.gameObject.GetComponent<TrainManager>();
        string name = trainManager.TrainName.ToString();
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
    }
}
