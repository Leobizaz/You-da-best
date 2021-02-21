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
    //variaveis privadas
    private bool wantToJump;
    private bool isMoving;
    private bool hasSquashed;
    private bool facingLeft;
    private bool facingRight;
    public float x = 0;
    private float y = 0;
    private float yVelocity;
    private float jumpCooldown;
    private bool jumpBlock;
    bool walljumping;
    bool movimentionBlock;
    bool m_blockLeft;
    bool m_blockRight;
    //referencias publicas
    public GameObject playerRenderer;
    //referencias privadas
    Animator renderAnimator;
    Rigidbody2D rb;
    PlayerCollision playerCollision;

    private void Awake()
    {
        renderAnimator = playerRenderer.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollision = GetComponent<PlayerCollision>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (transform.position.y < -5)
        {
            transform.position = new Vector3(0, -2, 0);
        }
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

    private void LateUpdate()
    {
        Pulo();
        Walljump();
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
                    walljumping = true;
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
                    walljumping = true;
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
        x = 0;
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
                x = Mathf.SmoothDamp(x, Input.GetAxis("Horizontal"), ref yVelocity, decceleration);
            }

            Vector2 dir = new Vector2(x, y);
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
            }
            else
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
                    x = Mathf.SmoothDamp(x, Input.GetAxis("Horizontal"), ref yVelocity, decceleration);
                }

                Vector2 dir = new Vector2(x, y);
                rb.velocity += new Vector2(dir.x * speed, 0);
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
