using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

namespace Generator2d
{
    public class Generator2D
    {
        private GenerationSettings2D settings;
        
        //grin shows state of cells like room or empty
        public Grid2D<CellState> Grid { get; private set; }
        public Grid2D<DungeonTile2D> Tiles { get; private set; }
        public List<Vector2Int> Hallways { get; private set; }
        //created rooms
        public List<Room> Rooms { get; private set; }

        //class for Delaunay triangulation
        private Delaunay2D delaunay;
        //edges to creating path
        private List<Edge> selectedEdges;
        public Random Random { get; private set; }

        public Generator2D(GenerationSettings2D settings)
        {
            this.settings = ScriptableObject.CreateInstance<GenerationSettings2D>();
            this.settings.SetFields(settings);
            
            Random = new Random(settings.seed.GetHashCode());
            Grid = new Grid2D<CellState>(settings.size);
            Hallways = new List<Vector2Int>();
            Rooms = new List<Room>();
            Tiles = new Grid2D<DungeonTile2D>(settings.size);
            for (int i = 0; i < Tiles.Size.x; i++)
            {
                for (int j = 0; j < Tiles.Size.y; j++)
                {
                    Tiles[i, j] = new DungeonTile2D();
                }
            }
            
        }
        
        public void Generate()
        {
            PlaceRooms();
            Triangulate();
            CreateHallways();
            FindPaths();
        }

        private void PlaceRooms()
        {
            //trying until all rooms are created but not more than [extraTries] tries
            for (int i = 0, j = 0; i < settings.roomCount && j < settings.roomCount + settings.extraTries; j++)
            {
                //generate room position
                Vector2Int position = new Vector2Int
                    (
                        Random.Next(0, settings.size.x),
                        Random.Next(0, settings.size.y)
                    );

                //generate room size
                Vector2Int roomSize = new Vector2Int
                    (
                        Random.Next(settings.roomMinSize.x, settings.roomMaxSize.x),
                        Random.Next(settings.roomMinSize.y, settings.roomMaxSize.y)
                    );
                //should the room be created
                bool toAdd = true;

                //buffer will provide distance between rooms
                Room newRoom = new Room(position, roomSize);
                Room buffer = new Room(position - Vector2Int.one, roomSize + Vector2Int.one * 2);

                //check if we can put room here
                foreach (var room in Rooms)
                {
                    if (Room.Intersect(room, buffer))
                    {
                        toAdd = false;
                        break;
                    }
                }

                //check if this room in generator bounds
                if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax > settings.size.x ||
                    newRoom.bounds.yMin < 0 || newRoom.bounds.yMax > settings.size.y)
                {
                    toAdd = false;
                }

                //add created room to list and place it to scene
                //mark cells as rooms
                if (toAdd)
                {
                    i++;
                    Rooms.Add(newRoom);

                    foreach (var pos in newRoom.bounds.allPositionsWithin)
                    {
                       Grid[pos] = CellState.Room;
                       if (pos.x == newRoom.bounds.xMin)
                       {
                           Vector2Int v = Vector2Int.left;
                           Tiles[pos][v] = WallType.Wall;
                       }
                       if (pos.x == newRoom.bounds.xMax - 1)
                       {
                           Vector2Int v = Vector2Int.right;
                           Tiles[pos][v] = WallType.Wall;
                       }
                       if (pos.y == newRoom.bounds.yMin)
                       {
                           Vector2Int v = Vector2Int.down;
                           Tiles[pos][v] = WallType.Wall;
                       }
                       if (pos.y == newRoom.bounds.yMax - 1)
                       {
                           Vector2Int v = Vector2Int.up;
                           Tiles[pos][v] = WallType.Wall;
                       }
                    }
                    
                }

            }
        }

        private void Triangulate()
        {
            List<Vertex> vertices = new List<Vertex>();

            foreach (var room in Rooms)
            {
                vertices.Add(new Vertex<Room>(room.bounds.center, room));
                
            }

            delaunay = Delaunay2D.Triangulate(vertices);
        }

        private void CreateHallways()
        {
            selectedEdges = Prim.BuildMST(delaunay.Edges);
            foreach (var edge in delaunay.Edges)
            {
                if (Random.NextDouble() < settings.cycleChance && !selectedEdges.Contains(edge))
                {
                    selectedEdges.Add(edge);
                }
            }
            
        }

        private void  FindPaths()
        {
            PathFinder2D pathFinder = new PathFinder2D(settings.size);

            List<Tuple<PathFinder2D.Node, PathFinder2D.Node>> edges = new List<Tuple<PathFinder2D.Node, PathFinder2D.Node>>();
            foreach (var edge in selectedEdges)
            {
                Vector2Int start = new Vector2Int((int) edge.U.Position.x, (int) edge.U.Position.y); 
                Vector2Int end = new Vector2Int((int) edge.V.Position.x, (int) edge.V.Position.y);
                foreach (var pair in pathFinder.FindPath(start, end, Heuristic))
                {
                    edges.Add(pair);
                    if (Grid[pair.Item1.Position] != CellState.Room)
                    {
                        Grid[pair.Item1.Position] = CellState.Path;
                    }
                }
            }
            foreach (var pair in edges)
                {
                    Vector2Int pos1 = pair.Item1.Position;
                    Vector2Int pos2 = pair.Item2.Position;
                    if (Grid[pos1] != CellState.Room)
                    {

                        if (Grid[pos2] == CellState.Room)
                        {
                            Tiles[pos1][pos2 - pos1] = WallType.Door;
                            Tiles[pos2][pos1 - pos2] = WallType.Door;
                        }
                    }
                    else
                    {

                        if (Grid[pos2] == CellState.Path)
                        {
                            Tiles[pos1][pos2 - pos1] = WallType.Door;
                            Tiles[pos2][pos1 - pos2] = WallType.Door;
                        }
                        
                    }
                }
                
                foreach (var pair in edges)
                {
                    Vector2Int pos = pair.Item1.Position;
                    if (Grid[pos] != CellState.Room)
                    {
                        foreach (var v in new Vector2Int[] {Vector2Int.up, Vector2Int.down,Vector2Int.left,Vector2Int.right})
                        {
                            Vector2Int posN = pos + v;
                            if (Grid.InBounds(posN))
                            {
                                if (Grid[posN] == CellState.Room)
                                {
                                    if (Tiles[pos][v] != WallType.Door) Tiles[pos][v] = WallType.Wall;
                                }

                                if (Grid[posN] == CellState.Empty)
                                {
                                    Tiles[pos][v] = WallType.Wall;
                                }
                            }
                            else
                            {
                                Tiles[pos][v] = WallType.Wall;
                            }
                        }
                    }
                }

        }

        private float Heuristic(PathFinder2D.Node n1, PathFinder2D.Node n2)
        {
            float res = Vector2.Distance(n1.Position,n2.Position);
            if (Grid[n2.Position] == CellState.Room)
            {
                res += 10;
            }else
            if (Grid[n2.Position] == CellState.Empty)
            {
                res += 5;
            }else
            if (Grid[n2.Position] == CellState.Room)
            {
                res += 1;
            }
            

            return res;
        }

    }

}
