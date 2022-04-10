using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
    }
}
