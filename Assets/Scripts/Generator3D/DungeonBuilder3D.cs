using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generator3D
{

    public class DungeonBuilder3D : MonoBehaviour
    {
        [SerializeField] private GenerationSettings3D settings;
        [SerializeField] private TilesPrefabs prefs;

        [SerializeField] private Transform tilesParent;

        [SerializeField] private bool TestMode;
        
        private Generator3D generator;
        private Grid3D<CellState> grid;
        private List<Room> rooms;
        void Start()
        {
            generator = new Generator3D(settings);
            generator.Generate();
            grid = generator.Grid;
            rooms = generator.Rooms;
            if (!TestMode)
            {
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

            for (int x = 0; x < grid.Size.x; x++)
            {
                for (int y = 0; y < grid.Size.y; y++)
                {
                    for (int z = 0; z < grid.Size.z; z++)
                    {
                        if (grid[x, y, z] == CellState.StairsBot ||
                            grid[x, y, z] == CellState.Space)
                        {
                            GameObject obj = Instantiate(cube, new Vector3(x,y,z) + Vector3.one*.5f,Quaternion.identity);
                            obj.GetComponent<MeshRenderer>().material.color = Color.green;
                        }

                        if (grid[x, y, z] == CellState.StairsStart ||
                            grid[x, y, z] == CellState.StairsEnd)
                        {
                            GameObject obj = Instantiate(cube, new Vector3(x,y,z) + Vector3.one*.5f,Quaternion.identity);
                            obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                        }
                        if (grid[x, y, z] == CellState.Path)
                        {
                            GameObject obj = Instantiate(cube, new Vector3(x,y,z) + Vector3.one*.5f,Quaternion.identity);
                            //obj.GetComponent<MeshRenderer>().material.color = Color.green;
                        }
                    }
                }
            }
            Destroy(cube);
        }

    }
}