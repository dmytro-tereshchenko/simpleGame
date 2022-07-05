using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class EnemyFolow : MonoBehaviour
{
    public Transform target;
    public float speed;
    public Vector3 startPosition;
    public float hp;
    public float maxHp;
    public int xp;

    private double attackDelay = 2f;
    private double moveDelay = 4f;
    private double rotateDelay = 2f;
    private DateTime attackTime = DateTime.Now;
    private DateTime moveTime = DateTime.Now;
    private DateTime rotateTime = DateTime.Now;
    private Animator anim;
    private Rigidbody rb;
    private float damage = 10;
    private float dangerDistance = 30;
    private float agrDistance = 20;
    private int patrolDistance = 20;
    private System.Random rand = new System.Random();
    private GameObject movePoint;
    private bool back = false;
    private Image healthBarImage;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        hp = maxHp;
        anim.SetBool("isRunning", true);
        startPosition = gameObject.transform.position;
        /*UnityEngine.Object point = AssetDatabase.LoadAssetAtPath("Assets/point.prefab", typeof(GameObject));*/
        GameObject point = Resources.Load<GameObject>("point") as GameObject;
        movePoint = Instantiate(point) as GameObject;
        movePoint.transform.position = GenerateNewMovePosition();
        transform.LookAt(movePoint.transform);
        healthBarImage = GameObject.Find("HealthBarInner").GetComponent<Image>();
        Transform hpBar = gameObject.transform.Find("CanvasEnemy").transform;
        hpBar.LookAt(target.Find("CameraParent/Main Camera/Camera"));
        hpBar.Rotate(0, 180, 0);
        Transform textHpBar = hpBar.Find("HealthBarEnemy/HealthBarInnerEnemy/HealthTextEnemy");
        textHpBar.GetComponent<Text>().text = hp + " / " + maxHp;
    }

    void OnTriggerStay(Collider otherObj)
    {
        if (otherObj.gameObject.tag == "Player")
        {
            anim.SetBool("isRunning", false);
            if (attackTime < DateTime.Now.AddSeconds(-attackDelay))
            {
                Attack(otherObj.gameObject);
            }
        }
    }

    private void Attack(GameObject targ)
    {
        attackTime = DateTime.Now;
        targ.GetComponent<SC_TPSController>().hp -= damage;
        anim.SetTrigger("attack");
        Debug.Log("Player hp = " + targ.GetComponent<SC_TPSController>().hp + ", distance = " + Vector3.Distance(startPosition, transform.position));
        healthBarImage.fillAmount = Mathf.Clamp(targ.GetComponent<SC_TPSController>().hp / targ.GetComponent<SC_TPSController>().maxHp, 0, 1f);
        Transform textHpBar = healthBarImage.transform.Find("HealthText");
        textHpBar.GetComponent<Text>().text = targ.GetComponent<SC_TPSController>().hp + " / " + targ.GetComponent<SC_TPSController>().maxHp;
        if (targ.GetComponent<SC_TPSController>().hp <= 0)
        {
            Debug.Log("Game over");
            /*EditorApplication.isPlaying = false;*/
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            //Application.Quit();
        }
    }

    void OnTriggerExit(Collider otherObj)
    {
        if (otherObj.gameObject.tag == "Player")
        {
            anim.SetBool("isRunning", true);
        }
    }

    void OnCollisionStay(Collision otherObj)
    {
        if (otherObj.gameObject.tag == "Player")
        {
            anim.SetBool("isRunning", false);
            if (attackTime < DateTime.Now.AddSeconds(-attackDelay))
            {
                Attack(otherObj.gameObject);
            }
        }
    }

    void OnCollisionExit(Collision otherObj)
    {
        if (otherObj.gameObject.tag == "Player")
        {
            anim.SetBool("isRunning", true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Transform hpBar = gameObject.transform.Find("CanvasEnemy").transform;
        hpBar.LookAt(target.Find("CameraParent/Main Camera/Camera"));
        hpBar.Rotate(0, 180, 0);
        if (Vector3.Distance(startPosition, transform.position) > dangerDistance)
        {
            if (!back)
            {
                movePoint.transform.position = startPosition;
                transform.LookAt(movePoint.transform);
                back = true;
            }
            if (transform.position != movePoint.transform.position)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, movePoint.transform.position, speed * Time.fixedDeltaTime);
                rb.MovePosition(pos);
            }
            else
            {
                movePoint.transform.position = GenerateNewMovePosition();
                transform.LookAt(movePoint.transform);
                back = false;
            }
        }
        else if (Vector3.Distance(target.transform.position, transform.position) < agrDistance )
        {
            back = false;
            if (anim.GetBool("isRunning"))
            {
                if (transform.position != movePoint.transform.position)
                {
                    Vector3 pos = Vector3.MoveTowards(transform.position, movePoint.transform.position, speed * Time.fixedDeltaTime);
                    rb.MovePosition(pos);
                    //movePoint.transform.position = pos;
                }
                else
                {
                    Vector3 pos = Vector3.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime * 10);
                    movePoint.transform.position = pos;
                    transform.LookAt(movePoint.transform);
                }
            }
        }
        else
        {
            back = false;
            if (moveTime < DateTime.Now.AddSeconds(-moveDelay))
            {
                moveTime = DateTime.Now;
                movePoint.transform.position = GenerateNewMovePosition();
                transform.LookAt(movePoint.transform);
            }
            if (Vector3.Distance(startPosition, movePoint.transform.position) > patrolDistance)
            {
                movePoint.transform.position = startPosition + Vector3.zero;
                transform.LookAt(movePoint.transform);
            }
            if (transform.position != movePoint.transform.position)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, movePoint.transform.position, speed * Time.fixedDeltaTime);
                rb.MovePosition(pos);
            }
            else
            {
                movePoint.transform.position = GenerateNewMovePosition();
                transform.LookAt(movePoint.transform);
            }
        }
    }

    private Vector3 GenerateNewMovePosition()
    {
        Vector3 pos = startPosition + Vector3.zero;
        pos.x = startPosition.x + rand.Next(-patrolDistance, patrolDistance);
        pos.z = startPosition.z + rand.Next(-patrolDistance, patrolDistance);
        return pos;
    }
}
