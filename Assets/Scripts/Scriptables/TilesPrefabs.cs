using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TilesPrefabs", menuName = "Prefabs/Tiles Prefabs")]
public class TilesPrefabs : ScriptableObject
{
    public int sizeInUnits;
    public GameObject Wall;
    public GameObject Door;
    public GameObject Floor;
    public GameObject WallWithDoor;
    public GameObject Pillar;
}
