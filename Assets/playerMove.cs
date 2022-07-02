using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Include the namespace required to use Unity UI
using UnityEngine.UI;

public class playerMove : MonoBehaviour
{
    CharacterController characterController;
    public float MovementSpeed = 2;
    public float Gravity = 9.8f;
    private float velocity = 0;

    public Text countText;
    public Text winText;

    // Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
    private Rigidbody rb;
    private int count;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Assign the Rigidbody component to our private rb variable
        rb = GetComponent<Rigidbody>();

        // Set the count to zero 
        count = 0;

        // Run the SetCountText function to update the UI (see below)
        SetCountText();

        // Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
        winText.text = "";
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime;

        //        _transform.localPosition += _transform.right * h;
        //        _transform.localPosition += _transform.forward * v;

        Vector3 RIGHT = transform.TransformDirection(Vector3.right);
        Vector3 FORWARD = transform.TransformDirection(Vector3.forward);
        int layerMask = 8;
        LayerMask mask = LayerMask.GetMask("Wall");
        transform.localPosition += RIGHT * h;
        transform.localPosition += FORWARD * v;
        Vector3 direction = (RIGHT * h + FORWARD * v) * Time.deltaTime;
        direction = new Vector3(direction.x, 0f, direction.z);
        Vector3 newPosition = transform.localPosition + direction;
        //newPosition.y = 0.65f;
        float distance = direction.magnitude + 0.8f;
        //Debug.Log($"oldPos={transform.localPosition}newPos={newPosition}, distance={distance}, vector={RIGHT * h + FORWARD * v}");
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.localPosition, direction, out hit, distance))
        {
            //transform.position = new Vector3(transform.position.x + force.x * speed * Time.deltaTime, heightY, transform.position.z + force.z * speed * Time.deltaTime);
            //transform.position = transform.TransformDirection(force) * hit.distance;
            Debug.Log($"Player Hit {hit.distance}");
            Debug.Log($"oldPos={transform.localPosition}newPos={newPosition}, distance={distance}, vector={direction}");
        }
        else
        {
            transform.localPosition = newPosition;
        }

        // Set some local float variables equal to the value of our Horizontal and Vertical Inputs
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
        // multiplying it by 'speed' - our public player speed that appears in the inspector
        rb.AddRelativeForce(movement * MovementSpeed * Time.deltaTime);
    }

    // When this game object intersects a collider with 'is trigger' checked, 
    // store a reference to that collider in a variable named 'other'..
    void OnTriggerEnter(Collider other)
    {
        // ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
        if (other.gameObject.CompareTag("Pick Up"))
        {
            // Make the other game object (the pick up) inactive, to make it disappear
            other.gameObject.SetActive(false);

            // Add one to the score variable 'count'
            count = count + 1;

            // Run the 'SetCountText()' function (see below)
            SetCountText();
        }
    }

    // Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
    void SetCountText()
    {
        // Update the text field of our 'countText' variable
        countText.text = "Count: " + count.ToString();

        // Check if our 'count' is equal to or exceeded 12
        if (count >= 12)
        {
            // Set the text value of our 'winText'
            winText.text = "You Win!";
        }
    }

}
