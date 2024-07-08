using System;
using System.Collections;
using System.Collections.Generic;
using IsoMatrix.Scripts.Rail;
using IsoMatrix.Scripts.Utilities;
using UnityEngine;

public class RailGroupManager : MonoBehaviour
{
    [NonSerialized]
    public Dictionary<TrainManager, RailType> DirTrain = new Dictionary<TrainManager, RailType>();

    public void AddTrainToRail(TrainManager train, RailType rail)
    {
        if (DirTrain.ContainsKey(train))
        {
            DirTrain[train] = rail;
        }
        else
        {
            DirTrain.Add(train, rail);
        }
    }
}
