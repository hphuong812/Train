using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TrainName
{
    Kimbap,
    Tokbokki,
    Banhtet,
    Banhmi
}
public class TrainManager : MonoBehaviour
{
    public TrainName TrainName = TrainName.Kimbap;
    public Transform DefaultParent;
    public UnityEvent OnRun;

    public void StartRun()
    {
        OnRun?.Invoke();
    }
}
