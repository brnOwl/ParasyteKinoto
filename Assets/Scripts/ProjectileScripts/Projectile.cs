using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassTyping;

public class Projectile : MonoBehaviour
{
    protected GameObject gameManager;
    public float longevityTime = 10f;
    protected Vector3 dir = Vector3.zero;
    protected Rigidbody2D rb;
    private float projectileDamage = 20f;

    private EntityOwner launcher;
    private SpriteRenderer projectileSprite;

    private void Start()
    {
        //gameManager = GameManager.Instance.gameObject;
        DespawnProjectile(longevityTime);
        rb = GetComponent<Rigidbody2D>();
        projectileSprite = gameObject.GetComponentInChildren<SpriteRenderer>();

        if (launcher == EntityOwner.Enemy)
        {
            projectileSprite.color = Color.red;
        } 
        else if (launcher == EntityOwner.Player)
        {
            projectileSprite.color = Color.green;
        }
    }

    private void Update()
    {
        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, rb.velocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 100);
    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground") Destroy(gameObject);
    }

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public void DespawnProjectile(float disappearTime)
    {
        StartCoroutine(DespawnProjectileTimer(disappearTime));
    }

    public IEnumerator DespawnProjectileTimer(float bulletDespawnTime)
    {
        yield return new WaitForSeconds(bulletDespawnTime);
        Destroy(gameObject);
    }

    public float GetProjectileDamage()
    {
        return projectileDamage;
    }

    public void SetProjectileDamage(float newProjectileDamage)
    {
        projectileDamage = newProjectileDamage;
    }

    public void SetLauncher(EntityOwner newLauncher)
    {
        this.launcher = newLauncher;
    }
}
