using System;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public CharacterController controller;
    public float baseSpeed = 3f;
    public float runSpeed = 5f;
    public float sprintSpeed = 7f;
    public float angularSpeed = 180f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    protected float _speedBoost;
    protected Vector3 _velocity;
    protected Vector3 _movement;

    public event Action JumpStart;
    


    private void Awake()
    {
        _velocity = Vector3.zero;
    }
    
    private void OnJump()
    {
        _velocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }
    
    protected void Jump()
    {
        JumpStart?.Invoke();
    }
    
    protected void Move(Vector3 velocity)
    {
        Quaternion camRotation = Quaternion.Euler(
            0f, 
            MouseCameraLook.rig.transform.rotation.eulerAngles.y,
            0f
        );
        velocity = camRotation * velocity;
        
        Vector3 horizontal = new Vector3(velocity.x, 0, velocity.z);
        Vector3 vertical = new Vector3(0, velocity.y, 0);
        
        float speed = horizontal.magnitude;
        if (speed > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(horizontal),
                Time.deltaTime * angularSpeed);
        }
        
        controller.Move( Time.deltaTime * velocity);
    }
}