using System.Collections.Generic;
using UnityEngine;

namespace Generator2D
{

    public class DungeonBuilder2D : MonoBehaviour
    {
        [SerializeField] private GenerationSettings2D settings;
        [SerializeField] private TilesPrefabs prefs;

        [SerializeField] private Transform tilesParent;

        [SerializeField] private bool TestMode;
        
        private Generator2D generator;
        private Grid2D<CellState> grid;
        private Grid2D<DungeonTile2D> tiles;
        private List<Room> rooms;
        void Start()
        {
            generator = new Generator2D(settings);
            generator.Generate();
            grid = generator.Grid;
            tiles = generator.Tiles;
            rooms = generator.Rooms;
            if (!TestMode)
            {
                BuildDungeon();
            }
            else
            {
                TestBuild();
            }
        }

        private void TestBuild()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            foreach (var room in rooms)
            {
                GameObject obj = Instantiate(cube, room.bounds.center,
                    Quaternion.identity);
                obj.transform.localScale = room.bounds.size;
                obj.GetComponent<MeshRenderer>().material.color = Color.red;
            }

            foreach (var v in generator.Hallways)
            {
                Instantiate(cube, new Vector3(v.x+.5f, .5f, v.y+.5f), Quaternion.identity);
            }
            Destroy(cube);
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
            if (grid[x, y] != CellState.Empty)
            {
                Instantiate(prefs.Floor, new Vector3(x, 0, y) * prefs.sizeInUnits, Quaternion.identity);
            }
        }
    }
}
