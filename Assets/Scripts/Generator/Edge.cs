using System;
using UnityEngine;

namespace Graphs
{
    public class Edge
    {
        public Vertex U { get; private set; }
        public Vertex V { get; private set; }
        
        public float W { get; private set; }
        
        public bool isBad { get; set; }

        public Edge(Vertex u, Vertex v)
        {
            U = u;
            V = v;
            W = Vector3.SqrMagnitude(u.Position -v.Position);
        }

        public Edge(Edge edge)
        {
            U = new Vertex(edge.U);
            V = new Vertex(edge.V);
            W = edge.W;
        }

        public static bool operator ==(Edge a, Edge b)
        {
            return ((a.U == b.U && a.V == b.V) ||
                   (a.V == b.U && a.U == b.V)) &&
                   (Math.Abs(a.W - b.W) < float.Epsilon);
        }
        
        public static bool operator !=(Edge a, Edge b)
        {
            return !(a == b);
        }

    }
}