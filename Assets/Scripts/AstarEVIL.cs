using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShouldNotBeUsed
{
    public class AStar
    {
        public static List<T> PathFinding<T>(T start, T goal, NodeMap<T> nodeMap, int breakPoint = 35) where T : class
        {
            var startNode = nodeMap.Map.FirstOrDefault(x => x.Value == start).Key;
            var goalNode = nodeMap.Map.FirstOrDefault(x => x.Value == goal).Key;


            var nodePath = AStarAlgorithm(
                startNode,
                goalNode,
                nodeMap.Nodes,
                breakPoint);

            if (nodePath == null)
                return null;

            return nodePath.Select(node => nodeMap.Map[node]).ToList();
        }
        static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node currentNode)
        {
            if (cameFrom.ContainsKey(currentNode))
            {
                List<Node> p = ReconstructPath(cameFrom, cameFrom[currentNode]);
                p.Add(currentNode);                                            // p + current_node ????
                return p;
            }
            else
                return new List<Node>();
        }

        private static List<Node> AStarAlgorithm(Node start, Node goal, List<Node> allNodes, int breakPoint)
        {
            var cameFrom = new Dictionary<Node, Node>();

            var closedSet = new List<Node>();            // Searched through nodes                  
            var openSet = new List<Node> { start };      // Nodes to be searched
            var gScore = new Dictionary<Node, float>();  // Optimal path length adjusted for costs
            var hScore = new Dictionary<Node, float>();  // Assumed path length
            var fScore = new Dictionary<Node, float>();  // Combination of g and h score
            gScore.Add(start, 0);

            foreach (Node node in allNodes)
            {
                //g_score.Add(node, 0f);
                fScore.Add(node, float.MaxValue);
            }

            int nodesChecked = 0;

            while (openSet.Count > 0)
            {  //  while open set is not empty 
                var temp = openSet.OrderBy(n => fScore[n]).ToList();
                var x = temp.First();

                if (x == goal)
                {
                    return ReconstructPath(cameFrom, goal);
                }

                openSet.Remove(x);
                closedSet.Add(x);

                foreach (Node y in x.ActiveNeighbours.OrderBy(_x => _x.DistanceTo(goal)))
                {
                    if (closedSet.Contains(y))
                        continue;
                    float tentativeGScore = gScore[x] + x.DistanceTo(y);

                    bool tentativeIsBetter = false;
                    if (!openSet.Contains(y))
                    {
                        openSet.Add(y);
                        hScore.Add(y, y.DistanceTo(goal));
                        tentativeIsBetter = true;
                    }
                    else if (tentativeGScore < gScore[y])
                    {
                        tentativeIsBetter = true;
                    }
                    if (tentativeIsBetter == true)
                    {
                        if (cameFrom.ContainsKey(y))
                            cameFrom[y] = x;
                        else
                            cameFrom.Add(y, x);
                        gScore[y] = tentativeGScore;

                        fScore[y] = gScore[y] + hScore[y];
                    }
                }
                nodesChecked++;
                if (nodesChecked > breakPoint)
                    return null;
            }
            return null;
        }
        public class NodeMap<T>
        {
            public List<Node> Nodes { get; } = new();
            public Dictionary<Node, T> Map { get; } = new();


            public void CreateEdgesUsingDistances(float maxDistance)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    for (int j = i + 1; j < Nodes.Count; j++)
                    {
                        if (Nodes[i].Distances[Nodes[j]] <= maxDistance)
                        {
                            Nodes[i].Neighbours.Add(Nodes[j]);
                            Nodes[j].Neighbours.Add(Nodes[i]);
                        }
                    }
                }
            }
            public void CalculateDistances()
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Distances.Add(Nodes[i], 0);
                    for (int j = i + 1; j < Nodes.Count; j++)
                    {
                        float dis = Vector2.Distance(Nodes[i].Position, Nodes[j].Position);
                        Nodes[i].Distances.Add(Nodes[j], dis);
                        Nodes[j].Distances.Add(Nodes[i], dis);
                    }
                }
            }

            public void AddNode(Node node, T t)
            {
                Nodes.Add(node);
                Map.Add(node, t);
            }
        }
        public class Node
        {
            public IEnumerable<Node> ActiveNeighbours { get => Neighbours.Where(x => x.IsActiveCheck()); }
            public List<Node> Neighbours { get; set; } = new List<Node>();
            public Dictionary<Node, float> Distances { get; set; } = new Dictionary<Node, float>();
            public float Cost { get; set; }
            public Vector2 Position { get; set; }
            public bool Disabled { get => IsActiveCheck(); }

            public delegate bool IsActiveCheckDelegate();

            public IsActiveCheckDelegate IsActiveCheck { get; set; } = () => true;

            public float DistanceTo(Node node)
            {
                if (Distances.TryGetValue(node, out float distance))
                    return distance;
                
                Distances.Add(node, Vector2.Distance(Position, node.Position));
                return Distances[node];
            }
        }
    }
}
