using System;
using System.Collections.Generic;
using Graphs;
using UnityEngine;
using Random = System.Random;


namespace Generator.Generator3D
{
    public class Generator3D
    {
        private GenerationSettings3D settings;
        private List<Edge> selectedEdges;
        public Grid3D<DungeonTile> Tiles { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<Tuple<Vector3Int, Vector3Int>> Hallways { get; private set; }
        public List<Stairs> StairsList { get; private set; }

        public Random Random { get; private set; }

        private Delaunay3D delaunay;
        public Generator3D(GenerationSettings3D settings)
        {
            this.settings = ScriptableObject.Instantiate(settings);
            Random = new Random(settings.seed.GetHashCode());
            Rooms = new List<Room>();
            StairsList = new List<Stairs>();
            selectedEdges = new List<Edge>();
            Hallways = new List<Tuple<Vector3Int, Vector3Int>>();
            Tiles = new Grid3D<DungeonTile>(settings.size);
            for (int x = 0; x < Tiles.Size.x; x++)
            {
                for (int y = 0; y < Tiles.Size.y; y++)
                {
                    for (int z = 0; z < Tiles.Size.z; z++)
                    {
                        Tiles[x, y, z] = new DungeonTile();
                    }
                }   
            }
        }

        public void Generate()
        {
            PlaceRooms();
            Triangulate();
            CreateHallways();
            FindPaths();
            CreateWalls();
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
                        Tiles[v].State = CellState.Room;
                    }
                }

            }
        }
             
        private void Triangulate()
        {
            List<Vertex> vertices = new List<Vertex>();

            foreach (var room in Rooms)
            {
                Vector3 pos = room.bounds.center;
                //pos.y = room.bounds.yMin;
                pos.y = room.bounds.position.y;
                vertices.Add(new Vertex<Room>(pos, room));
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
                        Tiles[p1 + horDir           ].State = CellState.Stairs;
                        Tiles[p1 + horDir*2         ].State = CellState.Stairs;
                        Tiles[p1 + horDir   + verDir].State = CellState.Space;
                        Tiles[p1 + horDir*2 + verDir].State = CellState.Space;
                        Stairs stairs = new Stairs(p1 + horDir, p2 - horDir);
                        if (!StairsList.Exists(st => st == stairs))
                        {
                            StairsList.Add(stairs); 
                        }
                    }
                    
                    if (Tiles[p1].State != CellState.Room)
                    {
                        Tiles[p1].State = CellState.Path;
                    }

                    if (Tiles[p2].State != CellState.Room)
                    {
                        Tiles[p2].State = CellState.Path;
                    }
                    Hallways.Add(new Tuple<Vector3Int, Vector3Int>(p1,p2));
                    
                }
            }
        }

        private void CreateWalls()
        {
            foreach (var room in Rooms)
            {
                int minX = room.bounds.xMin;
                int minZ = room.bounds.zMin;
                int maxX = room.bounds.xMax;
                int maxZ = room.bounds.zMax;
                int minY = room.bounds.yMin;
                int maxY = room.bounds.yMax;
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, maxZ - 1);
                        Tiles[pos][Vector3Int.forward] = WallType.Wall;
                        if (Tiles.InBounds(pos + Vector3Int.forward))
                            Tiles[pos + Vector3Int.forward][Vector3Int.back] = WallType.Wall;
                        pos.z = minZ;
                        Tiles[pos][Vector3Int.back] = WallType.Wall;
                        if (Tiles.InBounds(pos + Vector3Int.back))
                            Tiles[pos + Vector3Int.back][Vector3Int.forward] = WallType.Wall;
                    }
                }
                for (int z = minZ; z < maxZ; z++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        Vector3Int pos = new Vector3Int(maxX - 1, y, z);
                        Tiles[pos][Vector3Int.right] = WallType.Wall;
                        if (Tiles.InBounds(pos + Vector3Int.right))
                            Tiles[pos + Vector3Int.right][Vector3Int.left] = WallType.Wall;
                        Tiles[pos][Vector3Int.right] = WallType.Wall;
                        pos.x = minX;
                        Tiles[pos][Vector3Int.left] = WallType.Wall;
                        if (Tiles.InBounds(pos + Vector3Int.left))
                            Tiles[pos + Vector3Int.left][Vector3Int.right] = WallType.Wall;
                    }
                }
                
                for (int x = minX; x < maxX; x++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        Tiles[x, minY, z].Floor = FloorType.Floor;
                        if (Tiles.InBounds(new Vector3Int(x, minY - 1, z)))
                        {
                            Tiles[x, minY-1, z].Ceiling = CeilingType.Ceiling;
                        }
                        Tiles[x, maxY-1, z].Ceiling = CeilingType.Ceiling;
                        if (Tiles.InBounds(new Vector3Int(x, maxY, z)))
                        {
                            Tiles[x, maxY, z].Floor = FloorType.Floor;
                        }
                        
                    }
                }
            }

            foreach (var v in Hallways)
            {
                Tiles[v.Item1].Floor = FloorType.Floor;
                if (Tiles.InBounds(v.Item1 + Vector3Int.down))
                {
                    Tiles[v.Item1 + Vector3Int.down].Ceiling = CeilingType.Ceiling;
                }
                Tiles[v.Item2].Floor = FloorType.Floor;
                if (Tiles.InBounds(v.Item2 + Vector3Int.down))
                {
                    Tiles[v.Item2 + Vector3Int.down].Ceiling = CeilingType.Ceiling;
                }
                
                if (Tiles[v.Item1].State == CellState.Room &&
                    Tiles[v.Item2].State != CellState.Room)
                {
                    Vector3Int dir = v.Item2 - v.Item1;
                    dir.y = 0;
                    dir.Clamp(-Vector3Int.one, Vector3Int.one);
                    if (v.Item1.y == v.Item2.y)
                    {
                        Tiles[v.Item1][dir] = WallType.Door;
                        Tiles[v.Item2][-dir] = WallType.Door;
                    }
                }
                if (Tiles[v.Item1].State != CellState.Room &&
                    Tiles[v.Item2].State == CellState.Room)
                {
                    Vector3Int dir = v.Item1 - v.Item2;
                    dir.y = 0;
                    dir.Clamp(-Vector3Int.one, Vector3Int.one);
                    if (v.Item1.y == v.Item2.y)
                    {
                        Tiles[v.Item1][-dir] = WallType.Door;
                        Tiles[v.Item2][dir] = WallType.Door;
                    }
                }
                    
            }

            for (int x = 0; x < Tiles.Size.x; x++)
            {

                for (int y = 0; y < Tiles.Size.y; y++)
                {

                    for (int z = 0; z < Tiles.Size.z; z++)
                    {
                        
                        Vector3Int pos = new Vector3Int(x, y, z);
                        if (Tiles[pos].State == CellState.Empty) continue;
                        Vector3Int dir = Vector3Int.back;
                        for (int i = 0; i < 4; i++)
                        {
                            if (Tiles.InBounds(pos + dir) && Tiles[pos + dir].State == CellState.Empty)
                            {
                                Tiles[pos][dir] = WallType.Wall;
                                Tiles[pos + dir][-dir] = WallType.Wall;
                            }
                            else
                            if(!Tiles.InBounds(pos + dir))
                            {
                                Tiles[pos][dir] = WallType.Wall;
                            }

                            dir.Set(dir.z, dir.y, -dir.x);
                        }
                        dir = Vector3Int.up;
                        if (Tiles.InBounds(pos + dir) && 
                            (Tiles[pos + dir].State == CellState.Empty || 
                             Tiles[pos + dir].State == CellState.Stairs))
                        {
                            Tiles[pos].Ceiling = CeilingType.Ceiling;
                            Tiles[pos + dir].Floor = FloorType.Floor;
                        }
                        else 
                        if (!Tiles.InBounds(pos + dir))
                        {
                            Tiles[pos].Ceiling = CeilingType.Ceiling;
                        }

                    }
                }
            }
        }
        
        
        private float Heuristic(PathFinder3D.Node n1, PathFinder3D.Node n2)
        {
            
            float res = Vector3.Distance(n1.Position,n2.Position);
            if (Tiles[n2.Position].State == CellState.Room)
            {
                res += settings.roomCost;
            }else
            if (Tiles[n2.Position].State == CellState.Empty)
            {
                res += settings.emptyCellCost;
            }else
            if (Tiles[n2.Position].State == CellState.Path)
            {
                res += settings.pathCost;
            }

            if (n1.Position != n2.Position)
            {
                res += settings.stairsCost;
            }

            return res;
        }

        private List<Vector3Int> Neighbours(PathFinder3D.Node node)
        {
            List<Vector3Int> res = new List<Vector3Int>();

            if (Tiles[node.Position].State == CellState.Stairs   ||
                Tiles[node.Position].State == CellState.Space)
            {
                return res;
            }

            Vector3Int v = Vector3Int.left;
            Vector3Int neigh;
            for (int i = 0; i < 4; i++)
            {
                neigh = node.Position + v;
                if(Tiles.InBounds(neigh) &&
                   (Tiles[neigh].State == CellState.Room ||
                   Tiles[neigh].State == CellState.Empty ||
                   Tiles[neigh].State ==CellState.Path))
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
                    if (Tiles.InBounds(neigh) && CanBeStairs(neigh, node.Position))
                    {
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
            
            if (p1.y > p2.y)
            {
                (p1, p2) = (p2, p1);
            }

            if (Tiles[p1].State == CellState.Room || Tiles[p2].State == CellState.Room) return false;


            Vector3Int dir = new Vector3Int(p2.x - p1.x, 0, p2.z - p1.z);
            dir.Clamp(new Vector3Int(-1,0,-1),new Vector3Int(1,0,1));
            
            Stairs stairs = new Stairs(p1 + dir, p2 - dir);

            if (stairs.GetAllPositions().TrueForAll(pos => Tiles[pos].State == CellState.Empty))
            {
                return true;
            }
            
            if (StairsList.Exists(st => st == stairs))
                return true;
            return false;
            

        }
    }
}