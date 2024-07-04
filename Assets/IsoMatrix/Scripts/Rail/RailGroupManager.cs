using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGroupManager : MonoBehaviour
{
    private Dictionary<Vector2, List<TrainManager>> topLeft;
    private Dictionary<Vector2, List<TrainManager>> topRight;
    private Dictionary<Vector2, List<TrainManager>> bottomLeft;
    private Dictionary<Vector2, List<TrainManager>> bottomRight;
    private Dictionary<Vector2, List<TrainManager>> top;
    private Dictionary<Vector2, List<TrainManager>> right;
    private List<TrainManager> trainTopLeft = new List<TrainManager>{};
    private List<TrainManager> trainTopRight = new List<TrainManager>{};
    private List<TrainManager> trainBottomRight = new List<TrainManager>{};
    private List<TrainManager> trainBottomLeft = new List<TrainManager>{};
    private List<TrainManager> trainRight = new List<TrainManager>{};
    private List<TrainManager> trainTop = new List<TrainManager>{};

    public void AddTrainToRail(TrainManager train, RailType rail)
    {
        switch (rail)
        {
            case RailType.top_left:
                Vector2 point = new Vector2(0, 1);
                AddTrain(train, topLeft, trainTopLeft, point);
                break;
            case RailType.top_right:
                Vector2 point2 = new Vector2(0, 1);
                AddTrain(train, topRight, trainTopRight, point2);
                break;
            case RailType.bottom_right:
                Vector2 point3 = new Vector2(0, 1);
                AddTrain(train, bottomRight, trainBottomRight, point3);
                break;
            case RailType.bottom_left:
                Vector2 point4 = new Vector2(0, 1);
                AddTrain(train, bottomLeft, trainBottomLeft, point4);
                break;
            case RailType.top:
                Vector2 point5 = new Vector2(0, 1);
                AddTrain(train, top, trainTop, point5);
                break;
            case RailType.right:
                Vector2 point6 = new Vector2(0, 1);
                AddTrain(train, right, trainRight, point6);
                break;
        }
    }

    public void AddTrain(TrainManager train, Dictionary<Vector2, List<TrainManager>> dir, List<TrainManager> listTrain, Vector2 point)
    {
        if (dir.Count == 0)
        {
            listTrain.Add(train);
            dir.Add(point, listTrain);
        }
        else
        {
            listTrain.Add(train);
            dir[point] = listTrain;
        }
    }
}
