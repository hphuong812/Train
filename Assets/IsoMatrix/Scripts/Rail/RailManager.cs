using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RailOption
{
    edge,
    angle
}
public class RailManager : MonoBehaviour
{
    public RailType railType;
    public RailOption railOption;
}
