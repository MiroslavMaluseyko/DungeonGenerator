using System;
using System.Collections.Generic;

namespace Graphs
{
    public class Prim
    {

        public static List<Edge> BuildMST(List<Edge> edges)
        {
            HashSet<Vertex> openSet = new HashSet<Vertex>();
            HashSet<Vertex> closeSet = new HashSet<Vertex>();

            foreach (var edge in edges)
            {
                openSet.Add(edge.U);
                openSet.Add(edge.V);
            }

            closeSet.Add(edges[0].U);

            List<Edge> result = new List<Edge>();

            while (openSet.Count > 0)
            {
                bool chosen = false;
                Edge chosenEdge = null;
                float minWeight = float.PositiveInfinity;

                foreach (var edge in edges)
                {
                    int closedVertices = 0;
                    if (!closeSet.Contains(edge.U)) closedVertices++;
                    if (!closeSet.Contains(edge.V)) closedVertices++;
                    if (closedVertices != 1) continue;

                    if (edge.W < minWeight)
                    {
                        chosenEdge = edge;
                        chosen = true;
                        minWeight = edge.W;
                    }
                }

                if (!chosen) break;
                
                result.Add(chosenEdge);
                openSet.Remove(chosenEdge.V);
                openSet.Remove(chosenEdge.U);
                closeSet.Add(chosenEdge.V);
                closeSet.Add(chosenEdge.U);
            }
            
            
            return result;
        }
        
    }
}