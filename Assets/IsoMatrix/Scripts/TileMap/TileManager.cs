using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public enum TileSpriteName
{
    tex_tile_select,
    tex_tile_destroy
}
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
    public UnityEvent Selected;
    public UnityEvent UnSelected;
    public SpriteRenderer tileSelect;

    public bool isBlock;
    [NonSerialized]
    public TileManager preTile ;
    [NonSerialized]
    public TileManager previous;
    [NonSerialized]
    public Vector2 GridLocation;

    private bool isSelected;

    public void ChangeSprite(TileSpriteName spriteTile)
    {
        Addressables.LoadAssetAsync<Sprite>(spriteTile.ToString()).Completed += handle =>
        {
            tileSelect.sprite = handle.Result;
        };
    }

    public void OnSelect()
    {
        if (!isSelected)
        {
            Selected?.Invoke();
            isSelected = true;
        }
    }
    public void OnUnSelect()
    {
        if (isSelected)
        {
            UnSelected?.Invoke();
            isSelected = false;
        }
    }
}
