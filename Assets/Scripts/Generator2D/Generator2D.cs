using System;
using System.Collections;
using System.Collections.Generic;
using Graphs;
using UnityEngine;
using Random = System.Random;

namespace Generator2d
{

    public class Generator2D : MonoBehaviour
    {

        //Size of field to spawn rooms
        [SerializeField] private Vector2Int size;
        //count of rooms will be from x to y
        [SerializeField] private Vector2Int roomCountRange;
        //minimal size of rooms will be from x to y
        [SerializeField] private Vector2Int roomMinSize;
        //maximal size of rooms will be from x to y
        [SerializeField] private Vector2Int roomMaxSize;
        [SerializeField] private Material roomMat;
        [SerializeField] private Material pathMat;
        [SerializeField] private GameObject cubePrefab;
        
        //grin shows state of cells like room or empty
        private Grid2D<CellState> grid;
        //class for Delaunay triangulation
        private Delaunay2D delaunay;
        //created rooms
        private List<Room> rooms;
        //count of rooms
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
            Triangulate();
            
        }


        #region Room Placing

        

        
        private void PlaceRooms()
        {
            //trying until all rooms are created but not more than 100 bonus tries
            for (int i = 0, j = 0; i < roomCount && j < roomCount + 100; j++)
            {
                //generate room position
                Vector2Int position = new Vector2Int
                    (
                        random.Next(0, size.x),
                        random.Next(0, size.y)
                    );

                //generate room size
                Vector2Int roomSize = new Vector2Int
                    (
                        random.Next(roomMinSize.x, roomMaxSize.x),
                        random.Next(roomMinSize.y, roomMaxSize.y)
                    );
                //should the room be created
                bool toAdd = true;

                //buffer will provide distance between rooms
                Room newRoom = new Room(position, roomSize);
                Room buffer = new Room(position - Vector2Int.one, roomSize + Vector2Int.one * 2);

                //check if we can put room here
                foreach (var room in rooms)
                {
                    if (Room.Intersect(room, buffer))
                    {
                        toAdd = false;
                        break;
                    }
                }

                //check if this room in generator bounds
                if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax > size.x ||
                    newRoom.bounds.yMin < 0 || newRoom.bounds.xMax > size.y)
                {
                    toAdd = false;
                }

                //add created room to list and place it to scene
                //mark cells as rooms
                if (toAdd)
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

        
        #endregion

        private void Triangulate()
        {
            List<Vertex> vertices = new List<Vertex>();

            foreach (var room in rooms)
            {
                vertices.Add(new Vertex<Room>(room.bounds.center, room));
                
            }

            delaunay = Delaunay2D.Triangulate(vertices);
        }
        
        private void PlaceCube(Vector2 pos, Vector2Int size, Material material)
        {
            GameObject go = Instantiate(cubePrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3(size.x, 2, size.y);
            go.GetComponent<MeshRenderer>().material = material;
        }

    }

}
