using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] float jumpPower;

    Rigidbody playerRigidbody;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void PlayerJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
        }
    }

    private void Jump()
    {
        Vector3 newVelocity = new Vector3(playerRigidbody.velocity.x, playerRigidbody.velocity.y + jumpPower, playerRigidbody.velocity.z);
        playerRigidbody.velocity = newVelocity;
    }
}
