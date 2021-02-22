using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraTransition : MonoBehaviour
{
    public bool follow;

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            transform.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 10;
            Invoke("Deactivate", 4);
        }
    }

    void Deactivate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
