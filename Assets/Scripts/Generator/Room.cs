using System.Xml;
using UnityEngine;

namespace Generator
{

    public class Room
    {
        public BoundsInt bounds;

        public Room(Vector2Int position, Vector2Int size)
        {
            bounds = new BoundsInt(new Vector3Int(position.x, 0, position.y), new Vector3Int(size.x, 1, size.y));
        }

        public Room(Vector3Int position, Vector3Int size)
        {
            bounds = new BoundsInt(position, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return (new Bounds(a.bounds.center, a.bounds.size)).Intersects(new Bounds(b.bounds.center, b.bounds.size));
        }

    }
}