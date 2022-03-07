using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Graphs
{
    public class Triangle
    {
        public Vertex A { get; private set; }
        public Vertex B { get; private set; }
        public Vertex C { get; private set; }
        
        public bool isBad { get; set; }

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
        }

        public bool ContainsVertex(Vertex v)
        {
            return v == A || v == B || v == C;
        }

        public bool CircumCircleContains(Vertex v)
        {
            Vector3 a = A.Position;
            Vector3 b = B.Position;
            Vector3 c = C.Position;
            
            
            float xab = a.x - b.x;
            float xbc = b.x - c.x;
            float xca = c.x - a.x;
            float yab = a.z - b.z;
            float ybc = b.z - c.z;
            float yca = c.z - a.z;
            float za = Mathf.Pow(a.x,2f) + Mathf.Pow(a.z,2f);
            float zb = Mathf.Pow(b.x,2f) + Mathf.Pow(b.z,2f);
            float zc = Mathf.Pow(c.x,2f) + Mathf.Pow(c.z,2f);

            float zx = yab * zc + ybc * za + yca * zb;
            float zy = xab * zc + xbc * za + xca * zb;
            float z = xab * yca - yab * xca;

            Vector3 center = new Vector3(-zx/z/2, a.y,zy/z/2);

            float radius = Vector3.SqrMagnitude(a - center);
            float distance = Vector3.SqrMagnitude(v.Position - center);

            
            return distance <= radius;
        }

        public static bool operator ==(Triangle a, Triangle b)
        {
            return
                ((a.A == b.A) || (a.A == b.B) || (a.A == b.C)) &&
                ((a.B == b.A) || (a.B == b.B) || (a.B == b.C)) &&
                ((a.C == b.A) || (a.C == b.B) || (a.C == b.C));
        }
        
        public static bool operator !=(Triangle a, Triangle b) {
            return !(a == b);
        }
    }
}