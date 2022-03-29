using BlueRaja;
using UnityEngine;
using System.Collections.Generic;
using System;
using Generator2D;

namespace Generator3D
{
    public class PathFinder3D
    {
        public class Node
        {
            public Vector3Int Position;
            public Node Previous;
            public float Cost;
            public HashSet<Node> Prevs;


            public Node(Vector3Int pos)
            {
                Prevs = new HashSet<Node>();
                Position = pos;
            }
        }
        
        
        private Grid3D<Node> grid;
        SimplePriorityQueue<Node, float> open = new SimplePriorityQueue<Node, float>();
        
        public PathFinder3D(Vector3Int size)
        {
            grid = new Grid3D<Node>(size);
            for (int x = 0; x < size.x; x++) 
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        grid[x, y, z] = new Node(new Vector3Int(x, y, z));
                    }
                }
            }
        }
        
        private void ResetNodes()
        {
            var size = grid.Size;
            
            for (int x = 0; x < size.x; x++) 
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        var node = grid[x, y, z];
                        node.Previous = null;
                        node.Cost = float.PositiveInfinity;
                        node.Prevs.Clear();
                    }
                }
            }
        }
        
        private List<Vector3Int> Neighbours(Node node)
        {
            List<Vector3Int> res = new List<Vector3Int>();
            
            Vector3Int v = Vector3Int.left;
            Vector3Int neigh;
            for (int i = 0; i < 4; i++)
            {
                neigh = node.Position + v;
                if (neigh.x < grid.Size.x && neigh.y < grid.Size.y && neigh.z < grid.Size.z && 
                    neigh.x > 0 && neigh.y > 0 && neigh.z > 0)
                {
                    res.Add(neigh);
                }
                v.Set(v.z, v.y,-v.x);
            }
            
            neigh = node.Position + Vector3Int.up;
            if (neigh.x < grid.Size.x && neigh.y < grid.Size.y && neigh.z < grid.Size.z && 
                neigh.x > 0 && neigh.y > 0 && neigh.z > 0)
            {
                res.Add(neigh);
            }
            
            neigh = node.Position + Vector3Int.down;
            if (neigh.x < grid.Size.x && neigh.y < grid.Size.y && neigh.z < grid.Size.z && 
                neigh.x > 0 && neigh.y > 0 && neigh.z > 0)
            {
                res.Add(neigh);
            }

            return res;
        }
        
        public List<Tuple<Node, Node>> FindPath(Vector3Int start, Vector3Int end, Func<Node, Node, float> heuristic, 
            Func<Node, List<Vector3Int>> neighbours = null)
        {
            if (neighbours == null) neighbours = Neighbours;

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

                foreach (Vector3Int pos in neighbours(curr))
                {
                    Node neighbour = grid[pos];
                    if (close.Contains(neighbour) || curr.Prevs.Contains(neighbour)) continue;

                    float tempCost = curr.Cost = heuristic(curr, neighbour);
                    if (!open.Contains(neighbour) || tempCost < neighbour.Cost)
                    {
                        if (neighbour.Position.y != curr.Position.y)
                        {
                            Vector3Int horDir = (neighbour.Position - curr.Position);
                            horDir.Clamp(new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1));
                            Vector3Int verDir = new Vector3Int(0, neighbour.Position.y - curr.Position.y, 0);

                            if (curr.Prevs.Contains(grid[curr.Position + horDir]) ||
                                curr.Prevs.Contains(grid[curr.Position + horDir * 2]) ||
                                curr.Prevs.Contains(grid[curr.Position + horDir + verDir]) ||
                                curr.Prevs.Contains(grid[curr.Position + horDir * 2 + verDir]))
                                continue;
                            
                            neighbour.Prevs.Add(grid[curr.Position + horDir]);
                            neighbour.Prevs.Add(grid[curr.Position + horDir*2]);
                            neighbour.Prevs.Add(grid[curr.Position + horDir + verDir]);
                            neighbour.Prevs.Add(grid[curr.Position + horDir*2 + verDir]);

                        }
                        neighbour.Prevs.Add(curr);
                        neighbour.Prevs.UnionWith(curr.Prevs);
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