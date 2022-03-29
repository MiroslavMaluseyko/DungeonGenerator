using System;
using System.Collections.Generic;
using System.Xml.Schema;
using Generator2D;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Generator3D
{

    public class DungeonBuilder3D : MonoBehaviour
    {
        [SerializeField] private GenerationSettings3D settings;
        [SerializeField] private TilesPrefabs prefs;

        [SerializeField] private Transform tilesParent;

        [SerializeField] private bool testMode;
        
        public Generator3D Generator { get; private set; }
        public Vector3 TSize { get; private set; }
        private Grid3D<DungeonTile> tiles;
        private List<Room> rooms;

        public event Action OnDungeonBuilded;
        void Start()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            
            Generator = new Generator3D(settings);
            TSize = prefs.sizeInUnits;
            
            Generator.Generate();
            tiles = Generator.Tiles;
            rooms = Generator.Rooms;
            if (!testMode)
            {
                BuildDungeon();
            }
            else
            {
                TestBuild();
            }
            OnDungeonBuilded?.Invoke();
        }

        private void BuildDungeon()
        {
            for (int x = 0; x < tiles.Size.x; x++)
            {

                for (int y = 0; y < tiles.Size.y; y++)
                {

                    for (int z = 0; z < tiles.Size.z; z++)
                    {
                        Vector3 pos = new Vector3(
                            x * TSize.x,
                            y * TSize.y,
                            z * TSize.z);
                        Vector3Int dir = Vector3Int.back;
                        for (int i = 0; i < 4; i++)
                        {
                            if (tiles[x, y, z][dir] == WallType.Wall)
                            {
                                Instantiate(prefs.Wall, pos, Quaternion.Euler(0,i*90,0), tilesParent);
                            }


                            if (i < 2)
                            {
                                if (tiles[x, y, z][dir] == WallType.Door)
                                {
                                    Vector3 nPos = new Vector3(
                                        pos.x + dir.x * .5f * TSize.x,
                                        pos.y,
                                        pos.z + dir.z * .5f * TSize.z);
                                    Instantiate(prefs.WallWithDoor, nPos, Quaternion.Euler(0,i*90,0), tilesParent);
                                }
                            }
                            
                            dir.Set(dir.z, dir.y,-dir.x);
                        }
                    }
                }
                
                
            }
            
            //Stairs
            foreach (var st in Generator.StairsList)
            {

                Vector3 pos = st.Start;
                Vector3 endPos = new Vector3(
                    (st.Start.x + st.Direction.x),
                    (st.Start.y + st.Height   ),
                    (st.Start.z + st.Direction.z));
                while (pos.y < endPos.y)
                {
                    Vector3 p1 = new Vector3(
                        pos.x * TSize.x,
                        pos.y * TSize.y,
                        pos.z * TSize.z);
                    Vector3 p2 = new Vector3(
                        (pos.x + st.Direction.x) * TSize.x,
                        (pos.y) * TSize.y,
                        (pos.z + st.Direction.z) * TSize.z);

                    pos += 2 * st.Direction + 2 * Vector3.up;
                    Instantiate(prefs.LowStairs, p1, Quaternion.LookRotation(-st.Direction), tilesParent);
                    Instantiate(prefs.HighStairs, p2, Quaternion.LookRotation(-st.Direction), tilesParent);
                }
            }

            for (int x = 0; x < tiles.Size.x; x++)
            {
                for (int y = 0; y < tiles.Size.y; y++)
                {
                    for (int z = 0; z < tiles.Size.z; z++)
                    {
                        if (tiles[x,y,z].Floor == FloorType.Floor)
                        {
                            Vector3 p = new Vector3(
                                x * TSize.x, 
                                y * TSize.y, 
                                z * TSize.z);
                            Instantiate(prefs.Floor, p, Quaternion.identity, tilesParent);
                        }

                        if (tiles[x, y, z].Ceiling == CeilingType.Ceiling)
                        {
                            Vector3 p = new Vector3(
                                x * TSize.x, 
                                (y+1) * TSize.y - .1f, 
                                z * TSize.z);
                            Instantiate(prefs.Floor, p, Quaternion.Euler(180,0,0), tilesParent);
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

            Destroy(cube);
        }

    }
}