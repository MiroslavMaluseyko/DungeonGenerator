using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCameraLook : MonoBehaviour
{
   [SerializeField] private float mouseSensitivity = 100f;

   public static Transform rig;

   private void Awake()
   {
      rig = transform;
      Cursor.lockState = CursorLockMode.Locked;
   }

   private void Update()
   {
      float mouseY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
      float mouseX = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

      Vector3 euler = transform.rotation.eulerAngles;
      transform.rotation = Quaternion.Euler(euler.x + mouseX, euler.y + mouseY, 0f);
   }
}
