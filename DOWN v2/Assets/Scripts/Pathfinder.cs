using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    public List<Tile> FindPath(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
            {
                return GetFinishedList(start, end);
            }

            var neighborTiles = GetNeighborTiles(currentTile);

            foreach(var neighbor in neighborTiles)
            {
                if (neighbor.isOccupied || closedList.Contains(neighbor))
                {
                    continue;
                }

                neighbor.G = GetManhattanDistance(start, neighbor);
                neighbor.H = GetManhattanDistance(end, neighbor);

                neighbor.previous = currentTile;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }
        }
        return new List<Tile>();
    }

    private List<Tile> GetFinishedList(Tile start, Tile end)
    {
        List<Tile> finishedList = new List<Tile>();

        Tile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        finishedList.Reverse();

        return finishedList;
    }

    private int GetManhattanDistance(Tile start, Tile neighbor)
    {
        if (start != null)
        {
            return Mathf.Abs(start.gridLocation.x - neighbor.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbor.gridLocation.y);
        }
        else
        {
            return 0;
        }
    }

    public List<Tile> GetNeighborTiles(Tile currentTile)
    {
        var tiles = GridManager.Instance.tiles;

        List<Tile> neighbors = new List<Tile>();
        //Below
        Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);
        if(tiles.ContainsKey(locationToCheck))
        {
            neighbors.Add(tiles[locationToCheck]);
        }
        //Above
        locationToCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);
        if (tiles.ContainsKey(locationToCheck))
        {
            neighbors.Add(tiles[locationToCheck]);
        }
        //Left
        locationToCheck = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);
        if (tiles.ContainsKey(locationToCheck))
        {
            neighbors.Add(tiles[locationToCheck]);
        }
        //Right
        locationToCheck = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);
        if (tiles.ContainsKey(locationToCheck))
        {
            neighbors.Add(tiles[locationToCheck]);
        }

        return neighbors;
    }
}
