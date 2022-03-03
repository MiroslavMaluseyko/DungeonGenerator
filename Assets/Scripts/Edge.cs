using System;

namespace Graphs
{
    public class Edge
    {
        public Vertex U { get; set; }
        public Vertex V { get; set; }
        
        public bool isBad { get; set; }

        public Edge()
        {
        }

        public Edge(Vertex u, Vertex v)
        {
            U = u;
            V = v;
        }

        public static bool operator ==(Edge a, Edge b)
        {
            return (a.U == b.U && a.V == b.V) ||
                   (a.V == b.U && a.U == b.V);
        }
        
        public static bool operator !=(Edge a, Edge b)
        {
            return !(a == b);
        }
        
        public static bool AlmostEqual(Edge left, Edge right) {
            return Vertex.AlmostEqual(left.U, right.U) && Vertex.AlmostEqual(left.V, right.V)
                   || Vertex.AlmostEqual(left.U, right.V) && Vertex.AlmostEqual(left.V, right.U);
        }
        
    }
}