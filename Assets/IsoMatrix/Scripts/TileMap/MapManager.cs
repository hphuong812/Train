using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance
    {
        get { return instance; }
    }

    public Dictionary<Vector2, TileManager> map;

    private void Awake()
    {
        if (instance!= null && instance!= this)
        {
           Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void OnEnable()
    {
        map = new Dictionary<Vector2, TileManager>();
        var tileContainer = gameObject.GetComponentInChildren<Tilemap>();
        foreach (Transform child in tileContainer.transform)
        {
            TileManager tileManager = child.GetComponent<TileManager>();
            if (tileManager)
            {
                map.Add(new Vector2(child.transform.localPosition.x,child.transform.localPosition.z), tileManager);
            }
        }

        foreach (var item in map)
        {
            item.Value.GridLocation = item.Key;
        }
    }
}
