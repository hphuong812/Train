using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TrainName
{
    Num1,
    Num2,
    Num3,
    Num4,
    TNT
}
public class TrainManager : MonoBehaviour
{
    public TrainName TrainName = TrainName.Num1;
    public Transform DefaultParent;
    public UnityEvent OnRun;

    public void StartRun()
    {
        OnRun?.Invoke();
    }
}
