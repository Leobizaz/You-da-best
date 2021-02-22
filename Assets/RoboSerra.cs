using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboSerra : MonoBehaviour
{
    [Header("Settings")]
    public float damping;

    [Space]
    [Header("Debug")]
    public GameObject target;
    public Vector3 startPosition;
    Rigidbody2D rb;
    EnemyHealth hp;
    float xVelocity;
    float yVelocity;

    private void OnValidate()
    {
        startPosition = this.transform.position;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hp = GetComponentInParent<EnemyHealth>();
    }

    private void OnEnable()
    {
        transform.position = startPosition;
        rb.MovePosition(startPosition);
        target = GameObject.FindGameObjectWithTag("Player");
        hp.currentHP = hp.maxHP;
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float x = 0;
        float y = 0;

        x = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref xVelocity, damping);
        y = Mathf.SmoothDamp(transform.position.y, target.transform.position.y, ref yVelocity, damping);

        rb.MovePosition(new Vector2(x,y));
    }

}
