using System.Collections.Generic;
using Graphs;
using System;
using UnityEngine;

namespace Generator3D
{
    public class Delaunay3D
    {
        private List<Vertex> Vertices;
        private List<Tetrahedron> Tetras;
        
        public List<Edge> Edges { get; private set; }
        
        public static Delaunay3D Triangulate(List<Vertex> vertices)
        {
            Delaunay3D delaunay = new Delaunay3D();
            delaunay.Vertices = new List<Vertex>(vertices);
            delaunay.Edges = new List<Edge>();
            delaunay.Tetras = new List<Tetrahedron>();
            delaunay.Triangulate();
            return delaunay;
        }

        private void Triangulate()
        {
            float minX = Vertices[0].Position.x;
            float minY = Vertices[0].Position.y;
            float minZ = Vertices[0].Position.z;
            float maxX = minX;
            float maxY = minY;
            float maxZ = minZ;

            foreach (var vert in Vertices)
            {
                minX = Math.Min(minX, vert.Position.x);
                minY = Math.Min(minY, vert.Position.y);
                minZ = Math.Min(minZ, vert.Position.z);
                maxX = Math.Max(maxX, vert.Position.x);
                maxY = Math.Max(maxY, vert.Position.y);
                maxZ = Math.Max(maxZ, vert.Position.z);
            }
            
            float dx = maxX - minX;
            float dy = maxY - minY;
            float dz = maxZ - minZ;
            float deltaMax = Mathf.Max(dx, dy, dz) * 2;

            Vertex v1 = new Vertex(new Vector3(minX - 1,  minY - 1, minZ - 1));
            Vertex v2 = new Vertex(new Vector3(minX + deltaMax,  minY - 1, minZ - 1));
            Vertex v3 = new Vertex(new Vector3(minX - 1,  minY + deltaMax, minZ - 1));
            Vertex v4 = new Vertex(new Vector3(minX - 1,  minY - 1, minZ + deltaMax));

            Tetras.Add(new Tetrahedron(v1,v2,v3,v4));

            foreach (var v in Vertices)
            {
                List<Triangle> triangles = new List<Triangle>();

                foreach (var t in Tetras)
                {
                    if (t.CircumSphereContains(v.Position))
                    {
                        t.isBad = true;
                        triangles.Add(new Triangle(t.A, t.B, t.C));
                        triangles.Add(new Triangle(t.A, t.B, t.D));
                        triangles.Add(new Triangle(t.A, t.D, t.C));
                        triangles.Add(new Triangle(t.D, t.B, t.C));
                    }
                }

                for (int i = 0; i < triangles.Count;i++)
                {
                    for (int j = i + 1; j < triangles.Count; j++)
                    {
                        if (triangles[i] == triangles[j])
                        {
                            triangles[i].isBad = true;
                            triangles[j].isBad = true;
                        }
                    }
                }

                Tetras.RemoveAll(t => t.isBad);
                triangles.RemoveAll(t => t.isBad);

                foreach (var t in triangles)
                {
                    Tetras.Add(new Tetrahedron(t.A, t.B, t.C, v));
                }
            }

            Tetras.RemoveAll(t => t.Contains(v1) || t.Contains(v2) || t.Contains(v3) || t.Contains(v4));

            HashSet<Edge> edges = new HashSet<Edge>();

            foreach (var t in Tetras)
            {
                Edge ab = new Edge(t.A, t.B);
                Edge bc = new Edge(t.B, t.C);
                Edge ca = new Edge(t.C, t.A);
                Edge ad = new Edge(t.A, t.D);
                Edge bd = new Edge(t.B, t.D);
                Edge cd = new Edge(t.C, t.D);
                if(edges.Add(ab)) Edges.Add(ab);
                if(edges.Add(bc)) Edges.Add(bc);
                if(edges.Add(ca)) Edges.Add(ca);
                if(edges.Add(ad)) Edges.Add(ad);
                if(edges.Add(bd)) Edges.Add(bd);
                if(edges.Add(cd)) Edges.Add(cd);
            }
        }
    }
}