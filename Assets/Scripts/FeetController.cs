using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetController : MonoBehaviour
{
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag == "Ground")
        {
            playerController.canJump = true;
            playerController.SetCanWallJump(true);
            playerController.rb.velocity = new Vector2(playerController.rb.velocity.x, 0);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            playerController.canJump = false;
        }
    }
}
