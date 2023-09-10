using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RailType
{
    none,
    left,
    right,
    top,
    bottom,
    top_left_down,
    top_right_down,
    bottom_left_down,
    bottom_right_down,
    top_left_up,
    top_right_up,
    bottom_left_up,
    bottom_right_up
}
public class RailGenerate
{
    public RailType RailDirection(TileManager previusTile, TileManager currentTile, TileManager futureTile)
    {
        Vector2 passDirection =
            previusTile != null ? currentTile.GridLocation - previusTile.GridLocation : Vector2Int.zero;
        Vector2 futureDirection =
            futureTile != null ? futureTile.GridLocation - currentTile.GridLocation : Vector2.zero;

        Vector2 direction =
            futureDirection != passDirection ? passDirection + futureDirection : futureDirection;
        if (direction == new Vector2(0,1))
        {
            return RailType.left;
        }
        if (direction == new Vector2(0,-1))
        {
            return RailType.right;
        }
        if (direction == new Vector2(1,0))
        {
            return RailType.top;
        }
        if (direction == new Vector2(-1,0))
        {
            return RailType.bottom;
        }

        if (direction == new Vector2(1,1))
        {
            if (passDirection.x < futureDirection.x)
            {
                return RailType.bottom_right_up;
            }
            else
            {
                return RailType.top_left_up;
            }
        }

        if (direction == new Vector2(-1,1))
        {
            if (passDirection.x < futureDirection.x)
            {
                return RailType.bottom_left_down;
            }
            else
            {
                return RailType.top_right_down;
            }
        }

        if (direction == new Vector2(1,-1))
        {
            if (passDirection.x > futureDirection.x)
            {
                return RailType.top_right_up;
            }
            else
            {
                return RailType.bottom_left_up;
            }
        }

        if (direction == new Vector2(-1,-1))
        {
            if (passDirection.x > futureDirection.x)
            {
                return RailType.top_left_down;
            }
            else
            {
                return RailType.bottom_right_down;
            }
        }

        return RailType.none;
    }
}
