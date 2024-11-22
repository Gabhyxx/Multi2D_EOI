using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;

    bool canMove = true;
    bool onWall = false;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float rayLenght;
    [SerializeField] int maxJumps;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform floorDetector;
    int currentJumps;

    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] int dashQuantity;

    [Header("Health")]
    [SerializeField] Image imageHealth;
    [SerializeField] float maxHealth;

    float currentHealth;

    [Header("Fruits")]
    [SerializeField] TextMeshProUGUI fruitsText;
    [SerializeField] TextMeshProUGUI fruitsScoreText;
    public int fruits;
    public int maxFruits;
    public int fruitScore;

    [Header("Change Player")]
    public RuntimeAnimatorController[] controller;
    public int playerNumber;

    [Header("GameUI")]
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject pausePanel;

    SpriteRenderer sprite;
    Animator anim;
    Rigidbody2D rig;
    PhotonView view;
    AudioSource source;

    Vector3 initialPosition;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        maxFruits = GameObject.Find("Fruits").transform.childCount;
        fruitsText = GameObject.Find("FruitsText").GetComponent<TextMeshProUGUI>();
        fruitsScoreText = GameObject.Find("FruitScoreText").GetComponent<TextMeshProUGUI>();
        imageHealth = GameObject.Find("HealthImage").GetComponent<Image>();
        inGamePanel = GameObject.Find("InGameUI");
        pausePanel = GameObject.Find("PausePanel");
        source = GetComponent<AudioSource>();
    }
    private void Start()
    {
        initialPosition = transform.position;
        currentHealth = maxHealth;

        if (!view.IsMine)
        {
            Destroy(transform.GetChild(1).gameObject);
        }

        if (view.IsMine)
        {
            inGamePanel.SetActive(true);
            pausePanel.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if (!view.IsMine) return;

        if (canMove)
        {
            Movement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            rig.bodyType = RigidbodyType2D.Static;
            anim.Play("Death");
            anim.SetBool("isDead", true);
            view.RPC("ResetPlayer", RpcTarget.All);
        }

        if (collision.gameObject.CompareTag("Fruits"))
        {
            collision.enabled = false;
            TakeFruits(collision.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!view.IsMine) return;
        if (collision.gameObject.CompareTag("Traps"))
        {
            GetDamage();
        }
    }
    private void Update()
    {
        Flip();
        if (!view.IsMine) return;

        if (Input.GetButtonDown("Jump") && canMove)
        {
            Jump();
        }
        if (Input.GetButtonDown("Dash") && canMove && currentJumps >= 1)
        {
            if (dashQuantity == 0)
            {
                Dash();
            }
        }
        CheckWall();
        anim.SetFloat("velocity.x", Mathf.Abs(rig.velocity.x));
        anim.SetFloat("velocity.y", rig.velocity.y); ;
        fruitsText.text = fruits + "/" + maxFruits;
    }
    void Movement()
    {
        rig.velocity = Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime +
                       Vector2.up * rig.velocity.y;
    }
    void Flip()
    {
        if (rig.velocity.x > 0.1)
        {
            sprite.flipX = false;
        }
        else if (rig.velocity.x < -0.1)
        {
            sprite.flipX = true;
        }
    }
    void Jump()
    {
        bool grounded = Physics2D.OverlapCircle(floorDetector.position, 0.1f, groundLayer);

        if (grounded)
        {
            currentJumps = 0;
            dashQuantity = 0;
        }

        if (onWall)
        {
            StartCoroutine(WallJump());
        }
        else if (currentJumps < maxJumps)
        {
            rig.velocity = Vector2.zero;
            rig.AddRelativeForce(Vector2.up * jumpForce);
            currentJumps++;

            if (currentJumps > 1)
            {
                view.RPC("SyncAnimation", RpcTarget.All, "DoubleJump");
            }
        }
    }
    void CheckWall()
    {
        Vector2 direction = sprite.flipX ? Vector2.left : Vector2.right;
        Debug.DrawLine(transform.position, (Vector2)transform.position + direction * rayLenght);
        if (Physics2D.Raycast(transform.position, direction, rayLenght, wallLayer))
        {
            rig.velocity = Vector2.zero;
            anim.SetBool("onWall", true);
            view.RPC("SyncAnimation", RpcTarget.All, "WallJump");
            currentJumps = 1;
            onWall = true;
        }
        else
        {
            onWall = false;
            anim.SetBool("onWall", false);
        }
    }
    IEnumerator WallJump()
    {
        canMove = false;
        rig.AddForce(Vector2.up * jumpForce);
        rig.AddForce((sprite.flipX ? Vector2.right : Vector2.left) * jumpForce / 2);
        yield return new WaitForSeconds(1);
        canMove = true;
    }
    void GetDamage()
    {
        view.RPC("SyncAnimation", RpcTarget.All, "Hit");
        currentHealth--;
        imageHealth.fillAmount = currentHealth / maxHealth;
        StartCoroutine(WallJump());
        if (currentHealth <= 0)
        {
            ResetPlayer();
        }
    }
    void TakeFruits(GameObject fruit)
    {
        if (view.IsMine)
        {
            fruits++;
            fruitScore += fruit.GetComponent<PickUpFruits>().score;
            fruitsText.text = fruits + "/" + maxFruits;
            fruitsScoreText.text = fruitScore.ToString();
            PlayPickUpSound();
        }
        fruit.GetComponent<Animator>().Play("Pick");
        Destroy(fruit, 0.5f);
    }
    [PunRPC]
    public void SetPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
        anim.runtimeAnimatorController = controller[playerNumber];
    }
    [PunRPC]
    public void SyncAnimation(string animation)
    {
        anim.Play(animation);
    }
    [PunRPC]
    void ResetPlayer()
    {
        StartCoroutine(ResetPosition());
        currentHealth = maxHealth;
        imageHealth.fillAmount = 1;
    }
    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(1);
        anim.SetBool("isDead", false);
        rig.bodyType = RigidbodyType2D.Kinematic;
        rig.position = initialPosition;
        yield return new WaitForSeconds(1);
        rig.bodyType = RigidbodyType2D.Dynamic;
    }
    public float GetScore()
    {
        return fruitScore;
    }
    public float GetHealthScore()
    {
        return currentHealth / maxHealth;
    }
    public float GetFinalScore()
    {
        return fruitScore + fruitScore * currentHealth / maxHealth;
    }
    void Dash()
    {
        if (rig.velocity.x > 0.1)
        {
            rig.AddForce(new Vector2(dashForce, 0));
        }
        else if (rig.velocity.x < -0.1)
        {
            rig.AddForce(new Vector2(-dashForce, 0));
        }
        dashQuantity = 1;
    }
    void PlayPickUpSound()
    {
        source.Play();
    }
}