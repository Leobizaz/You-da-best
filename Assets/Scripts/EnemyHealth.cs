using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int currentHP;
    public int maxHP = 30;
    public bool dead = false;

    public void ActivateChild()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DeactivateChild()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
