using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
	// store a public reference to the Player game object, so we can refer to it's Transform
	public GameObject player;
	public GameObject camera;
	private Rigidbody rb;
	// horizontal rotation speed
	public float horizontalSpeed = 1f;
	// vertical rotation speed
	public float verticalSpeed = 1f;
	// Store a Vector3 offset from the player (a distance to place the camera from the player at all times)
	private Vector3 offset;
	private float xRotation = 0.0f;
	private float yRotation = 0.0f;

	// At the start of the game..
	void Start()
	{
		// Create an offset by subtracting the Camera's position from the player's position
		offset = camera.transform.position - transform.position;
		rb = GetComponent<Rigidbody>();
	}

	// After the standard 'Update()' loop runs, and just before each frame is rendered..
	void LateUpdate()
	{
		float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed * Time.deltaTime;

		yRotation += mouseX;
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90, 90);

		// Set the position of the Camera (the game object this script is attached to)
		// to the player's position, plus the offset amount
		rb.AddTorque(new Vector3(xRotation, 0.0f, 0.0f));
		//transform.eulerAngles = new Vector3(xRotation, 0.0f, 0.0f);
		camera.transform.eulerAngles = new Vector3(0.0f, yRotation, 0.0f);
		//camera.transform.eulerAngles = new Vector3(xRotation, 0.0f, 0.0f);
		//camera.transform.position = transform.position + offset;
	}
}
