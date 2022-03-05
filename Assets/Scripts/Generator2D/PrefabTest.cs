using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTest : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject arc;
    [SerializeField] private GameObject floor;

    private void Start()
    {
        Instantiate(wall, Vector3.zero, Quaternion.identity);
        Instantiate(wall, Vector3.zero, Quaternion.identity).transform.Rotate(Vector3.up, 90);
        //Instantiate(floor, Vector3.zero, Quaternion.identity);
        Instantiate(arc, new Vector3(0,0,0), Quaternion.identity).transform.Rotate(Vector3.up, 90);
    }
}
