using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]

public class SC_TPSController : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    private float gravity = 20.0f;
    public Transform playerCameraParent;
    private float lookSpeed = 2.0f;
    private float lookXLimit = 60.0f;
    private Animator anim;
    public float hp;
    public float maxHp;
    private float damage = 20;
    private bool isAttackDistance = false;
    private GameObject target = null;
    private double attackDelay = 1f;
    private DateTime dateTime = DateTime.Now;
    private int lvl;
    private int xp;
    private int maxXp;
    private int hpPot;
    private int maxHpPot;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;
        anim = GetComponent<Animator>();
        hp = maxHp;
        GameObject healthText = GameObject.Find("HealthText");
        healthText.GetComponent<Text>().text = hp + " / " + maxHp;
        lvl = 1;
        xp = 0;
        maxXp = 100;
        AddXp(0);
        hpPot = 50;
        maxHpPot = 10;
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed;
                anim.SetBool("isRunning", true);
            }

            if(curSpeedX != 0 || curSpeedY != 0)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (dateTime < DateTime.Now.AddSeconds(-attackDelay))
            {
                anim.SetTrigger("attack");
                dateTime = DateTime.Now;
                if (isAttackDistance)
                {
                    target.GetComponent<EnemyFolow>().hp -= damage;
                    Debug.Log("Enemy hp = " + target.GetComponent<EnemyFolow>().hp);
                    Image healthBarImage = target.transform.Find("CanvasEnemy/HealthBarEnemy/HealthBarInnerEnemy").GetComponent<Image>();
                    healthBarImage.fillAmount = Mathf.Clamp(target.GetComponent<EnemyFolow>().hp / target.GetComponent<EnemyFolow>().maxHp, 0, 1f);
                    Transform textHpBar = healthBarImage.transform.Find("HealthTextEnemy");
                    textHpBar.GetComponent<Text>().text = target.GetComponent<EnemyFolow>().hp + " / " + target.GetComponent<EnemyFolow>().maxHp;
                    if (target.GetComponent<EnemyFolow>().hp <= 0)
                    {
                        GameObject objectService = GameObject.Find("CanvasPlayer");
                        int xpEnemy = target.GetComponent<EnemyFolow>().xp;
                        if (objectService.GetComponent<Initial>().DestroyEnemy(target))
                        {
                            target = null;
                            isAttackDistance = false;
                            AddXp(xpEnemy);
                        }
                    }
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("Potion trig = " + other.gameObject.tag);
        if (other.gameObject.tag == "Enemy")
        {
            isAttackDistance = true;
            if (target == null)
            {
                target = other.gameObject;
            }
        }
        else if (other.gameObject.tag == "Hp potion" && hp < maxHp)
        {
            hp += hpPot;
            if (hp > maxHp)
            {
                hp = maxHp;
            }
            GameObject objectService = GameObject.Find("CanvasPlayer");
            objectService.GetComponent<Initial>().DestroyHpPotion(other.gameObject);
            OnDrawHpBar();
        }
        else if (other.gameObject.tag == "Increase hp")
        {
            maxHp += maxHpPot;
            GameObject objectService = GameObject.Find("CanvasPlayer");
            objectService.GetComponent<Initial>().DestroyMaxHpPotion(other.gameObject);
            OnDrawHpBar();
        }
    }

    private void OnDrawHpBar()
    {
        Image healthBarImage = GameObject.Find("HealthBarInner").GetComponent<Image>();
        healthBarImage.fillAmount = Mathf.Clamp(hp / maxHp, 0, 1f);
        Transform textHpBar = healthBarImage.transform.Find("HealthText");
        textHpBar.GetComponent<Text>().text = hp + " / " + maxHp;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            isAttackDistance = false;
            target = null;
        }
    }

    private void AddXp(int expirience)
    {
        xp += expirience;
        if (xp > maxXp)
        {
            xp -= maxXp;
            lvl++;
            maxXp = (int)((float)maxXp * 1.5);
            GameObject lvlText = GameObject.Find("LvlText");
            lvlText.GetComponent<Text>().text = lvl.ToString();
            hp = maxHp;
            OnDrawHpBar();
        }
        GameObject xpText = GameObject.Find("XpText");
        xpText.GetComponent<Text>().text = xp + " / " + maxXp;
        Image xpBarImage = GameObject.Find("XpBarInner").GetComponent<Image>();
        Debug.Log(xpBarImage);
        xpBarImage.fillAmount = Mathf.Clamp((float)xp / (float)maxXp, 0, 1f);
    }
}