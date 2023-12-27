using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassTyping;

public class EnemyTypeRange : EnemyType
{
    [Header("Enemy Projectiles")]
    public GameObject projectile;
    public float projectileVelocity = 20f;
    public Transform firePoint;
    public float enemyProjectileDamage = 20f;

    [Header("Enemy Rotation")]
    public Vector2 lookDirection;
    public float lookAngle;
    public float bulletDespawnTime = 2f;
    public bool fireCoolDown;
    public float fireInterval = 1f;
    public bool canFire = true;
    public RotatePoint rotatePoint;

    public bool isWithinRange = false;
    public bool isWithinDetection = false;


    [Header("Navigation AI")]
    public float fireRange = 50f;

    // Update is called once per frame
    new void Update()
    {
        FindTarget();

        EnemyRangeRotation();

        isWithinDetection = targetDistance <= targetDistanceLimit;

        if (fireCoolDown && target != null && canFire && isWithinRange && isWithinDetection && isRayCollide) StartCoroutine(FirePattern());
    }

    protected IEnumerator FirePattern()
    {
        FireProjectile();
        fireCoolDown = false;
        yield return new WaitForSeconds(fireInterval);
        fireCoolDown = true;
    }

    protected void FireProjectile()
    {
        GameObject newProjectile = Instantiate(projectile.gameObject);

        newProjectile.transform.position = transform.position;
        newProjectile.transform.rotation = Quaternion.Euler(0, 0, lookAngle);

        // Assuming: EnemyProjectile == 11 // tag and layer
        newProjectile.layer = 11;
        newProjectile.tag = "EnemyProjectile";

        newProjectile.GetComponent<Rigidbody2D>().velocity = firePoint.right * projectileVelocity;

        Projectile projectileController = newProjectile.GetComponent<Projectile>();

        
        projectileController.DespawnProjectile(bulletDespawnTime);
        projectileController.SetProjectileDamage(enemyProjectileDamage);
        projectileController.SetLauncher(EntityOwner.Enemy);
        
    }

    protected void EnemyRangeRotation()
    {
        if (target != null) lookAngle = rotatePoint.lookAngle;
        else lookAngle = 0;
    }
}
