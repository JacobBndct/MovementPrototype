using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] float jumpPower;
    [SerializeField] float maxDistance;
    [SerializeField] int maxNumberOfJumps;
    private int currentNumberOfJumps;
    private bool OnGround;

    Rigidbody playerRigidbody;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        IsOnGround();

        if (OnGround)
        {
            currentNumberOfJumps = maxNumberOfJumps;
            playerRigidbody.velocity -= new Vector3(playerRigidbody.velocity.x, 0.0f, playerRigidbody.velocity.z) * 0.05f;
        }
    }

    public void PlayerJumpInput(InputAction.CallbackContext context)
    {
        if (context.started && (currentNumberOfJumps > 0))
        {
            currentNumberOfJumps--;
            Jump();
        }
    }

    private void Jump()
    {
        Vector3 newVelocity = new Vector3(playerRigidbody.velocity.x, playerRigidbody.velocity.y + jumpPower, playerRigidbody.velocity.z);
        playerRigidbody.velocity = newVelocity;
    }

    private void IsOnGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            OnGround = hit.distance < maxDistance;
        }
    }
}
