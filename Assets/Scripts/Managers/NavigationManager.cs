using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ShouldNotBeUsed;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance { get; private set; }

    private int maxPath = 75;

    private AStar.NodeMap<GridTile> nodeMap;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            Debug.LogError("Singleton duplicated!");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GridMap.MapChanged += OnMapChanged;
    }
    public GridTile GetStupidPath(Vector2Int startPos, Vector2Int goalPos)
    {
        GridTile start = GridMap.Instance.Tiles[startPos];
        GridTile goal = GridMap.Instance.Tiles[goalPos];

        if (startPos == goalPos)         // If you are already in a goal pos, you stay there
            return start;

        if (start.Neighbours.Count == 0) // If there is no neighbours, stay where you are
            return start;

        var availableNeighbours = start.Neighbours.Where(x => x.Walkable).ToList();

        if (availableNeighbours.Count == 1)  // If there is only one,
                                             // you have no choice
            return availableNeighbours[0];


        var sortedAllNeighbours = start.Neighbours
            .OrderBy(x => Vector2Int.Distance(x.Position, goal.Position))
            .ToList();
        if (sortedAllNeighbours[0].Walkable) // If the best option is available,
                                             // take it
            return sortedAllNeighbours[0];


        var sortedNeighbours = availableNeighbours
            .OrderBy(x => Vector2Int.Distance(x.Position, goal.Position))
            .Take(2).ToList();              // Discard the worst option

        float distance0 = Vector2Int.Distance(sortedNeighbours[0].Position, goal.Position);
        float distance1 = Vector2Int.Distance(sortedNeighbours[1].Position, goal.Position);
        int power = 10; // Determines how much more preferable is the better path

        float random = UnityEngine.Random.Range(0, Mathf.Pow(distance0, power) + Mathf.Pow(distance1, power));

        if (random > Mathf.Pow(distance0, power))       // Pick option randomly,
                                                            // the better option have higher chance of being picked
            return sortedNeighbours[0];
        else
            return sortedNeighbours[1];
    }
    public List<GridTile> GetPath(GridTile start, GridTile goal)
    {
        return AStar.PathFinding(start, goal, nodeMap, maxPath);
    }
    private void OnMapChanged(GridMap gridMap)
    {
        nodeMap = ConstructNodeMap(gridMap);
    }
    private AStar.NodeMap<GridTile> ConstructNodeMap(GridMap gridMap)
    {
        AStar.NodeMap<GridTile> nodeMap = new AStar.NodeMap<GridTile>();

        foreach (var item in gridMap.Tiles)
        {
            Vector2Int pos = item.Key;
            GridTile gridTile = item.Value;

            if (!gridTile.Walkable)
                continue;

            AStar.Node node = new AStar.Node();
            node.Position = pos;
            node.IsActiveCheck = () => { return GridMap.Instance.CanWalk(gridTile.Position); };
            nodeMap.AddNode(node, gridTile);
        }

        nodeMap.CalculateDistances();
        nodeMap.CreateEdgesUsingDistances(1f);

        return nodeMap;
    }
}