using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    public List<TileManager> FindPath(TileManager start, TileManager end)
    {
        List<TileManager> openList = new List<TileManager>();
        List<TileManager> closeList = new List<TileManager>();
        openList.Add(start);
        while (openList.Count>0)
        {
            TileManager currentTile = openList.OrderBy(x => x.F).First();
            openList.Remove(currentTile);
            closeList.Add(currentTile);
            if (currentTile== end)
            {
                return GetFinishedList(start, end);
            }

            // checker
            var neigbourTiles = GetNeighbourTile(currentTile);
            foreach (var neigbour in neigbourTiles)
            {
                if (neigbour.isBlock || closeList.Contains(neigbour))
                {
                    continue;
                }

                neigbour.G = GetManhattenDistance(start, neigbour);
                neigbour.H = GetManhattenDistance(end, neigbour);
                neigbour.previous = currentTile;
                if (!openList.Contains(neigbour))
                {
                    openList.Add(neigbour);
                }
            }
        }

        return new List<TileManager>();
    }

    private List<TileManager> GetFinishedList(TileManager start, TileManager end)
    {
        List<TileManager> finishList = new List<TileManager>();
        TileManager currentTile = end;
        // finishList.Add(start);
        while (currentTile!= start)
        {
            finishList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        finishList.Reverse();
        return finishList;
    }

    private float GetManhattenDistance(TileManager start, TileManager neigbour)
    {
        return Mathf.Abs(start.GridLocation.x - neigbour.GridLocation.x) + Mathf.Abs(start.GridLocation.y - neigbour.GridLocation.y);
    }

    private List<TileManager> GetNeighbourTile(TileManager currentTile)
    {
        var map = MapManager.Instance.map;
        List<TileManager> neighbours = new List<TileManager>();

        //top
        Vector2 locationCheck = new Vector2(currentTile.GridLocation.x + 1, currentTile.GridLocation.y);
        if (map.ContainsKey(locationCheck))
        {
            neighbours.Add(map[locationCheck]);
        }

        //bottom
        locationCheck = new Vector2(currentTile.GridLocation.x - 1, currentTile.GridLocation.y);
        if (map.ContainsKey(locationCheck))
        {
            neighbours.Add(map[locationCheck]);
        }
        //left
        locationCheck = new Vector2(currentTile.GridLocation.x , currentTile.GridLocation.y- 1);
        if (map.ContainsKey(locationCheck))
        {
            neighbours.Add(map[locationCheck]);
        }
        //right
        locationCheck = new Vector2(currentTile.GridLocation.x, currentTile.GridLocation.y+ 1);
        if (map.ContainsKey(locationCheck))
        {
            neighbours.Add(map[locationCheck]);
        }

        return neighbours;
    }
}
