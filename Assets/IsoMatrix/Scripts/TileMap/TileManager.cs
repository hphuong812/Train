using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [NonSerialized]
    public float G;
    [NonSerialized]
    public float H;

    public float F
    {
        get
        {
            return G + H;
        }
    }

    public bool isBlock;
    [NonSerialized]
    public TileManager preTile ;
    [NonSerialized]
    public TileManager previous;
    [NonSerialized]
    public Vector2 GridLocation;
}
