using UnityEngine;

namespace Generator3D
{
    public enum WallType
    {
        None,
        Wall,
        Door,
        Window
    }

    public enum FloorType
    {
        None,
        Floor
    }

    public enum CeilingType
    {
        None,
        Ceiling
    }

    public class DungeonTile
    {
        public Vector3Int Position;

        public CellState State;
        public WallType LeftWall;
        public WallType RightWall;
        public WallType ForwardWall;
        public WallType BackWall;

        public FloorType Floor;
        public CeilingType Ceiling;

        public WallType this[Vector3Int v]
        {
            get
            {
                if (v == Vector3Int.left)
                    return LeftWall;
                if (v == Vector3Int.forward)
                    return ForwardWall;
                if (v == Vector3Int.right)
                    return RightWall;
                if (v == Vector3Int.back)
                    return BackWall;
                return WallType.None;
            }
            set
            {
                if (v == Vector3Int.left)
                    LeftWall = value;
                if (v == Vector3Int.forward)
                    ForwardWall = value;
                if (v == Vector3Int.right)
                    RightWall = value;
                if (v == Vector3Int.back)
                    BackWall = value;
            }
        }

    }

}
