using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimbController : MonoBehaviour
{
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerController.SetCanWallClimb(true);
        Debug.Log("Wall Grabbed");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerController.SetCanWallClimb(false);
        Debug.Log("Wall Released");
    }
}
