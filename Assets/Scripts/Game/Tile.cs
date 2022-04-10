using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Game
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
    public class Tile : MonoBehaviour
    {
        protected Vector3 Position;
        protected Vector3 Rotation;

        public WallType LeftWall;
        public WallType RightWall;
        public WallType ForwardWall;
        public WallType BackWall;

        public FloorType Floor;
        public CeilingType Ceiling;


        public GameObject TilePrefab;
        protected GameObject TileObject;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Rotate();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            TileObject = Instantiate(TilePrefab, Position, Quaternion.Euler(Rotation));
        }

        public void Rotate()
        {
            Rotation.y = (Rotation.y + 90f) % 360;
            (ForwardWall, RightWall, BackWall, LeftWall) = (LeftWall, ForwardWall, RightWall, BackWall);
        }
        
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