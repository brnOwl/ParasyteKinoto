using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassTyping;

public class EnemyType : MonoBehaviour
{
    public ClassType enemyTypeEnum;

    [Header("Target Information")]
    public GameObject target;
    public float targetDistance;
    public float targetOffset; // = targetDistance, but signed: if left: is negative
    public float targetDistanceLimit = 40f;
    [Header("Enemy Collision Raycast")]
    public bool isRayCollide = true; // Says whether or not the enemy has a clear LOS to the target (player)

    [Header("Animations")]
    public bool isEnemyMoving;
    protected Transform vfxTransform;
    protected Animator animator;

    [Header("Enemy Stats")]
    public float enemyMaxHealth = 50f;
    public float enemyCurrentHealth;

    protected int facingRight = 1;

    protected SpriteRenderer enemySprite;

    // Start is called before the first frame update
    protected void Start()
    {
        enemySprite = transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        enemyCurrentHealth = enemyMaxHealth;
        animator = GetComponentInChildren<Animator>();
        vfxTransform = gameObject.transform.GetChild(1).transform;
    }

    // Update is called once per frame
    protected void Update()
    {
        FindTarget();
        enemySprite.gameObject.transform.localScale = new Vector3(facingRight, 1, 1);
        //FaceTarget(targetOffset);
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
            } else if (other.tag == "PlayerProjectile")
            {
                float damage = other.GetComponent<Projectile>().GetProjectileDamage();
                Debug.Log("Hit!: " + damage);
                EnemyTakeDamage(damage);
            }
        }
    }

    protected void FindTarget()
    {
        target = GameObject.FindWithTag("Player");
       
        if (target != null)
        {
            targetDistance = GetTargetDistance();
            targetOffset = target.transform.position.x - transform.position.x;
            if (targetDistance < targetDistanceLimit)
            {
                isRayCollide = GetCollisionRaycast(transform.position, target.transform.position, targetDistance);


                if (targetOffset < 0) facingRight = -1;
                else if (targetOffset > 0) facingRight = 1;
                return;
            }
        }
        facingRight = 1;
    }

    protected void FaceTarget()
    {
        
        
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

    public float GetTargetDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position);
    }

    private bool GetCollisionRaycast(Vector2 origin, Vector2 end, float distance)
    {
        float angle = Mathf.Atan2(end.y - origin.y, end.x - origin.x);
        angle *= Mathf.Rad2Deg;

        // Cast Raycast
        float radianAngle = angle * Mathf.Deg2Rad;
        Vector2 angleDirection = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

        RaycastHit2D hit;
        hit = Physics2D.Raycast(origin, angleDirection, distance, LayerMask.GetMask("Ground", "Player")); ;

        // Debug by drawing in editor view
        Debug.DrawRay(origin, angleDirection * distance, Color.magenta);
        if (hit.collider == null) return false;
        //Debug.Log("Collision: " + hit.collider.gameObject + ", Target: " + target);

        return (hit.collider.gameObject == target);
    }

}
