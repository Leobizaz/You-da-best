using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DG.Tweening;
public class RoboSerra : MonoBehaviour
{
    [Header("Settings")]
    public float damping;
    [Space]

    [Header("References")]
    public ParticleSystem hitFX;
    public GameObject deathFX;
    public GameObject sprite;
    [Space]

    [Header("Debug")]
    public GameObject target;
    public Vector3 startPosition;
    public float x;
    public float y;
    Rigidbody2D rb;
    EnemyHealth hp;
    AIDestinationSetter pathfind;
    float xVelocity;
    float yVelocity;
    bool dead;

    private void OnValidate()
    {
        startPosition = this.transform.position;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfind = GetComponent<AIDestinationSetter>();
        hp = GetComponentInParent<EnemyHealth>();
    }

    private void OnEnable()
    {
        transform.position = startPosition;
        rb.MovePosition(startPosition);
        target = GameObject.FindGameObjectWithTag("Player");
        pathfind.target = target.transform;
        hp.currentHP = hp.maxHP;
    }

    void Update()
    {
        if (!dead)
        {
            //Movement();
        }
        DeathCheck();
    }

    void Movement()
    {
        x = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref xVelocity, damping);
        y = Mathf.SmoothDamp(transform.position.y, target.transform.position.y, ref yVelocity, damping);

        rb.AddForce(new Vector2(x,y));
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -2, 2), Mathf.Clamp(rb.velocity.y, -2, 2));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
        {
            TakeDamage(collision.gameObject);
            Destroy(collision.gameObject);
        }
    }

    void DeathCheck()
    {
        if(hp.currentHP <= 0 && !dead)
        {
            dead = true;
            hp.dead = true;
            Instantiate(deathFX, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(GameObject impact)
    {
        rb.AddRelativeForce(impact.transform.up * 2);
        sprite.transform.DOShakePosition(0.3f, 0.2f);
        sprite.transform.DOShakeRotation(0.5f, 1f);
        hitFX.Play();
        hp.currentHP = hp.currentHP - 10;
    }

}
