using System;
using System.Collections.Generic;
using BlueRaja;
using Graphs;
using UnityEngine;

namespace Generator2d
{
    public class PathFinder2D
    {
        public class Node
        {
            public Vector2Int Position { get; set; }
            public Node Previous { get; set; }
            public float Cost { get; set; }

            public Node(Vector2Int pos)
            {
                Position = pos;
            }
            
        }

        private Grid2D<Node> grid;
        SimplePriorityQueue<Node, float> open = new SimplePriorityQueue<Node, float>();

        public PathFinder2D(Vector2Int size)
        {
            grid = new Grid2D<Node>(size);
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    grid[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        private void ResetNodes()
        {
            var size = grid.Size;
            
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    var node = grid[x, y];
                    node.Previous = null;
                    node.Cost = float.PositiveInfinity;
                }
            }
        }

        private List<Node> Neighbours(Node node)
        {
            List<Node> res = new List<Node>();
            Vector2Int neigh = node.Position + Vector2Int.up;
            if (neigh.x < grid.Size.x && neigh.y < grid.Size.y &&
                neigh.x > 0 && neigh.y > 0)
            {
                res.Add(grid[neigh]);
            }
            neigh = node.Position + Vector2Int.down;
            if (neigh.x < grid.Size.x && neigh.y < grid.Size.y &&
                neigh.x > 0 && neigh.y > 0)
            {
                res.Add(grid[neigh]);
            }
            neigh = node.Position + Vector2Int.left;
            if (neigh.x < grid.Size.x && neigh.y < grid.Size.y &&
                neigh.x > 0 && neigh.y > 0)
            {
                res.Add(grid[neigh]);
            }
            neigh = node.Position + Vector2Int.right;
            if (neigh.x < grid.Size.x && neigh.y < grid.Size.y &&
                neigh.x > 0 && neigh.y > 0)
            {
                res.Add(grid[neigh]);
            }

            return res;
        }
        
        public List<Tuple<Node, Node>> FindPath(Vector2Int start, Vector2Int end, Func<Node, Node, float> heuristic)
        {
            ResetNodes();
            
            
            HashSet<Node> close = new HashSet<Node>();
            var result = new List<Tuple<Node, Node>>();
            open.Clear();
            
            grid[start].Cost = 0;
            open.Enqueue(grid[start], heuristic(grid[start],grid[end]));

            while (open.Count > 0)
            {
                Node curr = open.Dequeue();
                close.Add(curr);
                if (curr == grid[end])
                {
                    break;
                }

                foreach (Node neighbour in Neighbours(curr))
                {
                    if (close.Contains(neighbour)) continue;

                    float tempCost = curr.Cost = heuristic(curr, neighbour);
                    if (!open.Contains(neighbour) || tempCost < neighbour.Cost)
                    {
                        neighbour.Previous = curr;
                        neighbour.Cost = tempCost;
                        open.Enqueue(neighbour, tempCost + heuristic(neighbour, grid[end]));
                        open.UpdatePriority(neighbour, tempCost + heuristic(neighbour, grid[end]));
                    }
                }
                
            }

            Node node = grid[end];
            while (node.Previous != null)
            {
                result.Add(new Tuple<Node, Node>(node, node.Previous));
                node = node.Previous;
            }
            
            
            return result;
        }
    }
}