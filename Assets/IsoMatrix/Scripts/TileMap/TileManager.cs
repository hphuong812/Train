using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public float G;
    public float H;

    public float F
    {
        get
        {
            return G + H;
        }
    }

    public bool isBlock;
    public TileManager previous;
    public Vector2 GridLocation;
}
