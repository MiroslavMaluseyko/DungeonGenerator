using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Generator2d
{

    public class Generator2D : MonoBehaviour
    {

        [SerializeField] private Vector2Int size;
        [SerializeField] private Vector2Int roomCountRange;
        [SerializeField] private Vector2Int roomMinSize;
        [SerializeField] private Vector2Int roomMaxSize;
        [SerializeField] private Material roomMat;
        [SerializeField] private Material pathMat;
        [SerializeField] private GameObject cubePrefab;
        
        
        private Grid2D<CellState> grid;
        private List<Room> rooms;
        private int roomCount;
        private Random random;
        
        private void Start()
        {
            Generate();
        }

        private void Generate()
        {
            random = new Random(0);
            grid = new Grid2D<CellState>(size);
            rooms = new List<Room>();
            roomCount = random.Next(roomCountRange.x, roomCountRange.y);
            PlaceRooms();
        }

        
        
        private void PlaceRooms()
        {
            for (int i = 0, j = 0; i < roomCount && j < 100; j++)
            {
                Vector2Int position = new Vector2Int
                    (
                        random.Next(0, size.x),
                        random.Next(0, size.y)
                    );

                Vector2Int roomSize = new Vector2Int
                    (
                        random.Next(roomMinSize.x, roomMaxSize.x),
                        random.Next(roomMinSize.y, roomMaxSize.y)
                    );
                bool add = true;

                Room newRoom = new Room(position, roomSize);
                Room buffer = new Room(position - Vector2Int.one, roomSize + Vector2Int.one * 2);

                foreach (var room in rooms)
                {
                    if (Room.Intersect(room, buffer))
                    {
                        add = false;
                        break;
                    }
                }

                if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax > size.x ||
                    newRoom.bounds.yMin < 0 || newRoom.bounds.xMax > size.y)
                {
                    add = false;
                }

                if (add)
                {
                    i++;
                    rooms.Add(newRoom);
                    PlaceRoom(newRoom);

                    foreach (var pos in newRoom.bounds.allPositionsWithin)
                    {
                        grid[pos] = CellState.Room;
                    }
                    
                }
                
                

            }
        }


        private void PlaceRoom(Room room)
        {
            PlaceCube(room.bounds.center, room.bounds.size, roomMat);
        }

        private void PlaceCube(Vector2 pos, Vector2Int size, Material material)
        {
            GameObject go = Instantiate(cubePrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3(size.x, 1, size.y);
            go.GetComponent<MeshRenderer>().material = material;
        }

    }

}
