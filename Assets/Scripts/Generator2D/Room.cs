using UnityEngine;


namespace Generator2d
{

    public class Room
    {
        public RectInt bounds;

        public Room(Vector2Int position, Vector2Int size)
        {
            bounds = new RectInt(position, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return a.bounds.Overlaps(b.bounds);
        }

    }

}