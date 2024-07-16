using System;
using System.Collections;
using System.Collections.Generic;
using ADN.Meta.Core;
using IsoMatrix.Scripts.Rail;
using IsoMatrix.Scripts.Train;
using IsoMatrix.Scripts.Utilities;
using UnityEngine;

public class RailGroupManager : MonoBehaviour, IEventListener<TrainActionEvent>
{
    [NonSerialized]
    public Dictionary<TrainManager, List<RailType>> DirTrain = new Dictionary<TrainManager,  List<RailType>>();

    private void Awake()
    {
        EventManager.Subscribe(this);
    }

    public void AddTrainToRail(TrainManager train, RailType rail)
    {
        if (DirTrain.ContainsKey(train))
        {
            // DirTrain[train] = rail;
            DirTrain[train].Add(rail);  
        }
        else
        {
            List<RailType> listRail = new List<RailType> { rail };
            DirTrain.Add(train, listRail);
        }
    }
    
    public void OnEventTriggered(TrainActionEvent e)
    {
        if (e.type == TrainActionEventType.Reset)
        {
            DirTrain.Clear();
        }
    }
}
