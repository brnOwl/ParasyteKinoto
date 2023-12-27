using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChemistProjectile : Projectile
{
    public GameObject splashParticleSystem;


    protected new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            InstantiateSplash();
            Destroy(gameObject);
        }
        InstantiateSplash();
    }

    void InstantiateSplash()
    {
        GameObject newSplash = Instantiate(splashParticleSystem);
        newSplash.transform.position = transform.position;
        
    }
}
