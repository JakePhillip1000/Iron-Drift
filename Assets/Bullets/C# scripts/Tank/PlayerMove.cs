using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float walk = 5f;
    public float run = 10f;
    public float jumpForce = 10f; 
    public float customGravity = 2.5f; // custom gravity make player falls faster
    public float customGravityUp = 2f; // Faster falling, player can hold space to jump higher
    public Transform cameraTransform; 
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // enable the rigidody componenets
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Check if the player is grounded with raycasting
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.5f);

        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        // Set speed, walk or run using shorthand if-else notation C#
        float speed = (Input.GetKey(KeyCode.LeftShift)) ? run : walk;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Player is jumping");
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
        else if (!isGrounded)
        {
            Debug.Log("Player is not grounded");
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (customGravity - 1) * Time.deltaTime; // custom gravity
        } 
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) // player can hold space to jump higher
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (customGravityUp - 1) * Time.deltaTime; // when jumping up
        }

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDirection.magnitude >= 0.1f) // ser the player direction of movement
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            // Set the player movement
            Vector3 desiredMove = cameraForward * moveDirection.z + cameraRight * moveDirection.x;
            rb.velocity = new Vector3(desiredMove.x * speed, rb.velocity.y, desiredMove.z * speed);
        }
    }
}