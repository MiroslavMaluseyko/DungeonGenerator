using UnityEngine;

namespace Graphs
{
    public class Vertex
    {
        public Vector3 Position { get; set; }

        public Vertex()
        {
            
        }

        public Vertex(Vector3 pos)
        {
            Position = pos;
        }
        
        static bool AlmostEqual(float x, float y) {
            return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2
                   || Mathf.Abs(x - y) < float.MinValue;
        }

        public static bool AlmostEqual(Vertex a, Vertex b) {
            return AlmostEqual(a.Position.x, b.Position.x) && AlmostEqual(a.Position.y, b.Position.y);
        }


        public static bool operator ==(Vertex a, Vertex b)
        {
            return Vector3.Distance(a.Position, b.Position) < 0.01f;
        }
        public static bool operator !=(Vertex a, Vertex b)
        {
            return !(a == b);
        }
    }
    public class Vertex<T> : Vertex
    {
        public T Item { get; set; }

        public Vertex()
        {
            
        }

        public Vertex(Vector3 pos, T item):base(pos)
        {
            Item = item;
        }
        
        public static bool operator ==(Vertex<T> a, Vertex<T> b)
        {
            return Vector3.Distance(a.Position, b.Position) < 0.01f && a.Item.Equals(b.Item);
        }
        
        public static bool operator !=(Vertex<T> a, Vertex<T> b)
        {
            return !(a == b);
        }
        
    }
}