using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Utilities;

namespace Wakaman.AI
{
    /* 
     * About the Pathfinding presented here:
     *   This implementation uses A* pathfinding. According to
     *   the article used to base the Ghosts AI, the original
     *   game uses a much simpler greedy solution. Therefore,
     *   this A* solution should make the game (slightly)
     *   harder.
     *
     */
    public class Pathfinding : MonoBehaviour
    {
        // Graph Definitions
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

        // Runtime
        private Graph graph;

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

            Vector3Int start = MapInfo.UpperBound;
            Vector3Int end = MapInfo.LowerBound;
            int xLength = Mathf.Abs(end.x - start.x);
            int yLength = Mathf.Abs(start.y - end.y);

            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    int x = start.x + i;
                    int y = start.y - j;
                    Vector3Int pos = new Vector3Int(x, y, start.z);
                    bool hasTile = MapInfo.CollisionMap.HasTile(pos);
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
             *   Was used to check if nodes neighbourhood
             *   was being generated properly.
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

        public Vector3Int[] GetPath(Vector3Int origin, Vector3Int target, Vector3Int startDir)
        {
            // Position is blocked to ensure that Ghosts do not 
            // reverse movement direction
            Vector3Int blockPos = origin + (startDir * -1);

            // Debug-only code. Used to check if coordinate exists 
            // in the generated graph
            if (!graph.nodes.ContainsKey(origin))
            {
                Debug.LogErrorFormat("Origin position {0} not present in Playable game grid from {1}", origin, name);
                GameObject go = new GameObject("ERROR POSITION " + target);
                go.transform.position = MapInfo.CollisionMap.GetCellCenterWorld(target);
                return new Vector3Int[0];
            }
            if (!graph.nodes.ContainsKey(target))
            {
                Debug.LogErrorFormat("Target position {0} not present in Playable game grid from {1}", target, name);
                GameObject go = new GameObject("ERROR POSITION " + target);
                go.transform.position = MapInfo.CollisionMap.GetCellCenterWorld(target);
                return new Vector3Int[0];
            }

            // Here goes the A* implementation using Manhattan Distance
            var frontier = new PriorityQueue<Node>();
            var visited = new List<Node>();

            Node startNode = graph.nodes[origin];
            Node endNode = graph.nodes[target];

            frontier.Add(startNode, 0);
            var source = new Dictionary<Node, Node>(); // Used to reconstruct path (breadcrumbs!)
            var accCost = new Dictionary<Node, int>(); // Dijkstra cost accumulation

            source[startNode] = null;
            accCost[startNode] = 0;

            Node current = null;
            while (!frontier.IsEmpty)
            {
                current = frontier.Dequeue();
                if (current == endNode)
                    break;

                foreach (var next in current.neighbours)
                {
                    // Ignore the blocked position
                    if (next.pos == blockPos)
                        continue;

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

            // Path reconstruction
            var path = new List<Vector3Int>();
            if(current != null)
            {
                Node n = current;
                while (source[n] != null)
                {
                    path.Add(n.pos);
                    n = source[n];
                }
            }
            path.Reverse();
            return path.ToArray();
        }

        private int ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        /*
         *
         *   hang loose             _
         *        _        ,-.    / )
         *       ( `.     // /-._/ /
         *        `\ \   /(_/ / / /
         *          ; `-`  (_/ / /
         *          |       (_/ /
         *          \          /
         *           )       /`
         *          /      /`
         *
         */

        // -------------------------- //
        // Helpers
        // -------------------------- //

        public static Vector3Int GetTilePos(Vector3 pos)
        {
            return MapInfo.CollisionMap.WorldToCell(pos);
        }

        public Vector3Int GetNearestTileInBounds(Vector3Int origin, Vector3Int offset)
        {
            Vector3Int nearest = origin;
            int max = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y));
            Vector3Int dir = offset;
            dir.Clamp(Vector3Int.one * -1, Vector3Int.one);
            for (int i = max; i >= 0; i--)
            {
                var v = dir * i;
                if (IsPositionInBounds(origin+v) && !MapInfo.CollisionMap.HasTile(origin + v))
                {
                    nearest = origin + v;
                    break;
                }
            }
            return nearest;
        }

        public bool IsPositionInBounds(Vector3Int pos)
        {
            return pos.x > MapInfo.UpperBound.x && 
                   pos.x < MapInfo.LowerBound.x && 
                   pos.y < MapInfo.UpperBound.y && 
                   pos.y > MapInfo.LowerBound.y;
        }

        public int GetTileDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}
