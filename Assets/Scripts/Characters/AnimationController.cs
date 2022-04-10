using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]private CharacterController _controller;
    
    private readonly int _speedHorizontal = Animator.StringToHash("SpeedHor");
    private readonly int _speedVertical = Animator.StringToHash("SpeedVer");
    private readonly int _jump = Animator.StringToHash("Jump");
    private readonly int _grounded = Animator.StringToHash("Grounded");


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _controller.GetComponent<PlayerMovement>().JumpStart += OnJumpStart;
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateGround();
    }

    private void UpdateSpeed()
    {
        Vector3 velocity = _controller.velocity;
        Vector3 horizontal = new Vector3
            (
                velocity.x,
                0f,
                velocity.z
            );
        
        _animator.SetFloat(_speedHorizontal, horizontal.magnitude);
        _animator.SetFloat(_speedVertical, velocity.y);
    }

    private void UpdateAnimations()
    {
        
    }
    
    private void UpdateGround()
    {
        _animator.SetBool(_grounded, _controller.isGrounded);
    }

    private void OnJumpStart()
    {
        _animator.SetTrigger(_jump);
    }
}
