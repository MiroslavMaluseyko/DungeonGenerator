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
        public List<Triangle> Triangles { get; private set; }

        Delaunay3D()
        {
            Edges = new List<Edge>();
            Triangles = new List<Triangle>();
            Tetras = new List<Tetrahedron>();
        }

        public static Delaunay3D Triangulate(List<Vertex> vertices)
        {
            Delaunay3D delaunay = new Delaunay3D();
            delaunay.Vertices = new List<Vertex>(vertices);
            delaunay.Edges = new List<Edge>();
            delaunay.Tetras = new List<Tetrahedron>();
            delaunay.Triangulate();
            return delaunay;
        }

        void Triangulate()
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

            Vertex v1 = new Vertex(new Vector3(minX - 1, minY - 1, minZ - 1));
            Vertex v2 = new Vertex(new Vector3(maxX + deltaMax, minY - 1, minZ - 1));
            Vertex v3 = new Vertex(new Vector3(minX - 1, maxY + deltaMax, minZ - 1));
            Vertex v4 = new Vertex(new Vector3(minX - 1, minY - 1, maxZ + deltaMax));

            Tetras.Add(new Tetrahedron(v1, v2, v3, v4));

            foreach (var v in Vertices)
            {
                List<Triangle> triangles = new List<Triangle>();

                foreach (var t in Tetras)
                {
                    if (t.CircumSphereContains(v.Position))
                    {
                        t.IsBad = true;
                        triangles.Add(new Triangle(t.A, t.B, t.C));
                        triangles.Add(new Triangle(t.A, t.B, t.D));
                        triangles.Add(new Triangle(t.A, t.D, t.C));
                        triangles.Add(new Triangle(t.D, t.B, t.C));
                    }
                }

                for (int i = 0; i < triangles.Count; i++)
                {
                    for (int j = i + 1; j < triangles.Count; j++)
                    {
                        if (triangles[i] == triangles[j])
                        {
                            triangles[i].IsBad = true;
                            triangles[j].IsBad = true;
                        }
                    }
                }

                Tetras.RemoveAll(t => t.IsBad);
                triangles.RemoveAll(t => t.IsBad);

                foreach (var t in triangles)
                {
                    Tetras.Add(new Tetrahedron(t.A, t.B, t.C, v));
                }
            }

            List<Triangle> trians = new List<Triangle>();
            foreach (var t in Tetras)
            {
                trians.Add(new Triangle(t.A, t.B, t.C));
                trians.Add(new Triangle(t.D, t.B, t.C));
                trians.Add(new Triangle(t.A, t.D, t.C));
                trians.Add(new Triangle(t.A, t.B, t.D));
            }


            trians.RemoveAll(t =>
                t.ContainsVertex(v1) || t.ContainsVertex(v2) || t.ContainsVertex(v3) || t.ContainsVertex(v4));

            HashSet<Edge> edges = new HashSet<Edge>();

            foreach (var t in trians)
            {
                Edge ab = new Edge(t.A, t.B);
                Edge bc = new Edge(t.B, t.C);
                Edge ca = new Edge(t.C, t.A);
                if (edges.Add(ab)) Edges.Add(ab);
                if (edges.Add(bc)) Edges.Add(bc);
                if (edges.Add(ca)) Edges.Add(ca);
            }

        }
    }
}