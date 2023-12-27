using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePoint : MonoBehaviour
{
    public GameObject target;
    public Vector3 lookDirection;
    public float lookAngle;
    public EnemyTypeRange controller;
    public float enemyRotateGuardLERP = 0.2f;
    public float enemyRotateRelaxedLERP = 0.01f;
    float g;

    public float maxDistance;
    public float playerDistance;

    private void Awake()
    {
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start Rotate: ");
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.root.CompareTag("Enemy"))
        {
            controller = GetComponentInParent<EnemyTypeRange>();
            target = controller.target;
        }
        if (target != null) LookAtTarget();
        else LookAtDefault();
    }

    private void LookAtTarget()
    {
        // Calculate Angles and set rotation of projectile launcher
        lookAngle = CalculateAngleForDistance(target.transform.position, transform.position,
        GetComponentInParent<EnemyTypeRange>().projectileVelocity) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, lookAngle);
        
    }
    private void LookAtDefault()
    {
        lookAngle = Mathf.Lerp(lookAngle, 0, enemyRotateRelaxedLERP);
        transform.rotation = Quaternion.Euler(0, 0, lookAngle);
    }

    float CalculateAngleForDistance(Vector2 position1, Vector2 position2, float initialVelocity)
    {
        Vector2 distance = position1 - position2;
        float firstPartWhole = initialVelocity * initialVelocity / g / Mathf.Abs(distance.x);
        float secondPart1 = Mathf.Pow(initialVelocity, 2) * (Mathf.Pow(initialVelocity, 2) - 2 * g * distance.y);
        float secondPart2 = g * g * distance.x * distance.x;
        float divisionPart = secondPart1 / secondPart2 - 1;

        if (divisionPart < 0)
        {
            //Debug.Log("Too Far!");
            controller.isWithinRange = false;
            return 0;
        } else
        {
            controller.isWithinRange = true;
        }

        float secondPartWhole = Mathf.Sqrt(divisionPart);
        float finalDif = firstPartWhole - secondPartWhole;

        float finalPart = Mathf.Atan(finalDif);

        if (distance.x < 0)
        {
            finalPart *= -1;
            finalPart += (distance.y < 0) ? (-Mathf.PI) : (Mathf.PI);
        }

        return finalPart;
    }
}
