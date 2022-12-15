using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Vector3 velocity;

    private float yVec = 0f;
    private float jumpSpeed = 8;
    private Vector3 previous;
    public float speed = 12;
    private int airSpeed = 5;
    private CharacterController controller;
    private Rigidbody rb;
    private Vector3 movementVector;
    // Start is called before the first frame update
    void Start()
    {
        previous = Vector3.zero;
        rb = this.GetComponent<Rigidbody>();
        movementVector = Vector3.zero;
        controller = this.GetComponent<CharacterController>();
    }
    void UpdateVelocity() {
        velocity = ((transform.position - previous)) / Time.deltaTime;
        previous = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVelocity();
        UpdateMovementVector();

        if (controller.isGrounded)
        {
            yVec = -1f;
            if (Input.GetButtonDown("Jump"))
            {
                yVec = jumpSpeed;
            }
        }
        yVec -= 30f * Time.deltaTime;
        movementVector.y = yVec;


        controller.Move(movementVector * Time.deltaTime);

    }

    void UpdateMovementVector()
    {
        
        movementVector = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        if (controller.isGrounded)
        {
            movementVector *= speed;

        }
        else
        {
            movementVector *= airSpeed;
        }
        movementVector = transform.TransformDirection(movementVector);
//        movementVector = new Vector3(x, 0, y);
        Debug.Log(movementVector);
    }

}
