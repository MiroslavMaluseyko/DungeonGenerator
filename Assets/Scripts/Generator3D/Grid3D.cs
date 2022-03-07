using UnityEngine;

namespace Generator3D
{
    public class Grid3D<T>
    {
        private T[] data;

        public Vector3Int Size { get; private set; }

        public Grid3D(Vector3Int size)
        {
            Size = size;
            
            data = new T[size.x * (size.y+1) * size.z + 1];
        }

        public int GetIndex(Vector3Int pos)
        {
            return Size.x * Size.z * (pos.y) + Size.x * pos.z + pos.x;
        }

        public bool InBounds(Vector3Int pos)
        {
            return new BoundsInt(Vector3Int.zero, Size).Contains(pos);
        }

        public T this[int x, int y, int z]
        {
            get { return this[new Vector3Int(x, y, z)]; }
            set { this[new Vector3Int(x, y, z)] = value; }
        }

        public T this[Vector3Int pos]
        {
            get { return data[GetIndex(pos)]; }
            set { data[GetIndex(pos)] = value; }
        }
    }
}