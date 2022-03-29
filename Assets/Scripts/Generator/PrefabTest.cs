using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTest : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject arc;
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject hall;
    [SerializeField] private GameObject hallCenter;
    [SerializeField] private GameObject hallWall;
    [SerializeField] private GameObject holeFix1;
    [SerializeField] private GameObject holeFix2;
    [SerializeField] private GameObject pillar;
    [SerializeField] private GameObject stairsUp;
    [SerializeField] private GameObject stairsDown;

    private void Start()
    {
        /*Instantiate(hall, new Vector3(8,0,0), Quaternion.identity).transform.Rotate(Vector3.up, 0f);
        Instantiate(hallWall, new Vector3(12,0,0), Quaternion.identity).transform.Rotate(Vector3.up, 0f);
        Instantiate(hallCenter, new Vector3(12,0,4), Quaternion.identity).transform.Rotate(Vector3.up, 0f);

        Instantiate(floor, Vector3.zero, Quaternion.identity);
        Instantiate(wall, Vector3.zero, Quaternion.identity);
        Instantiate(holeFix2, Vector3.zero, Quaternion.identity);*/
        //Instantiate(wall, Vector3.zero, Quaternion.identity);
        //Instantiate(wall, new Vector3(1,0,-1)*4, Quaternion.identity).transform.Rotate(Vector3.up,90f);
        //Instantiate(pillar, new Vector3(.5f, 0, -.5f) * 4, Quaternion.identity);
        Instantiate(stairsDown, Vector3.zero, Quaternion.identity);
        Instantiate(stairsUp, new Vector3(0,0,-1)*4, Quaternion.identity);
    }
}
