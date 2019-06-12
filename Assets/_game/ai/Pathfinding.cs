using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Utilities;

namespace Wakaman.AI
{
    public class Pathfinding : MonoBehaviour
    {
        // Definitions
        public class Graph
        {
            public Dictionary<Vector3Int, Node> nodes;

            public Graph()
            {
                nodes = new Dictionary<Vector3Int, Node>();
            }
        }
        public class Node
        {
            public Vector3Int pos;
            public List<Node> neighbours;

            public Node()
            {
                pos = Vector3Int.zero;
                neighbours = new List<Node>();
            }
        }

        // Singleton
        public static Pathfinding instance = null;

        // Map Info Editor-Tweakables
        [SerializeField] private Tilemap collisionMap;
        [SerializeField] private Transform upperLeftBound;
        [SerializeField] private Transform lowerRightBound;
        [SerializeField] private Transform cageCenter;
        [SerializeField] private Transform cageExit;

        // Runtime
        private Graph graph;

        // Accessors
        public Vector3 CageCenter {
            get => cageCenter.position;
        }
        public Vector3 CageExit {
            get => cageExit.position;
        }

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            instance = this;
            GenerateGraph();
        }

        // -------------------------- //
        // Initialization
        // -------------------------- //

        private void GenerateGraph()
        {
            graph = new Graph();

            Vector3Int start = collisionMap.WorldToCell(upperLeftBound.position);
            Vector3Int end = collisionMap.WorldToCell(lowerRightBound.position);
            int xLength = Mathf.Abs(end.x - start.x);
            int yLength = Mathf.Abs(start.y - end.y);

            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    int x = start.x + i;
                    int y = start.y - j;
                    Vector3Int pos = new Vector3Int(x, y, start.z);
                    bool hasTile = collisionMap.HasTile(pos);
                    if (!hasTile)
                    {
                        Node node = new Node();
                        node.pos = new Vector3Int(x, y, end.z);
                        var neighbours = new Vector3Int[] {
                            node.pos + Vector3Int.up,
                            node.pos + Vector3Int.left
                        };
                        foreach (var neighbour in neighbours)
                        {
                            if (graph.nodes.ContainsKey(neighbour))
                            {
                                node.neighbours.Add(graph.nodes[neighbour]);
                                graph.nodes[neighbour].neighbours.Add(node);
                            }
                        }
                        graph.nodes.Add(node.pos, node);
                    }
                }
            }
            /* 
             * Debug code: 
             * Was used to check if nodes neighbourhood
             * was being generated properly.
             * 
            foreach(var node in graph.nodes.Values)
            {
                GameObject go = new GameObject("Graph node pos(" + node.pos + ") neighbours(" + node.neighbours.Count + ")[");
                foreach (var n in node.neighbours)
                    go.name += n.pos + ",";
                go.name += "]";
                go.transform.parent = collisionMap.transform;
                go.transform.position = collisionMap.GetCellCenterWorld(new Vector3Int(node.pos.x, node.pos.y, start.z));
            } *
            */
        }

        // -------------------------- //
        // Pathfinding algorithm
        // -------------------------- //

        public Vector3Int[] GetPath(Vector3Int origin, Vector3Int target)
        {
            if (!graph.nodes.ContainsKey(origin))
            {
                Debug.LogErrorFormat("Origin position {0} not present in Playable game grid from {1}", origin, name);
                GameObject go = new GameObject("ERROR POSITION " + target);
                go.transform.position = collisionMap.GetCellCenterWorld(target);
                return new Vector3Int[0];
            }

            if (!graph.nodes.ContainsKey(target))
            {
                Debug.LogErrorFormat("Target position {0} not present in Playable game grid from {1}", target, name);
                GameObject go = new GameObject("ERROR POSITION " + target);
                go.transform.position = collisionMap.GetCellCenterWorld(target);
                return new Vector3Int[0];
            }

            var frontier = new PriorityQueue<Node>();
            var visited = new List<Node>();

            Node startNode = graph.nodes[origin];
            Node endNode = graph.nodes[target];

            frontier.Add(startNode, 0);
            var source = new Dictionary<Node, Node>();
            var accCost = new Dictionary<Node, int>();

            source[startNode] = null;
            accCost[startNode] = 0;

            while (!frontier.IsEmpty)
            {
                Node current = frontier.Dequeue();

                if (current == endNode)
                    break;

                foreach (var next in current.neighbours)
                {
                    int newCost = accCost[current] + 1;
                    if (!accCost.ContainsKey(next) || newCost < accCost[next])
                    {
                        accCost[next] = newCost;
                        int priority = newCost + ManhattanDistance(endNode.pos, next.pos);
                        frontier.Add(next, priority);
                        source[next] = current;
                    }
                }
            }

            var path = new List<Vector3Int>();
            Node curr = endNode;
            while (source[curr] != null)
            {
                path.Add(curr.pos);
                curr = source[curr];
            }
            path.Reverse();
            return path.ToArray();
        }

        private int ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        // -------------------------- //
        // Helpers
        // -------------------------- //

        public Vector3Int GetNearestTileInBounds(Vector3Int origin, Vector3Int offset)
        {
            Vector3Int nearest = origin;
            int max = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y));
            Vector3Int dir = offset;
            dir.Clamp(Vector3Int.one * -1, Vector3Int.one);
            for (int i = max; i >= 0; i--)
            {
                var v = dir * i;
                if (!collisionMap.HasTile(origin + v))
                {
                    nearest = origin + v;
                    break;
                }
            }
            return nearest;
        }

        public int GetTileDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}
