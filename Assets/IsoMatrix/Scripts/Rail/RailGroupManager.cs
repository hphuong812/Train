using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGroupManager : MonoBehaviour
{
    [NonSerialized]
    public Dictionary<TrainManager, RailType> DirTrain = new Dictionary<TrainManager, RailType>();

    public void AddTrainToRail(TrainManager train, RailType rail)
    {
        foreach (var dir in DirTrain)
        {
            if (dir.Key.name == train.name)
            {
                Debug.Log(dir.Key+"||"+ rail);
                ChangeValueTrain(dir.Key, rail);
                return;
            }
        }
        Debug.Log(train+"||"+ rail);
        DirTrain.Add(train, rail);
    }

    public void ChangeValueTrain(TrainManager key, RailType railType)
    {
        DirTrain[key] = railType;
    }
    //
    // public void AddTrain(TrainManager train, Dictionary<Vector2, List<TrainManager>> dir, List<TrainManager> listTrain, Vector2 point)
    // {
    //     if (dir.Count == 0)
    //     {
    //         listTrain.Add(train);
    //         dir.Add(point, listTrain);
    //     }
    //     else
    //     {
    //         listTrain.Add(train);
    //         dir[point] = listTrain;
    //     }
    // }
}
