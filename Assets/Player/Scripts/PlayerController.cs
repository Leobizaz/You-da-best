using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    //variaveis publicas
    public float speed = 5;
    public float jumpForce = 5;
    public float acceleration = 0.04f;
    public float decceleration = 0.01f;
    public float maxHP = 100;
    public float currentHP = 0;
    public float invulFrame = 1;
    //variaveis privadas
    private bool wantToJump;
    private bool isMoving;
    private bool hasSquashed;
    private bool facingLeft;
    private bool facingRight;
    public float x = 0;
    private float y = 0;
    private float yVelocity;
    private float dVelocity;
    private float jumpCooldown;
    private bool jumpBlock;
    bool walljumping;
    bool movimentionBlock;
    bool m_blockLeft;
    bool m_blockRight;
    bool dashing;
    float hitcooldown;
    //referencias publicas
    public GameObject playerRenderer;
    public ParticleSystem DashRightFX;
    public ParticleSystem DashLeftFX;
    //referencias privadas
    PlayerHUD hud;
    Animator renderAnimator;
    Rigidbody2D rb;
    PlayerCollision playerCollision;

    private void Awake()
    {
        renderAnimator = playerRenderer.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollision = GetComponent<PlayerCollision>();
        hud = GetComponent<PlayerHUD>();
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        if (transform.position.y < -20)
        {
            transform.position = new Vector3(0, -2, 4.102344f);
        }

        if (hitcooldown > 0) hitcooldown -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
        if (!movimentionBlock)
        {
            Movimentação();
        }
        SquashStretch();
        RenderRotation();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Harm")
        {
            if(hitcooldown <= 0)
            {
                TakeDamage(collision.gameObject);
            }
        }
    }

    public void TakeDamage(GameObject source)
    {
        rb.AddRelativeForce(source.transform.forward * 2);
        currentHP -= 10;
        hud.SetHPBarValue(currentHP / 100);
        hitcooldown = invulFrame;
        ShakeCamera.current.Shake(0.3f, 2, 2);
    }

    private void LateUpdate()
    {
        Pulo();
        Dash();
        Walljump();
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isMoving)
        {
            if (facingRight)
            {
                Debug.Log("DashRight");
                dashing = true;
                rb.AddForce(new Vector2(5, 0));
                DashRightFX.Play();
                if (playerCollision.onAir)
                {
                    Invoke("CancelDashRight", 0.1f);
                }
                else
                {
                    Invoke("CancelDashRight", 0.2f);
                }
            }
            if (facingLeft)
            {
                Debug.Log("DashLeft");
                dashing = true;
                rb.AddForce(new Vector2(-5, 0));
                DashLeftFX.Play();
                if (playerCollision.onAir)
                {
                    Invoke("CancelDashLeft", 0.1f);
                }
                else
                {
                    Invoke("CancelDashLeft", 0.2f);
                }
            }
        }
    }

    void CancelDashRight()
    {
        rb.AddForce(new Vector2(-4, 0));
        dashing = false;
    }

    void CancelDashLeft()
    {
        rb.AddForce(new Vector2(4, 0));
        dashing = false;
    }

    private void Pulo()
    {
        if (Input.GetButtonDown("Jump"))
        {
            CancelInvoke("ResetWantToJump");
            Invoke("ResetWantToJump", 0.07f);
            wantToJump = true;
        }

        if ((playerCollision.onGround || playerCollision.onGroundCoyote) && jumpCooldown <= 0 && wantToJump == true)
        {
            Jump();
            jumpCooldown = 0.2f;
        }

        if (jumpCooldown > 0)
        {
            jumpCooldown = jumpCooldown - Time.deltaTime;
        }
    }
    void Walljump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCooldown <= 0)
            {
                if (playerCollision.onAir && playerCollision.onWallRight)
                {
                    //walljumping = true;
                    playerCollision.onGroundCoyote = false;
                    wantToJump = false;
                    rb.velocity = new Vector2(rb.velocity.x, 0); //reset
                    rb.AddForce(new Vector2(-5, jumpForce * 1.2f), ForceMode2D.Impulse);
                    jumpBlock = true;
                    Invoke("ResetJumpBlock", 0.4f);
                    hasSquashed = false;
                    jumpCooldown = 0.3f;
                }
                if (playerCollision.onAir && playerCollision.onWallLeft)
                {
                    //walljumping = true;
                    playerCollision.onGroundCoyote = false;
                    wantToJump = false;
                    rb.velocity = new Vector2(rb.velocity.x, 0); //reset
                    rb.AddForce(new Vector2(5, jumpForce * 1.2f), ForceMode2D.Impulse);
                    jumpBlock = true;
                    Invoke("ResetJumpBlock", 0.4f);
                    hasSquashed = false;
                    jumpCooldown = 0.3f;
                }
                Invoke("ResetWalljump", 0.4f);
            }
        }
    }
    void ResetWalljump()
    {
        walljumping = false;
        //x = 0;
        y = 0;
    }
    private void Movimentação()
    {
        if (rb.velocity.x <= 0 && isMoving)
        {
            facingLeft = true;
            facingRight = false;
        }
        else if (rb.velocity.x >= 0 && isMoving)
        {
            facingRight = true;
            facingLeft = false;
        }

        if (playerCollision.onWallRight && playerCollision.onAir) m_blockRight = true; else m_blockRight = false;
        if (playerCollision.onWallLeft && playerCollision.onAir) m_blockLeft = true; else m_blockLeft = false;

        //if (!playerCollision.onGround) acceleration = Mathf.SmoothDamp(acceleration, 0, ref yVelocity, 0.1f);
        //else acceleration = 0.04f;

        if (!walljumping)
        {
            if (playerCollision.onGround || playerCollision.onGroundCoyote)
            {

                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    isMoving = true;
                    x = Mathf.SmoothDamp(x, Input.GetAxis("Horizontal"), ref yVelocity, acceleration);
                    if (m_blockLeft)
                    {
                        x = Mathf.Clamp(x, 0, 1);
                    }
                    if (m_blockRight)
                    {
                        x = Mathf.Clamp(x, -1, 0);
                    }
                }
                else
                {
                    isMoving = false;
                    x = Mathf.SmoothDamp(x, Input.GetAxis("Horizontal"), ref dVelocity, decceleration);
                }

                rb.velocity = new Vector2(x * speed, rb.velocity.y);


            }
            else         /////////////////////////////////////////////////////////////
            {
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    isMoving = true;
                    x = Mathf.SmoothDamp(x, Input.GetAxis("Horizontal"), ref yVelocity, acceleration);
                    if (m_blockLeft)
                    {
                        x = Mathf.Clamp(x, 0, 1);
                    }
                    if (m_blockRight)
                    {
                        x = Mathf.Clamp(x, -1, 0);
                    }
                }
                else
                {
                    isMoving = false;
                    //x = Mathf.SmoothDamp(x, Input.GetAxis("Horizontal"), ref dVelocity, decceleration);
                    x = 0;
                }


            }

            Vector2 dir = new Vector2(x, y);
            rb.velocity += new Vector2(dir.x * speed, 0);
            if (!dashing)
            {
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -speed, speed), rb.velocity.y);
            }

        }
    }
    private void Jump()
    {
        //playerCollision.coyote = true;
        playerCollision.onGroundCoyote = false;
        wantToJump = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;
        jumpBlock = true;
        Invoke("ResetJumpBlock", 0.4f);
        hasSquashed = false;
    }
    void ResetJumpBlock()
    {
        jumpBlock = false;
    }
    void ResetWantToJump()
    {
        wantToJump = false;
    }
    void RenderRotation()
    {

        if (isMoving)
        {
            if (facingLeft)
            {
                if (!playerCollision.onAir)
                    playerRenderer.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.3f);
                else
                    playerRenderer.transform.DOLocalRotate(new Vector3(0, 0, -10), 0.1f);
            }
            else if (facingRight)
            {
                if (!playerCollision.onAir)
                    playerRenderer.transform.DOLocalRotate(new Vector3(0, 0, -10), 0.3f);
                else
                    playerRenderer.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.1f);
            }
        }
        else
        {
            playerRenderer.transform.DOLocalRotate(Vector3.zero, 0.3f);
        }

    }
    void SquashStretch()
    {
        if (!jumpBlock)
        {
            if (!playerCollision.onAir && !hasSquashed)
            {
                CancelInvoke("Cu");
                Invoke("Cu", 0.5f);
                renderAnimator.enabled = true;
                renderAnimator.Play("Squash");
                hasSquashed = true;
            }
        }
    }
    void Cu()
    {
        renderAnimator.enabled = false;
    }

}
