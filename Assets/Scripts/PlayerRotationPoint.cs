using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationPoint : MonoBehaviour
{
    private PlayerController playerController;

    public float rotationAngle;
    public float currentRotation;
    public float swingDistance = 90f;

    private float newRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        rotationAngle = playerController.playerAngle * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }

    void RotateSwingHitbox()
    {
        currentRotation = Mathf.Lerp(currentRotation, newRotation, Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }
}
