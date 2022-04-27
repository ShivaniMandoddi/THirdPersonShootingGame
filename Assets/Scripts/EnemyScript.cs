using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int startingHealth;
    [SerializeField] int currentHealth;

    void Start()
    {
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageMethod(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Health:" + currentHealth);
        if (currentHealth<=0)
        {
            DeathMethod();
           
        }
    }

    private void DeathMethod()
    {
        gameObject.SetActive(false);
    }
}
