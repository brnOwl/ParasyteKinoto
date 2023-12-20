using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyTyping;

namespace EnemyTyping
{
    public enum EnemyTypeEnum
    {
        Archer,
        Swordman,
        Chemist
    }
}

public class EnemyType : MonoBehaviour
{
    public EnemyTypeEnum enemyTyping;

    [Header("Target Information")]
    public GameObject target;
    public float targetDistance;
    public float targetOffset; // = targetDistance, but signed: if left: is negative
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

    protected SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
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
            isRayCollide = GetCollisionRaycast(transform.position, target.transform.position, targetDistance);

            if (targetOffset < 0) EnemyFaceRight(false);
            else if (targetOffset > 0) EnemyFaceRight(true);
        }
        else
        {
            EnemyFaceRight(true);
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
        Debug.Log("Collision: " + hit.collider.gameObject + ", Target: " + target);

        return (hit.collider.gameObject == target);
    }

    protected void EnemyFaceRight(bool setFace)
    {
        sprite.flipX = setFace;
    }
}
