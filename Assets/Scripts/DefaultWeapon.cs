using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DefaultWeapon : MonoBehaviour
{
    public float shootingCooldown = 1;
    private float cooldown;
    public GameObject emitter;
    public GameObject projectile;
    public GameObject weaponSprite;
    public AnimationCurve recoilCurve;
    public float recoilStrength;
    private void Start()
    {
        
    }

    private void Update()
    {
        Aiming();
        Shooting();
    }

    void Aiming()
    {
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        var dir = Input.mousePosition - pos;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Shooting()
    {
        if(cooldown != 0)
        {
            cooldown -= Time.deltaTime;
        }


        if (Input.GetMouseButtonDown(0) && cooldown <= 0)
        {
            weaponSprite.transform.DOPunchPosition(new Vector3(-recoilStrength, 0), shootingCooldown).SetEase(recoilCurve);
            Instantiate(projectile, emitter.transform.position, emitter.transform.rotation);
            cooldown = shootingCooldown;
        }
    }

}
