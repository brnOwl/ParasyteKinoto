using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchArcRenderer : MonoBehaviour
{
    LineRenderer lr;

    public float velocity;
    public float angle;
    public int resolution = 10;
    public float maxDistance;
    public EnemyArcherType controller;
    public float calculatedAngle;

    float g;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics2D.gravity.y);
        controller = GetComponent<EnemyArcherType>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.target != null) ManageArc();

    }

    private void ManageArc()
    {
        velocity = controller.projectileVelocity;
        angle = controller.rotatePoint.lookAngle;
        RenderArc();
    }

    // Populating the LineRender
    private void RenderArc()
    {
        lr.positionCount = resolution + 1;
        lr.SetPositions(CalculateArcArray());
    }

    private Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];
        if (angle < 0) angle += 360f;
        float radianAngle = Mathf.Deg2Rad * angle;

        //lookAngle = angle * Mathf.Rad2Deg;


        // maxDistance = Mathf.Abs((velocity * velocity * Mathf.Sin(2 * radianAngle))) / g;
        maxDistance = controller.target.transform.position.x - controller.transform.position.x;
        //maxDistance = Vector2.Distance(controller.target.transform.position, controller.target.transform.position);
        float totalTime = maxDistance / (velocity * Mathf.Cos(radianAngle));

        for(int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution * (float)totalTime;
            arcArray[i] = CalculateArcPoint(t, radianAngle);
        }

        return arcArray;
    }

    private Vector3 CalculateArcPoint(float t, float lookAngle)
    {
        float x = transform.position.x + t * velocity * Mathf.Cos(lookAngle);
        int yFactor = (lookAngle < 0) ? -1 : 1; 
        float y = transform.position.y + yFactor * t * velocity * Mathf.Sin(lookAngle) - g * t * t/2;
        return new Vector3(x, y);
    }

    
}
