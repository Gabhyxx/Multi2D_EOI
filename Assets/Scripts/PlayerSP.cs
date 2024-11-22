using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSP : MonoBehaviour
{
    public static PlayerSP instance;

    [Header("Movement")]
    public float speed;
    [SerializeField] float orangeBoostTime;
    [SerializeField] float strawberryBoostTime;
    [SerializeField] float pinappleBoostTime;

    float orangeBoost;
    float strawberryBoost;
    bool canMove = true;
    bool onWall = false;

    [Header("Jump")]
    public float jumpForce;
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

    public float currentHealth;

    [Header("Fruits")]
    [SerializeField] TextMeshProUGUI fruitsText;
    [SerializeField] TextMeshProUGUI fruitsScoreText;
    public int fruits;
    public int maxFruits;
    public int fruitScore;

    [Header("Change Player")]
    public RuntimeAnimatorController[] controller;
    public int playerNumber;

    SpriteRenderer sprite;
    Animator anim;
    Rigidbody2D rig;
    AudioSource source;

    Vector3 initialPosition;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        maxFruits = GameObject.Find("Fruits").transform.childCount;
        fruitsText = GameObject.Find("FruitsText").GetComponent<TextMeshProUGUI>();
        imageHealth = GameObject.Find("HealthImage").GetComponent<Image>();
        fruitsText = GameObject.Find("FruitsText").GetComponent<TextMeshProUGUI>();
        fruitsScoreText = GameObject.Find("FruitScoreText").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        initialPosition = transform.position;
        currentHealth = maxHealth;
        orangeBoost = speed / 2; //Primera división entre 2, por ende, es un 50% de la velocidad inicial
        strawberryBoost = jumpForce / 2;
        SetPlayerNumber(GameManager.instance.playerNumber);
    }
    private void FixedUpdate()
    {
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
            ResetPlayer();
        }

        if (collision.gameObject.CompareTag("Fruits"))
        {
            collision.enabled = false;
            StartCoroutine(TakeFruits(collision.gameObject));
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Traps"))
        {
            StartCoroutine(GetDamage());
        }
    }
    private void Update()
    {
        Flip();

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
        if (rig.bodyType == RigidbodyType2D.Dynamic)
        {
            rig.velocity = Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime +
                           Vector2.up * rig.velocity.y;
        }
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
                anim.Play("DoubleJump");
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
            anim.Play("WallJump");
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
    IEnumerator GetDamage()
    {
        anim.Play("Hit");
        currentHealth--;
        imageHealth.fillAmount = currentHealth / maxHealth;
        StartCoroutine(WallJump());
        if (currentHealth <= 0)
        {
            yield return new WaitForSeconds(1f);
            rig.bodyType = RigidbodyType2D.Static;
            anim.Play("Death");
            anim.SetBool("isDead", true);
            ResetPlayer();
        }
    }
    IEnumerator TakeFruits(GameObject fruit)
    {
        fruits++;
        fruitScore += fruit.GetComponent<PickUpFruits>().score;
        fruitsText.text = fruits + "/" + maxFruits;
        fruitsScoreText.text = fruitScore.ToString();
        fruit.GetComponent<Animator>().Play("Pick");
        yield return new WaitForSeconds(0.5f);
        PlayPickUpSound();
        fruit.SetActive(false);
        if (fruit.GetComponent<PickUpFruits>().isOrange)
        {
            speed += orangeBoost;
            yield return new WaitForSeconds(orangeBoostTime);
            speed -= orangeBoost;
            yield return new WaitForEndOfFrame();
        }
        else if (fruit.GetComponent<PickUpFruits>().isStrawberry)
        {
            jumpForce += strawberryBoost;
            yield return new WaitForSeconds(strawberryBoostTime);
            jumpForce -= strawberryBoost;
            yield return new WaitForEndOfFrame();
        }
        else if (fruit.GetComponent<PickUpFruits>().isPinapple)
        {
            speed += orangeBoost / 2; //Al estar DOS veces dividido entre 2, hace un total del 25% de la velocidad original
            jumpForce += strawberryBoost / 2;
            yield return new WaitForSeconds(pinappleBoostTime);
            speed -= orangeBoost / 2;
            jumpForce -= strawberryBoost / 2;
            yield return new WaitForEndOfFrame();
        }
    }
    public void SetPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
        anim.runtimeAnimatorController = controller[playerNumber];
    }
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
    void PlayPickUpSound()
    {
        source.Play();
    }
}