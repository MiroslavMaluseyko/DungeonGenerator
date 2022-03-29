using System;
using Graphs;
using System.Collections.Generic;
using UnityEngine;

namespace Generator2D
{
    public class Delaunay2D
    {
        
        public List<Vertex> Vertices { get; private set; }
        
        public List<Edge> Edges { get; private set; }

        private List<Triangle> Triangles;

        public Delaunay2D()
        {
            Edges = new List<Edge>();
            Triangles = new List<Triangle>();
        }
        public static Delaunay2D Triangulate(List<Vertex> vertices)
        {
            Delaunay2D delaunay = new Delaunay2D();
            delaunay.Vertices = new List<Vertex>(vertices);
            delaunay.Triangulate();
            return delaunay;
        }

        private void Triangulate()
        {
            
            //Making super triangle
            float minX = Vertices[0].Position.x;
            float minY = Vertices[0].Position.z;
            float maxX = minX;
            float maxY = minY;

            foreach (var vert in Vertices)
            {
                minX = Math.Min(minX, vert.Position.x);
                minY = Math.Min(minY, vert.Position.z);
                maxX = Math.Max(maxX, vert.Position.x);
                maxY = Math.Max(maxY, vert.Position.z);
            }

            float dx = maxX - minX;
            float dy = maxY - minY;
            float deltaMax = Math.Max(dx, dy) * 2;

            Vertex v1 = new Vertex(new Vector3(minX - 1, Vertices[0].Position.y, minY - 1));
            Vertex v2 = new Vertex(new Vector3(minX - 1, Vertices[0].Position.y, maxY + deltaMax));
            Vertex v3 = new Vertex(new Vector3(maxX + deltaMax, Vertices[0].Position.y, minY - 1));

            Triangles.Add(new Triangle(v1,v2,v3));

            
            foreach (var vertex in Vertices)
            {
                List<Edge> polygon = new List<Edge>();

                //if vertex lies in circum circle of triangle delete it
                //make polygon with edges of deleted triangles
                foreach (var t in Triangles)
                {
                    if (t.CircumCircleContains(vertex))
                    {
                        t.IsBad = true;
                        polygon.Add(new Edge(t.A, t.B));
                        polygon.Add(new Edge(t.B, t.C));
                        polygon.Add(new Edge(t.C, t.A));
                    }
                }

                Triangles.RemoveAll(t => t.IsBad);

                
                //two edges equal means they are inside polygon
                //delete both of them
                for (int i = 0; i < polygon.Count; i++)
                {
                    for (int j = i + 1; j < polygon.Count; j++)
                    {
                        if (polygon[i] == polygon[j])
                        {
                            polygon[i].isBad = true;
                            polygon[j].isBad = true;
                        }
                    }
                }

                polygon.RemoveAll(e => e.isBad);

                //divide polygon to triangles with current vertex
                foreach (var edge in polygon)
                {
                    Triangles.Add(new Triangle(edge.U, edge.V, vertex));
                }
            }

            
            //delete super triangle and triangles lying on his vertixes
            Triangles.RemoveAll(t => t.ContainsVertex(v1) || t.ContainsVertex(v2) || t.ContainsVertex(v3));
            
            //divide triangles to edges
            HashSet<Edge> edgeSet = new HashSet<Edge>();

            foreach (var t in Triangles) {
                Edge ab = new Edge(t.A, t.B);
                Edge bc = new Edge(t.B, t.C);
                Edge ca = new Edge(t.C, t.A);

                if (edgeSet.Add(ab)) {
                    Edges.Add(ab);
                }

                if (edgeSet.Add(bc)) {
                    Edges.Add(bc);
                }

                if (edgeSet.Add(ca)) {
                    Edges.Add(ca);
                }
            }
        }
        
    }
}