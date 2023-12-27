using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetController : MonoBehaviour
{
    private PlayerType playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerType>();
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

    // Trigger

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.tag);

        if (collider.gameObject.tag == "Ground")
        {
            playerController.canJump = true;
            playerController.SetCanWallJump(true);
            playerController.rb.velocity = new Vector2(playerController.rb.velocity.x, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            playerController.canJump = false;
        }
    }
}
