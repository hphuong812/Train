using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Data;
using IsoMatrix.Scripts.Level;
using IsoMatrix.Scripts.Train;
using IsoMatrix.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LocomotiveManager : MonoBehaviour, IEventListener<TrainActionEvent>
{
    [SerializeField] private LayerMask exitLayerMask;
    [SerializeField] private LayerMask completeLayerMask;
    [SerializeField] private TrainController _trainController;
    [SerializeField] private Transform trainContainer;
    [SerializeField] private int numTrain = 1;
    public UnityEvent OnRun;
    public List<String> ListTrainConrect { get; set; }
    private List<String> ListTrainGet = new List<string>();
    private int countTrain = 0;
    private bool isCheck;

    private void Start()
    {
        EventManager.Subscribe(this);
    }
    private void OnDisable()
    {
        EventManager.Unsubscribe(this);
    }

    public void OnTrainCollider(GameObject train)
    {
        train.transform.parent = trainContainer;
    }

    private void LateUpdate()
    {
        countTrain = trainContainer.childCount;
        if (countTrain == numTrain)
        {
            if (!isCheck)
            {
                CheckConrectOrder();
                isCheck = true;
            }
        }
    }

    private void CheckConrectOrder()
    {
        ListTrainGet.Clear();
        foreach (Transform child in trainContainer.transform)
        {
            TrainManager trainManager = child.gameObject.GetComponent<TrainManager>();
            ListTrainGet.Add(trainManager.TrainName.ToString());
        }

        if (Enumerable.SequenceEqual(ListTrainConrect, ListTrainGet))
        {
            TrainActionEvent.Trigger(TrainActionEventType.LocomotiveRun);
            OnRun.Invoke();
        }
    }

    public void CallRun()
    {
        _trainController.canRun = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, exitLayerMask))
        {
            other.gameObject.SetActive(false);
            Destroy(gameObject);
            GameConfig.Instance.CurrentLevel += 1;
            if (GameConfig.Instance.CurrentLevel >14)
            {
                GameConfig.Instance.CurrentLevel = Random.Range(0, 14);
            }
            LevelEvent.Trigger(LevelEventType.NextLevel, GameConfig.Instance.CurrentLevel);

        }
        if (LayerMarkChecker.LayerInLayerMask(other.gameObject.layer, completeLayerMask))
        {
            other.gameObject.SetActive(false);
            ScreenEvent.Trigger(ScreenEventType.ScreenIn);
        }
    }

    public void OnEventTriggered(TrainActionEvent e)
    {
        if (e.type == TrainActionEventType.Reset)
        {
            isCheck = false;
        }
    }
}
