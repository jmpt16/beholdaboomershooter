using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
	public float sens;
	[SerializeField] float moveSpeed;
	float xRot, yRot;

	public Rigidbody rb;

	float horizontalMovement;
	float verticalMovement;

	Vector3 moveDirection;

	[Header("Jumping")]
	public float jumpForce;

	[Header("Ground Detection")]
	[SerializeField] Transform groundCheck;
	[SerializeField] LayerMask groundMask;
	[SerializeField] float groundDistance;
	public bool isGrounded;
	public bool groundHit;

	// Start is called before the first frame update
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		rb= GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		rb.useGravity = !OnSlope();
		float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

		xRot -= mouseY;
		xRot = Mathf.Clamp(xRot, -90f, 90f);

		Camera.main.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
		transform.Rotate(mouseX * Vector3.up);

		horizontalMovement = Input.GetAxisRaw("Horizontal");
		verticalMovement = Input.GetAxisRaw("Vertical");

		moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
		if (Input.GetAxisRaw("Jump")>0 && isGrounded)
		{
			rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
		}
	}
	private bool OnSlope()
	{
		if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit slopeHit, groundDistance, groundMask))
		{
			return slopeHit.normal != Vector3.up;
		}
		return false;
	}
	private void FixedUpdate()
	{
		rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
	}
	private void OnCollisionStay(Collision collision)
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		//Debug.Log(slopeHit.distance);
	}

	private void OnCollisionExit(Collision collision)
	{
		isGrounded = false;
	}
}