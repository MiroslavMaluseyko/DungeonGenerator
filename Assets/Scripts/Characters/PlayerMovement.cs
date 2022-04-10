using UnityEngine;

public class PlayerMovement : ActionController
{

    void Update()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (controller.isGrounded)
        {
            _speedBoost = Input.GetKey(KeyCode.LeftShift) ? runSpeed : baseSpeed;
            _speedBoost = Input.GetKey(KeyCode.LeftAlt) ? sprintSpeed : _speedBoost;

            _movement.x = Input.GetAxis("Horizontal");
            _movement.z = Input.GetAxis("Vertical");
            _movement.Normalize();
            
            _velocity = new Vector3
                (
                    _movement.x * _speedBoost,
                    _velocity.y,
                    _movement.z * _speedBoost
                );
        }
        _velocity.y += gravity * Time.deltaTime;


        Move(_velocity);
    }
}
