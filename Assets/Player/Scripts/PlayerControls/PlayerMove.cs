using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Movement Parameters")]
    [SerializeField] float movementSpeed;
    [SerializeField] float acceleration;

    Vector2 playerDirection = Vector2.zero;
    Rigidbody playerRigidbody;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    public void PlayerMovement(InputAction.CallbackContext context)
    {
        playerDirection = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Update the player's velocity according to the recorded input of the user
    /// </summary>
    public void MovePlayer()
    {
        Accelerate(transform.TransformDirection(new Vector3(playerDirection.x, 0.0f, playerDirection.y)), acceleration, movementSpeed);
    }

    public void Accelerate(Vector3 direction, float acceleration, float maxSpeed)
    {
        Vector3 currentVelocity = playerRigidbody.velocity;

        if (direction == Vector3.zero)
            return;

        Vector3 desiredAcceleration = direction.normalized * acceleration * Time.deltaTime;

        if ((currentVelocity + desiredAcceleration).magnitude > maxSpeed)
            return;

        Vector3 newVelocity = currentVelocity + desiredAcceleration;
        playerRigidbody.velocity = newVelocity;
    }
}
