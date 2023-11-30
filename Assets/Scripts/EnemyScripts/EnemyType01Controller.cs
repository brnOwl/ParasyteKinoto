using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType01Controller : MonoBehaviour
{
    [Header("Enemy Projectiles")]
    public GameObject target;
    public GameObject projectile;
    public float projectileVelocity = 20f;
    public Transform firePoint;
    public Vector2 lookDirection;
    public float lookAngle;
    public float bulletDespawnTime = 2f;
    public bool fireCoolDown;
    public float fireInterval = 1f;
    public bool canFire = true;
    public RotatePoint rotatePoint;
    public bool isWithinRange = true;
    public float targetDistance;
    public float targetOffset; // = targetDistance, but signed: if left: is negative

    [Header("Enemy Collision Raycast")]
    public bool isRayCollide = true; // Says whether or not the enemy has a clear LOS to the target (player)

    [Header("Enemy Stats")]
    public float enemyMaxHealth = 50f;
    public float enemyCurrentHealth;

    [Header("Game Manager")]
    //public GameObject gameManagerObject;
    //private GameManager gameManager;

    [Header("Navigation AI")]
    //private GameObject followTarget;
    //NavMeshAgent agent;
    public float fireRange = 50f;

    [Header("Animations")]
    public bool isEnemyMoving;
    private Transform vfxTransform;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GameManager.Instance;
        //gameManager = gameManagerObject.GetComponent<GameManager>();

        fireCoolDown = true;
        enemyCurrentHealth = enemyMaxHealth;

        // Fire Point Transform
        //rotatePoint = transform.GetChild(0).transform;

        // Navigation AI
        //agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        //agent.updateUpAxis = false;

        // Animation
        animator = GetComponentInChildren<Animator>();
        vfxTransform = gameObject.transform.GetChild(1).transform;

        // Target Assignment
        //target = PlayerController.Instance.gameObject;
        //followTarget = PlayerController.Instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        if (target != null)
        {
            targetDistance = GetTargetDistance();
            isRayCollide = GetCollisionRaycast(transform.position, target.transform.position, targetDistance);
        }
        
        EnemyRangeRotation();
        // Calculate disance between target and enemy
        if (fireCoolDown && target != null && canFire && isWithinRange && isRayCollide) StartCoroutine(FirePattern());

        // Navigation AI
        //if (target != null && agent.enabled) agent.SetDestination(followTarget.transform.position);

        if (lookDirection.x < 0) vfxTransform.localScale = new Vector3(-1, vfxTransform.localScale.y, vfxTransform.localScale.z);
        else if (lookDirection.x > 0) vfxTransform.localScale = new Vector3(1, vfxTransform.localScale.y, vfxTransform.localScale.z);

        //if (agent.velocity != Vector3.zero) animator.SetBool("IsEnemyMoving", true);
        //else animator.SetBool("IsEnemyMoving", false);
    }

    IEnumerator FirePattern()
    {
        FireProjectile();
        fireCoolDown = false;
        yield return new WaitForSeconds(fireInterval);
        fireCoolDown = true;

    }

    private void FireProjectile()
    {
        GameObject newProjectile = Instantiate(projectile.gameObject);

        newProjectile.transform.position = transform.position;
        newProjectile.transform.rotation = Quaternion.Euler(0, 0, lookAngle);

        newProjectile.GetComponent<Rigidbody2D>().velocity = firePoint.right * projectileVelocity;

        StartCoroutine(DespawnProjectile(newProjectile));
    }

    IEnumerator DespawnProjectile(GameObject despawningProjectile)
    {
        yield return new WaitForSeconds(bulletDespawnTime);
        Destroy(despawningProjectile);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (other.tag == "PlayerMelee")
            {
                float damage = other.GetComponent<MeleeController>().meleeDamage;
                Debug.Log("Hit!: " + damage);
                EnemyTakeDamage(damage);
            }
        }
    }

    public void EnemyTakeDamage(float damage)
    {
        enemyCurrentHealth -= damage;
        if (enemyCurrentHealth <= 0) TriggerEnemyDeath();
    }

    public void TriggerEnemyDeath()
    {
        //Debug.Log("Boom");
        //gameManager.Explode(transform);
        Destroy(gameObject);


    }

    private void EnemyRangeRotation()
    {
        if (target != null) lookAngle = rotatePoint.lookAngle;
        else lookAngle = 0;
    }

    // Manage Raycast
    private bool GetCollisionRaycast(Vector2 origin, Vector2 end, float distance)
    {
        float angle = Mathf.Atan2(end.y-origin.y, end.x-origin.x);
        angle *= Mathf.Rad2Deg;

        // Cast Raycast
        float radianAngle = angle * Mathf.Deg2Rad;
        Vector2 angleDirection = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

        RaycastHit2D hit;
        hit = Physics2D.Raycast(origin, angleDirection, distance, LayerMask.GetMask("Ground", "Player")); ;

        // Debug by drawing in editor view
        Debug.DrawRay(origin, angleDirection * distance, Color.magenta);
        if (hit.collider == null) return false;
        Debug.Log("Collision: " + hit.collider.gameObject + ", Target: " + target);

        return (hit.collider.gameObject == target);
    }

    public float GetTargetDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position);
    }

    private void FindPlayer()
    {
        target = GameObject.FindWithTag("Player");
    }
}
