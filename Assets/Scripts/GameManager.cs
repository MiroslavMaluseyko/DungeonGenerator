using System.Collections;
using System.Collections.Generic;
using Generator2D;
using Generator3D;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]private DungeonBuilder3D builder;
    [SerializeField]private GameObject player;

    private void Start()
    {
        builder.OnDungeonBuilded += SpawnPlayer;
    }


    private void SpawnPlayer()
    {
        Vector3 pos = builder.Generator.Rooms[0].bounds.center;
        player.transform.position = new Vector3(
            (pos.x+.5f) * builder.TSize.x, 
            (pos.y+.5f) * builder.TSize.y, 
            (pos.z+.5f) * builder.TSize.z);
    }
}
