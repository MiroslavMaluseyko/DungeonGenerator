using System;
using Graphs;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Node = Generator3D.PathFinder3D.Node;
using Object = UnityEngine.Object;

namespace Generator3D
{
    public class Generator3D
    {
        private GenerationSettings3D settings;
        private List<Edge> selectedEdges;
        public Grid3D<CellState> Grid { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<Tuple<Vector3Int, Vector3Int>> Hallways { get; private set; }
        public List<Tuple<Vector3Int, Vector3Int>> Stairs { get; private set; }

        public Random Random { get; private set; }

        private Delaunay3D delaunay;
        public Generator3D(GenerationSettings3D settings)
        {
            this.settings = ScriptableObject.Instantiate(settings);
            Random = new Random(settings.seed.GetHashCode());
            Grid = new Grid3D<CellState>(settings.size);
            Rooms = new List<Room>();
            Stairs = new List<Tuple<Vector3Int, Vector3Int>>();
            selectedEdges = new List<Edge>();
            Hallways = new List<Tuple<Vector3Int, Vector3Int>>();
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
                Vector3Int position = new Vector3Int
                    (
                        Random.Next(0, settings.size.x),
                        Random.Next(0, settings.size.y),
                        Random.Next(0, settings.size.z)
                    );

                //generate room size
                Vector3Int roomSize = new Vector3Int
                    (
                        Random.Next(settings.roomMinSize.x, settings.roomMaxSize.x + 1),
                        Random.Next(settings.roomMinSize.y, settings.roomMaxSize.y + 1),
                        Random.Next(settings.roomMinSize.z, settings.roomMaxSize.z + 1)
                    );
                //should the room be created
                bool toAdd = true;

                //buffer will provide distance between rooms
                Room newRoom = new Room(position, roomSize);
                Room buffer = new Room(position - new Vector3Int(1, 0, 1), roomSize + new Vector3Int(1,0,1) * 2);

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
                    newRoom.bounds.yMin < 0 || newRoom.bounds.yMax > settings.size.y ||
                    newRoom.bounds.zMin < 0 || newRoom.bounds.zMax > settings.size.z)
                {
                    toAdd = false;
                }

                //add created room to list and place it to scene
                //mark cells as rooms
                if (toAdd)
                {
                    i++;
                    Rooms.Add(newRoom);
                    foreach (var v in newRoom.bounds.allPositionsWithin)
                    {
                        Grid[v] = CellState.Room;
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

            delaunay = Delaunay3D.Triangulate(vertices);
            
            
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

        private void FindPaths()
        {
            PathFinder3D pathFinder = new PathFinder3D(settings.size);
            foreach (var edge in selectedEdges)
            {
                Vector3Int start = new Vector3Int((int)edge.U.Position.x,(int)edge.U.Position.y,(int)edge.U.Position.z) ; 
                Vector3Int end =   new Vector3Int((int)edge.V.Position.x,(int)edge.V.Position.y,(int)edge.V.Position.z) ; 
                var list = pathFinder.FindPath(start, end, Heuristic, Neighbours);
                foreach (var pair in list)
                {
                    
                    Vector3Int p1 = pair.Item1.Position;
                    Vector3Int p2 = pair.Item2.Position;
                    if (p1.y != p2.y)
                    {
                        if (p1.y > p2.y)
                        {
                            (p1, p2) = (p2, p1);
                        }
                        Vector3Int horDir = (p2 - p1);
                        horDir.Clamp(new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1));
                        Vector3Int verDir = new Vector3Int(0, p2.y - p1.y, 0);
                        Grid[p1 + horDir           ] = CellState.StairsStart;
                        Grid[p1 + horDir*2         ] = CellState.StairsBot;
                        Grid[p1 + horDir   + verDir] = CellState.Space;
                        Grid[p1 + horDir*2 + verDir] = CellState.StairsEnd;
                        Stairs.Add(new Tuple<Vector3Int, Vector3Int>(p1 + horDir, horDir));
                    }
                    
                    if (Grid[p1] != CellState.Room)
                    {
                        Grid[p1] = CellState.Path;
                    }

                    if (Grid[p2] != CellState.Room)
                    {
                        Grid[p2] = CellState.Path;
                    }
                    Hallways.Add(new Tuple<Vector3Int, Vector3Int>(p1,p2));
                    
                }
            }
        }
        
        private float Heuristic(Node n1, Node n2)
        {
            
            float res = Vector3.Distance(n1.Position,n2.Position);
            if (Grid[n2.Position] == CellState.Room)
            {
                res += settings.roomCost;
            }else
            if (Grid[n2.Position] == CellState.Empty)
            {
                res += settings.emptyCellCost;
            }else
            if (Grid[n2.Position] == CellState.Path)
            {
                res += settings.pathCost;
            }

            if (n1.Position != n2.Position)
            {
                res += settings.stairsCost;
            }

            return res;
        }


        private List<Vector3Int> Neighbours(Node node)
        {
            List<Vector3Int> res = new List<Vector3Int>();

            if (Grid[node.Position] == CellState.StairsBot   ||
                Grid[node.Position] == CellState.StairsStart ||
                Grid[node.Position] == CellState.StairsEnd   ||
                Grid[node.Position] == CellState.Space)
            {
                return res;
            }

            Vector3Int v = Vector3Int.left;
            Vector3Int neigh;
            for (int i = 0; i < 4; i++)
            {
                neigh = node.Position + v;
                if(Grid.InBounds(neigh) &&
                   (Grid[neigh] == CellState.Room ||
                    Grid[neigh] == CellState.Empty ||
                    Grid[neigh] == CellState.Path))
                {
                    res.Add(neigh);
                }
                v.Set(v.z, v.y,-v.x);
            }

            Vector3Int dir = Vector3Int.up;
            for (int j = 0; j < 2; j++)
            {
                v = Vector3Int.left;
                for (int i = 0; i < 4; i++)
                {
                    neigh = node.Position + dir + v + v + v;
                    if (Grid.InBounds(neigh) && CanBeStairs(neigh, node.Position))
                    {
                        if (Grid[neigh] != CellState.Room)
                            res.Add(neigh);
                    }

                    v.Set(v.z, v.y, -v.x);
                }

                dir *= -1;
            }
            

            return res;
        
        }

        private bool CanBeStairs(Vector3Int p1, Vector3Int p2)
        {
            if(!(Grid[p1] == CellState.Room ||
                 Grid[p1] == CellState.Empty ||
                 Grid[p1] == CellState.Path))
            {
                return false;
            }
            
            
            if (p1.y > p2.y)
            {
                (p1, p2) = (p2, p1);
            }
            Vector3Int dir = new Vector3Int(p2.x - p1.x, 0, p2.z - p1.z);
            dir.Clamp(new Vector3Int(-1,0,-1),new Vector3Int(1,0,1));
            
            Vector3Int v = Vector3Int.up;

            if (Grid[p1 + dir        ] == CellState.Empty &&
                Grid[p1 + dir + v    ] == CellState.Empty &&
                Grid[p1 + dir * 2    ] == CellState.Empty &&
                Grid[p1 + dir * 2 + v] == CellState.Empty) 
                return true;
            
            if (Grid[p1 + dir] == CellState.StairsStart &&
                Grid[p1 + dir + v] == CellState.Space &&
                Grid[p1 + dir * 2] == CellState.StairsBot &&
                Grid[p1 + dir * 2 + v] == CellState.StairsEnd)
                return true;
            return false;
            

        }
    }
}