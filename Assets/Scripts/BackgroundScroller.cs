using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float movementFactorX, movementFactorY;
    [SerializeField] private Vector2 playerTransform;

    // Update is called once per frame
    void Update()
    {
        GetPlayerPosition();
    }

    void  GetPlayerPosition()
    {
        GameObject target = GameObject.FindWithTag("Player");

        if (target != null)
        {
            // playerTransform = new Vector2(target.transform.position.x * movementFactorX, target.transform.position.y * movementFactorY);
            playerTransform = new Vector2(target.transform.position.x * movementFactorX, 0);
            rawImage.uvRect = new Rect(playerTransform, rawImage.uvRect.size);
        }
    }
}
