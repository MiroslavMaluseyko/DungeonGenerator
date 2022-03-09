using System;
using System.Collections.Generic;
using UnityEditor;
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
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            
            generator = new Generator3D(settings);
            generator.Generate();
            grid = generator.Grid;
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

        private void BuildDungeon()
        {
            foreach (var room in rooms)
            {
                for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
                {
                    for (int z = room.bounds.zMin; z < room.bounds.zMax; z++)
                    {
                        Vector3 p = new Vector3(
                            x * prefs.sizeInUnits.x, 
                            room.bounds.yMin * prefs.sizeInUnits.y, 
                            z * prefs.sizeInUnits.z);
                        Instantiate(prefs.Floor, p, Quaternion.identity, tilesParent);
                    }
                }
            }

            foreach (var st in generator.Stairs)
            {
                Vector3 p1 = new Vector3(
                    st.Item1.x * prefs.sizeInUnits.x, 
                    st.Item1.y * prefs.sizeInUnits.y, 
                    st.Item1.z * prefs.sizeInUnits.z);
                Vector3 p2 = new Vector3(
                    (st.Item1.x + st.Item2.x) * prefs.sizeInUnits.x, 
                    (st.Item1.y + st.Item2.y) * prefs.sizeInUnits.y, 
                    (st.Item1.z + st.Item2.z) * prefs.sizeInUnits.z);
                Instantiate(prefs.LowStairs, p1, Quaternion.LookRotation(-st.Item2) ,tilesParent);
                Instantiate(prefs.HighStairs, p2, Quaternion.LookRotation(-st.Item2),tilesParent);
            }

            foreach (var t in generator.Hallways)
            {
                //if()
            }
            
            for (int x = 0; x < grid.Size.x; x++)
            {
                for (int y = 0; y < grid.Size.y; y++)
                {
                    for (int z = 0; z < grid.Size.z; z++)
                    {
                        if (grid[x, y, z] == CellState.Path)
                        {
                            Vector3 p = new Vector3(
                                x * prefs.sizeInUnits.x, 
                                y * prefs.sizeInUnits.y, 
                                z * prefs.sizeInUnits.z);
                            Instantiate(prefs.Floor, p, Quaternion.identity, tilesParent);
                        }
                    }
                }
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
                            GameObject obj = Instantiate(cube, new Vector3(x,y,z) + Vector3.one*.5f,Quaternion.identity, tilesParent);
                            obj.GetComponent<MeshRenderer>().material.color = Color.green;
                        }

                        if (grid[x, y, z] == CellState.StairsStart ||
                            grid[x, y, z] == CellState.StairsEnd)
                        {
                            GameObject obj = Instantiate(cube, new Vector3(x,y,z) + Vector3.one*.5f,Quaternion.identity, tilesParent);
                            obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                        }
                        if (grid[x, y, z] == CellState.Path)
                        {
                            GameObject obj = Instantiate(cube, new Vector3(x,y,z) + Vector3.one*.5f,Quaternion.identity, tilesParent);
                            //obj.GetComponent<MeshRenderer>().material.color = Color.green;
                        }
                    }
                }
            }
            Destroy(cube);
        }

    }
}