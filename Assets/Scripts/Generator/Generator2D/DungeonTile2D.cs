using UnityEngine;

namespace Generator2D
{
    public enum WallType
    {
        None,
        Wall,
        Door
    }
    
    
    
    public class DungeonTile2D
    {
        public Vector2Int Position;
        public WallType[] walls;

        public DungeonTile2D()
        {
            walls = new WallType[4];
        }

        public WallType this[Vector2Int v]
        {
            get
            {
                if (v == Vector2Int.left)
                    return walls[0];
                if (v == Vector2Int.up)
                    return walls[1];
                if (v == Vector2Int.right)
                    return walls[2];
                if (v == Vector2Int.down)
                    return walls[3];
                return WallType.None;
            }
            set
            {
                if (v == Vector2Int.left)
                    walls[0] = value;
                if (v == Vector2Int.up)
                    walls[1] = value;
                if (v == Vector2Int.right)
                    walls[2] = value;
                if (v == Vector2Int.down)
                    walls[3] = value;
            }
        }
        
        public WallType this[int v]
        {
            get => walls[v];
            set => walls[v] = value;
        }

    }

}