using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool Friendly;
    public float Speed;

    private void Start()
    {
        Destroy(gameObject, 6);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * Speed/100, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
