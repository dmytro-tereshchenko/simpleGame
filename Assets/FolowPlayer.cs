using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class FolowPlayer : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float heightY;
    public TextMeshPro viewText;
    private Rigidbody m_Rigidbody;
    private Vector3 force;
    private Vector3 rotate;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //viewText= GetComponent<TextMeshPro>();
        ChangeDirection(null);
    }

    void OnTriggerEnter(Collider otherObj)
    {
        //Debug.Log($"collider {otherObj.gameObject.tag}");
        if (otherObj.gameObject.tag == "Struct")
        {
            ChangeDirection(force);
        }
        else if (otherObj.gameObject.tag == "Player")
        {
            Debug.Log("Game over");
            /*viewText.text = "Game over";
            EditorApplication.isPlaying = false;*/
            //EditorApplication.isPaused = true;
            //Destroy(otherObj.gameObject);
            Application.Quit();
        }
    }

    void OnCollisionEnter(Collision otherObj)
    {
        //Debug.Log("collusion");
        if (otherObj.gameObject.tag == "Struct")
        {
            ChangeDirection(force);
        }
        else if (otherObj.gameObject.tag == "Player")
        {
            Debug.Log("Game over");
            /*viewText.text = "Game over";
            EditorApplication.isPlaying = false;*/
            //EditorApplication.isPaused = true;
            //Destroy(otherObj.gameObject);
            Application.Quit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"force before={force.x},{force.z} {force}");
        //transform.position = new Vector3(transform.position.x, heightY, transform.position.z);
        //ChangeDirection(null);

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8 (pick up items).
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(force), out hit, speed * Time.deltaTime, layerMask))
        {
            //transform.position = new Vector3(transform.position.x + force.x * speed * Time.deltaTime, heightY, transform.position.z + force.z * speed * Time.deltaTime);
            //transform.position = transform.TransformDirection(force) * hit.distance;
            Debug.Log($"Did Hit {hit.distance}");
            ChangeDirection(force);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + force.x * speed * Time.deltaTime, heightY, transform.position.z + force.z * speed * Time.deltaTime);
            //Debug.Log("Did not Hit");
        }

        /*Debug.Log($"force after={force.x},{force.z} {force}");
        if (!Physics.Raycast(transform.position, force * 0.5f))
        {
            Debug.Log($"mover={new Vector3(transform.position.x + force.x * speed * Time.deltaTime, heightY, transform.position.z + force.z * speed * Time.deltaTime)}");
            transform.position = new Vector3(transform.position.x + force.x * speed * Time.deltaTime, heightY, transform.position.z + force.z * speed * Time.deltaTime);
        }*/
        //transform.eulerAngles = rotate;
        //transform.Translate(force, Space.Self);
        //m_Rigidbody.AddForce(force * speed * Time.deltaTime);
    }

    private void ChangeDirection(Vector3? direction) {
        float pX = player.transform.position.x;
        float pZ = player.transform.position.z;
        float eX = transform.position.x;
        float eZ = transform.position.z;
        //Debug.Log($"pl.X={pX},pl.Z={pZ} {name} {eX}:{eZ}");

        rotate = new Vector3(0, 0, 0);
        //Debug.Log(direction);
        if (pZ >= eZ && Mathf.Abs(pX - eX) < Mathf.Abs(pZ - eZ))
        {
            if (direction == null || direction != Vector3.forward)
            {
                force = Vector3.forward;
                rotate = new Vector3(0, 180f, 0);
                //Debug.Log("forward1");
            }
            else if (pX < eX)
            {
                force = Vector3.left;
                rotate = new Vector3(0, 90f, 0);
               //Debug.Log("left2");
            }
            else
            {
                force = Vector3.right;
                rotate = new Vector3(0, -90f, 0);
                //Debug.Log("right2");
            }
        }
        else if (pZ < eZ && Mathf.Abs(pX - eX) < Mathf.Abs(pZ - eZ))
        {
            if (direction == null || direction != Vector3.back)
            {
                force = Vector3.back;
                rotate = new Vector3(0, 0f, 0);
                //Debug.Log("back1");
            }
            else if (pX < eX)
            {
                force = Vector3.left;
                rotate = new Vector3(0, 90f, 0);
                //Debug.Log("left2");
            }
            else
            {
                force = Vector3.right;
                rotate = new Vector3(0, -90f, 0);
               //Debug.Log("right2");
            }
        }
        else if (pX < eX && Mathf.Abs(pX - eX) >= Mathf.Abs(pZ - eZ))
        {
            if (direction == null || direction != Vector3.left)
            {
                force = Vector3.left;
                rotate = new Vector3(0, 90f, 0);
                //Debug.Log("left1");
            }
            else if (pZ >= eZ)
            {
                force = Vector3.forward;
                rotate = new Vector3(0, 180f, 0);
                //Debug.Log("forward2");
            }
            else
            {
                force = Vector3.back;
                rotate = new Vector3(0, 0f, 0);
                Debug.Log("back2");
            }
        }
        else
        {
            if (direction == null || direction != Vector3.right)
            {
                force = Vector3.right;
                rotate = new Vector3(0, -90f, 0);
                //Debug.Log("right1");
            }
            else if (pZ >= eZ)
            {
                force = Vector3.forward;
                rotate = new Vector3(0, 180f, 0);
                //Debug.Log("forward2");
            }
            else
            {
                force = Vector3.back;
                rotate = new Vector3(0, 0f, 0);
                //Debug.Log("back2    ");
            }
        }
        //transform.Rotate(0, rotateY, 0, Space.Self);
        transform.eulerAngles = rotate;
    }
}
