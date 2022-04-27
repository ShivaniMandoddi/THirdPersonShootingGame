using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    [Range(0.5f,1.5f)]
    float fireRate = 1f;
    [SerializeField]
    [Range(1f, 10f)]
    float damageRate = 1f;
    float timer;
    public Transform firePoint;
    [SerializeField]
    ParticleSystem particleSystem;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + Time.deltaTime;
        if(timer>=fireRate)
        {
            if(Input.GetButton("Fire1"))
            {
                timer = 0;
                ToFireGun();

            }
        }
    }

    private void ToFireGun()
    {

        particleSystem.Play();
        //To add audiosource
        Debug.DrawRay(firePoint.position, transform.forward * 100,Color.red,5f);
        Ray ray = new Ray(firePoint.position, transform.forward);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,100f))
        {
            //Need to shoot the enemy
           var health= hitInfo.collider.GetComponent<EnemyScript>();
            if(health!=null)
            {
                health.DamageMethod(5);
            }
        }
    }
}
