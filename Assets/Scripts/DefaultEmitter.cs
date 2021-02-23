using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEmitter : MonoBehaviour
{
    public LineRenderer laserPointerLine;
    public LayerMask laserPointerMask;
    void Start()
    {
        
    }

    void Update()
    {
        LaserGuide();
    }

    void LaserGuide()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1000, laserPointerMask);
        if(hit.collider != null)
        {
            laserPointerLine.useWorldSpace = false;
            float distance = Vector3.Distance(hit.point, transform.position);
            //laserPointerLine.SetPosition(0, transform.position);
            laserPointerLine.SetPosition(0, new Vector3(0, 0, 0));
            //laserPointerLine.SetPosition(1, hit.point);
            laserPointerLine.SetPosition(1, new Vector3(0, distance/2, 0));
        }
        else
        {
            laserPointerLine.useWorldSpace = false;
            laserPointerLine.SetPosition(0, new Vector3(0, 0, 0));
            laserPointerLine.SetPosition(1, new Vector3(0, 100, 0));
        }
    }
}
