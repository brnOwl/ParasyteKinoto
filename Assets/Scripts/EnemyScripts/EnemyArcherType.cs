using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcherType : EnemyType
{
    [Header("Enemy Projectiles")]
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

    [Header("Game Manager")]
    //public GameObject gameManagerObject;
    //private GameManager gameManager;

    [Header("Navigation AI")]
    //private GameObject followTarget;
    //NavMeshAgent agent;
    public float fireRange = 50f;



    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GameManager.Instance;
        //gameManager = gameManagerObject.GetComponent<GameManager>();

        sprite = GetComponentInChildren<SpriteRenderer>();

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
        FindTarget();
        
        EnemyRangeRotation();
        // Calculate distance between target and enemy
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

    private void EnemyRangeRotation()
    {
        if (target != null) lookAngle = rotatePoint.lookAngle;
        else lookAngle = 0;
    }

}
