using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShouldNotBeUsed
{
    public class Astar
    {
        public static List<T> PathFingding<T>(T start, T goal, NodeMap<T> nodeMap, int breakPoint = 35) where T : class
        {
            var startNode = nodeMap.Map.FirstOrDefault(x => x.Value == start).Key;
            var goalNode = nodeMap.Map.FirstOrDefault(x => x.Value == goal).Key;


            List<Node> nodePath = AstarAlghorytm(
                startNode,
                goalNode,
                nodeMap.Nodes,
                breakPoint);

            if (nodePath == null)
                return null;

            List<T> path = new List<T>();

            foreach (Node node in nodePath)
            {
                path.Add(nodeMap.Map[node]);
            }

            return path;
        }
        static List<Node> ReconstructPath(Dictionary<Node, Node> came_from, Node current_node)
        {
            if (came_from.ContainsKey(current_node))
            {
                List<Node> p = ReconstructPath(came_from, came_from[current_node]);
                p.Add(current_node);                                            // p + current_node ????
                return p;
            }
            else
                return new List<Node>();
        }
        static List<Node> AstarAlghorytm(Node start, Node goal, List<Node> allNodes, int breakPoint)
        {
            Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();

            List<Node> closedset = new List<Node>();                            // Zbiór wierzchołków przejrzanych.
            List<Node> openset = new List<Node>();                              // Zbiór wierzchołków nieodwiedzonych, sąsiadujących z odwiedzonymi.
            openset.Add(start);
            Dictionary<Node, float> g_score = new Dictionary<Node, float>();    // Długość optymalnej trasy.
            Dictionary<Node, float> h_score = new Dictionary<Node, float>();    // nwm kuźwa
            Dictionary<Node, float> f_score = new Dictionary<Node, float>();    // ojojoj...


            g_score.Add(start, 0);

            foreach (Node node in allNodes)
            {
                //g_score.Add(node, 0f);
                f_score.Add(node, float.MaxValue);
            }

            int nodesChecked = 0;

            while (openset.Count > 0)
            {  //  while openset is not empty 
                List<Node> temp = openset.OrderBy(n => f_score[n]).ToList();
                Node x = temp.First();


                if (x == goal)
                {

                    return ReconstructPath(came_from, goal);
                }
                    

                openset.Remove(x);
                closedset.Add(x);

                foreach (Node y in x.ActiveNeighbours.OrderBy(_x => _x.DistanceTo(goal)))
                {
                    if (closedset.Contains(y))
                        continue;
                    float tentative_g_score = g_score[x] + x.DistanceTo(y);

                    bool tentative_is_better = false;
                    if (!openset.Contains(y))
                    {
                        openset.Add(y);
                        h_score.Add(y, y.DistanceTo(goal));
                        tentative_is_better = true;
                    }
                    else if (tentative_g_score < g_score[y])
                    {
                        tentative_is_better = true;
                    }
                    if (tentative_is_better == true)
                    {
                        if (came_from.ContainsKey(y))
                            came_from[y] = x;
                        else
                            came_from.Add(y, x);
                        g_score[y] = tentative_g_score;

                        f_score[y] = g_score[y] + h_score[y];
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
            public List<Node> Nodes = new List<Node>();
            public Dictionary<Node, T> Map = new Dictionary<Node, T>();


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

            public IsActiveCheckDelegate IsActiveCheck = () => { return true; };

            public float DistanceTo(Node node)
            {
                if (Distances.TryGetValue(node, out float distance))
                    return distance;
                else
                {
                    Distances.Add(node, Vector2.Distance(Position, node.Position));
                    return Distances[node];
                }
            }
        }
    }
}
