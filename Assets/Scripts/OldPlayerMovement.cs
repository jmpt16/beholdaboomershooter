using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPlayerMovement : MonoBehaviour
{
    float playerHeight = 2f;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float airMultiplier;
    float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float acceleration;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    bool readyToJump;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance;
    public bool isGrounded;
    public bool groundHit;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection, slopeMoveDirectionOld;

    Rigidbody rb;

    RaycastHit slopeHit;
    RaycastHit fallHeight;

    public GameObject prefab;
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f, groundMask))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void HandleGravity()
    {
        if (groundHit && slopeHit.distance<=0)
        {
            if (Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 1f, groundMask))
            {
                rb.AddForce(-transform.up, ForceMode.Impulse);
                
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("a");
        //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //isGrounded = true;
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded=false;
    }
    private void Update()
    {
        

        MyInput();
        ControlDrag();
        ControlSpeed();
        HandleGravity();

        if (Input.GetKey(jumpKey) && isGrounded && readyToJump)
        {
            readyToJump = false;
            groundHit = false;
            //this SOMEWHAT fixes random jump height
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (readyToJump && !groundHit) {
            groundHit = isGrounded;
            //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        /*if (Input.GetKeyDown(jumpKey) && !isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x/2, 0, rb.velocity.z/2);
            Jump();
        }*/
        if (Input.GetKey(sprintKey) && !isGrounded)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.AddForce(orientation.forward * sprintSpeed*2, ForceMode.Impulse);
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        if (slopeMoveDirection.y.ToString("F2") != slopeMoveDirectionOld.y.ToString("F2"))
        {
            //Debug.Log(slopeMoveDirection.y.ToString("F2") + " vs " + slopeMoveDirectionOld.y.ToString("F2"));
        }
        slopeMoveDirectionOld = slopeMoveDirection;
        prefab.transform.localScale = new Vector3(groundDistance, groundDistance, groundDistance);
        prefab.SetActive(readyToJump);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

    }

    void Jump()
    {
        float slopeboost = rb.velocity.y;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * (jumpForce+ slopeboost), ForceMode.Impulse);
    }

    //this really doesn't need to be a seperate function,
    //I just don't know how to invoke "readyToJump = true;" on it's own
    private void ResetJump()
    {
        readyToJump = true;
    }

    void ControlSpeed()
    {
        if (!Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        rb.useGravity = !OnSlope();
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Force);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Force);
            /*if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Acceleration);*/
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Force);
        }
    }
}