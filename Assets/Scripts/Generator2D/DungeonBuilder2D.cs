using System.Collections.Generic;
using UnityEngine;

namespace Generator2d
{

    public class DungeonBuilder2D : MonoBehaviour
    {
        [SerializeField] private GenerationSettings2D settings;
        [SerializeField] private GameObject hallStraight;
        [SerializeField] private GameObject hallCorner;
        [SerializeField] private GameObject passage;
        [SerializeField] private GameObject floor;
        [SerializeField] private GameObject[] walls;
        [SerializeField] private GameObject cornerDecor;
        [SerializeField] private GameObject cornerTDecor;
        [SerializeField] private GameObject borderDecor;
        [SerializeField] private GameObject center;

        [SerializeField] private Transform tilesParent;
        private Generator2D generator;
        private Grid2D<CellState> grid;
        private Grid2D<DungeonTile2D> tiles;
        void Start()
        {
            generator = new Generator2D(settings);
            generator.Generate();
            grid = generator.Grid;
            tiles = generator.Tiles;
            BuildDungeon();
        }

        private void BuildDungeon()
        {
            
            for (int i = 0; i < tiles.Size.x; i++)
            {
                for (int j = 0; j < tiles.Size.y; j++)
                {
                    BuildTile(i,j);
                }
                
            }
        }

        private void BuildTile(int x, int y)
        {
            if (grid[x, y] == CellState.Empty) return;
            BuildDoors(x, y);
            BuildWalls(x, y, out List<Vector2Int> corners);
            BuildFloorAndRoof(x, y, corners);
        }

        private void BuildDoors(int x, int y)
        {
            if (tiles[x, y][Vector2Int.left] == WallType.Door)
            {
                Instantiate(passage, new Vector3(x - .5f, 0, y) * 4, Quaternion.identity, tilesParent).transform
                    .Rotate(Vector3.up, 90);
            }

            if (tiles[x, y][Vector2Int.up] == WallType.Door)
            {
                Instantiate(passage, new Vector3(x, 0, y + .5f) * 4, Quaternion.identity, tilesParent).transform
                    .Rotate(Vector3.up, 180);
            }
        }

        private void BuildWalls(int x, int y, out List<Vector2Int> corners)
        {
            corners = new List<Vector2Int>();
            Vector2Int v = Vector2Int.down;
            for(int i = 0; i < 4;i ++)
            {
                if (tiles[x, y][v] == WallType.Wall)
                {
                    float angle = -Vector2.SignedAngle(Vector2.down, v);
                    GameObject wall = walls[generator.Random.Next(walls.Length - 1)];
                    Instantiate(wall, new Vector3(x, 0, y) * 4, Quaternion.identity, tilesParent).transform.Rotate(Vector3.up, angle);
                }

                v.Set(v.y, -v.x);
            }

            
            v = Vector2Int.down;
            for(int i = 0; i < 4;i ++)
            {
                Vector2Int v1 = new Vector2Int(v.y, -v.x);
                Vector2Int pos = new Vector2Int(x,y);

                if (tiles[pos][v] != WallType.None || tiles[pos][v1] != WallType.None ||
                    (tiles.InBounds(pos + v) && tiles[pos + v][v1] != WallType.None) ||
                    (tiles.InBounds(pos + v1) && tiles[pos + v1][v] != WallType.None))
                {
                    corners.Add(v);
                }
                
                v = v1;
            }
        }

        private void BuildFloorAndRoof(int x, int y, List<Vector2Int> corners)
        {
            switch (corners.Count)
            {
                case 0 when grid[x,y] != CellState.Empty:
                    Instantiate(center, new Vector3(x, 0, y) * 4, Quaternion.identity, tilesParent);
                    break;
                case 4:
                    Instantiate(cornerTDecor, new Vector3(x, 0, y) * 4, Quaternion.identity, tilesParent);
                    break;
                case 3:
                case 1:
                {
                    Vector2Int dir = Vector2Int.zero;
                    corners.ForEach(v => dir += v);
                    Vector2Int v1 = new Vector2Int(dir.y, -dir.x);
                    float angle = -Vector2.SignedAngle(new Vector2(1,-1), dir+v1);
                    Instantiate(cornerDecor, new Vector3(x, 0, y) * 4, Quaternion.identity, tilesParent).transform.Rotate(Vector3.up, angle);
                    break;
                }
                case 2:
                {
                    Vector2Int dir = tiles[x, y][corners[0]] != WallType.None ? corners[0] : corners[1];
                    if (tiles[x, y][dir] != WallType.None)
                    {
                        float angle = -Vector2.SignedAngle(Vector2.down, dir);
                        Instantiate(borderDecor, new Vector3(x, 0, y) * 4, Quaternion.identity, tilesParent).transform
                            .Rotate(Vector3.up, angle);
                    }

                    break;
                }
                default:
                    Debug.LogWarning("There are more than 4 corners..?");
                    break;
            }
        }
    }
}
