using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    // horizontal rotation speed
    public float horizontalSpeed = 1f;
    // vertical rotation speed
    public float verticalSpeed = 1f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Camera cam;
    private Rigidbody rb;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Debug.Log($"Angle={new Vector3(xRotation, yRotation, 0.0f)}");
        //rb.AddRelativeTorque(new Vector3(xRotation, 0.0f, 0.0f));
        //rb.AddTorque(new Vector3(xRotation, 0.0f, 0.0f));
        //transform.eulerAngles = new Vector3(xRotation, 0.0f, 0.0f);
        //cam.transform.eulerAngles = new Vector3(xRotation, 0.0f, 0.0f);
    }
}
