using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraTransition : MonoBehaviour
{
    public bool follow;
    public GameObject[] enemiesToSpawn;

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        if (enemiesToSpawn.Length > 0)
        {
            foreach (GameObject enemy in enemiesToSpawn)
            {
                var hp = enemy.GetComponent<EnemyHealth>();
                hp.DeactivateChild();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 11;
            if (follow)
            {
                transform.GetComponentInChildren<CinemachineVirtualCamera>().m_Follow = collision.gameObject.transform;
            }

            if(enemiesToSpawn.Length > 0)
            {
                foreach(GameObject enemy in enemiesToSpawn)
                {
                    var hp = enemy.GetComponent<EnemyHealth>();
                    if (!hp.dead)
                    {
                        hp.ActivateChild();
                    }
                    else
                    {
                        hp.DeactivateChild();
                    }
                }
            }


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            transform.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 10;
            CancelInvoke("Deactivate");
            Invoke("Deactivate", 4);

            if(enemiesToSpawn.Length > 0)
            {
                CancelInvoke("DeactivateEnemies");
                Invoke("DeactivateEnemies", 1);
            }

        }
    }

    void DeactivateEnemies()
    {
        foreach(GameObject enemy in enemiesToSpawn)
        {
            var hp = enemy.GetComponent<EnemyHealth>();
            hp.DeactivateChild();
        }
    }

    void Deactivate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
