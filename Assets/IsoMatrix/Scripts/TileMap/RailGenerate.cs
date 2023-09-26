using System;
using System.Collections;
using System.Collections.Generic;
using IsoMatrix.Scripts.Rail;
using UnityEngine;

public enum RailType
{
    none,
    right,
    top,
    top_left,
    top_right,
    bottom_left,
    bottom_right,
    top_top_left,
    top_top_right,
    top_bottom_left,
    top_bottom_right,
    right_top_left,
    right_top_right,
    right_bottom_left,
    right_bottom_right

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
            return RailType.right;
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
            return RailType.top;
        }

        if (direction == new Vector2(1,1))
        {
            if (passDirection.x < futureDirection.x)
            {
                return RailType.bottom_right;
            }
            else
            {
                return RailType.top_left;
            }
        }

        if (direction == new Vector2(-1,1))
        {
            if (passDirection.x < futureDirection.x)
            {
                return RailType.bottom_left;
            }
            else
            {
                return RailType.top_right;
            }
        }

        if (direction == new Vector2(1,-1))
        {
            if (passDirection.x > futureDirection.x)
            {
                return RailType.top_right;
            }
            else
            {
                return RailType.bottom_left;
            }
        }

        if (direction == new Vector2(-1,-1))
        {
            if (passDirection.x > futureDirection.x)
            {
                return RailType.top_left;
            }
            else
            {
                return RailType.bottom_right;
            }
        }

        return RailType.none;
    }

    public RailOption GetOption(RailType type)
    {
        if (type == RailType.top_left || type == RailType.bottom_right || type == RailType.top_right || type == RailType.bottom_left)
        {
            return RailOption.angle;
        }
        else
        {
            return RailOption.edge;
        }
    }

    public string CheckAroundRail(List<RailManager> listRail, Vector3 railCheckPos, RailType TypeCheck)
    {
        // if (TypeCheck== "rail_"+RailType.top_bottom_left.ToString() || TypeCheck== "rail_"+RailType.top_top_left.ToString() || TypeCheck== "rail_"+RailType.top_bottom_right.ToString() || TypeCheck== "rail_"+RailType.top_top_right.ToString())
        // {
        //     Debug.Log("call_+1");
        //     return CheckAroundRailTop(listRail, railCheckPos);
        // }else if (TypeCheck== "rail_"+RailType.right_top_right.ToString() || TypeCheck== "rail_"+RailType.right_top_left.ToString() || TypeCheck== "rail_"+RailType.right_bottom_right.ToString() || TypeCheck== "rail_"+RailType.right_bottom_left.ToString())
        // {
        //     Debug.Log("call_+2");
        //     return CheckAroundRailRight(listRail, railCheckPos);
        // }
        RailType typeFirst = RailType.none;
        string typeFinal = "rail_";

        if (TypeCheck == RailType.top_right)
        {
            typeFirst = CheckAroundRailTopRight(listRail, railCheckPos);
        }else if (TypeCheck == RailType.top_left)
        {
            typeFirst = CheckAroundRailTopLeft(listRail, railCheckPos);
        }else if (TypeCheck == RailType.bottom_right)
        {
            typeFirst = CheckAroundRailBottomRight(listRail, railCheckPos);
        }else if (TypeCheck == RailType.bottom_left)
        {
            typeFirst = CheckAroundRailBottomLeft(listRail, railCheckPos);
        }

        if (typeFirst != RailType.none)
        {
            typeFinal += typeFirst.ToString() + "_" + TypeCheck.ToString();
        }
        else
        {
            typeFinal += TypeCheck.ToString();
        }


        return typeFinal;
    }

    //z -, x-
    private RailType CheckAroundRailBottomLeft(List<RailManager> listRail, Vector3 railCheck)
    {
        var count = 0;
        foreach (var rail in listRail)
        {
            if (rail.transform.localPosition == new Vector3(railCheck.x, 1, railCheck.z -1 )&&
                (rail.railType == RailType.right ||  rail.railType == RailType.bottom_left|| rail.railType == RailType.top_left
                 ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right
                 || rail.railType == RailType.top_top_left || rail.railType == RailType.top_bottom_left
                 ))
            {
                return RailType.right;
            }else if (rail.transform.localPosition == new Vector3(railCheck.x-1, 1, railCheck.z)&&
                      (rail.railType == RailType.top ||  rail.railType == RailType.bottom_left|| rail.railType == RailType.bottom_right
                       || rail.railType == RailType.top_top_left || rail.railType == RailType.top_top_right||rail.railType == RailType.top_bottom_left || rail.railType == RailType.top_bottom_right
                       ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right
                       ))
            {
                return RailType.top;
            }
        }
        return RailType.none;
    }

    // z+, x-
    private RailType CheckAroundRailBottomRight(List<RailManager> listRail, Vector3 railCheck)
    {
        var count = 0;
        foreach (var rail in listRail)
        {
            if (rail.transform.localPosition == new Vector3(railCheck.x, 1, railCheck.z + 1 )&&
                (rail.railType == RailType.right ||  rail.railType == RailType.bottom_right|| rail.railType == RailType.top_right
                 ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right
                 || rail.railType == RailType.top_top_right || rail.railType == RailType.top_bottom_right
                 ))
            {
                return RailType.right;
            }else if (rail.transform.localPosition == new Vector3(railCheck.x-1, 1, railCheck.z)&&
                      (rail.railType == RailType.top ||  rail.railType == RailType.bottom_left|| rail.railType == RailType.bottom_right
                      || rail.railType == RailType.top_top_left || rail.railType == RailType.top_top_right||rail.railType == RailType.top_bottom_left || rail.railType == RailType.top_bottom_right
                      ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right
                       ))
            {
                return RailType.top;
            }
        }
        return RailType.none;
    }

    // z-, x+
    private RailType CheckAroundRailTopLeft(List<RailManager> listRail, Vector3 railCheck)
    {
        var count = 0;
        foreach (var rail in listRail)
        {
            if (rail.transform.localPosition == new Vector3(railCheck.x, 1, railCheck.z -1 )&&
                (rail.railType == RailType.right ||  rail.railType == RailType.bottom_left|| rail.railType == RailType.top_left
                 ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right
                 || rail.railType == RailType.top_top_left || rail.railType == RailType.top_bottom_left
                 ))
            {
                return RailType.right;
            }else if (rail.transform.localPosition == new Vector3(railCheck.x+1, 1, railCheck.z )&&
                      (rail.railType == RailType.top ||  rail.railType == RailType.top_left|| rail.railType == RailType.top_right
                       || rail.railType == RailType.top_top_left || rail.railType == RailType.top_top_right ||rail.railType == RailType.top_bottom_left || rail.railType == RailType.top_bottom_right
                       ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right
                       ))
            {
                return RailType.top;
            }
        }
        return RailType.none;
    }


    //z+, x+
    private RailType CheckAroundRailTopRight(List<RailManager> listRail, Vector3 railCheck)
    {
        var count = 0;
        foreach (var rail in listRail)
        {
            if (rail.transform.localPosition == new Vector3(railCheck.x, 1, railCheck.z + 1 )&&
                (rail.railType == RailType.right ||  rail.railType == RailType.bottom_right|| rail.railType == RailType.top_right
                 ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right
                 || rail.railType == RailType.top_top_right || rail.railType == RailType.top_bottom_right
                ))
            {
                return RailType.right;
            }else if (rail.transform.localPosition == new Vector3(railCheck.x+1, 1, railCheck.z )&&
                      (rail.railType == RailType.top ||  rail.railType == RailType.top_left|| rail.railType == RailType.top_right
                       || rail.railType == RailType.top_top_left || rail.railType == RailType.top_top_right ||rail.railType == RailType.top_bottom_left || rail.railType == RailType.top_bottom_right
                       ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right
                       ))
            {
                return RailType.top;
            }
        }
        return RailType.none;
    }

    private bool CheckAroundRailRight(List<RailManager> listRail, Vector3 railCheck)
    {
        var count = 0;
        foreach (var rail in listRail)
        {
            if (rail.transform.localPosition == new Vector3(railCheck.x, 1, railCheck.z -1 )&&
                (rail.railType == RailType.right ||  rail.railType == RailType.bottom_right|| rail.railType == RailType.top_right
                 ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right))
            {
                count++;
            }

            if (rail.transform.localPosition == new Vector3(railCheck.x, 1, railCheck.z +1)&&
                (rail.railType == RailType.right ||  rail.railType == RailType.bottom_left|| rail.railType == RailType.top_left
                 ||rail.railType == RailType.right_bottom_left || rail.railType == RailType.right_bottom_right ||rail.railType == RailType.right_top_left || rail.railType == RailType.right_top_right))
            {
                count++;
            }
        }

        if (count == 2)
        {
            return true;
        }
        return false;
    }

    private bool CheckAroundRailTop(List<RailManager> listRail, Vector3 railCheck)
    {
        var count = 0;
        foreach (var rail in listRail)
        {
            if (rail.transform.localPosition == new Vector3(railCheck.x+1, 1, railCheck.z )&&
                (rail.railType == RailType.top ||  rail.railType == RailType.top_left|| rail.railType == RailType.top_right
                 || rail.railType == RailType.top_top_left || rail.railType == RailType.top_top_right ||rail.railType == RailType.top_bottom_left || rail.railType == RailType.top_bottom_right ))
            {
                Debug.Log("call_+13");
                count++;
            }

            if (rail.transform.localPosition == new Vector3(railCheck.x-1, 1, railCheck.z)&&
                (rail.railType == RailType.top ||  rail.railType == RailType.bottom_left|| rail.railType == RailType.bottom_right
                 || rail.railType == RailType.top_top_left || rail.railType == RailType.top_top_right||rail.railType == RailType.top_bottom_left || rail.railType == RailType.top_bottom_right))
            {
                Debug.Log("call_+15");
                count++;
            }
        }

        if (count == 2)
        {
            return true;
        }
        return false;
    }
}
