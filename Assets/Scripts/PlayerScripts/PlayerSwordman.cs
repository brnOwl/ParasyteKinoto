using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PlayerSwordman : PlayerType
{
    // Sword Dash
    [Header("Player Dash")]
    public bool isDashTimerDone;
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;
    public bool isDashing = false;
    public bool canDash;

    // Get Scripts in children
    private MeleeController meleeController;

    // Start is called before the first frame update
    private new void Awake()
    {
        base.Awake();
        meleeController = GetComponentInChildren<MeleeController>();
        isDashTimerDone = true;
    }

    // Update is called once per frame
    private new void Update()
    {
        ManageWallRays(transform.position, facingRight, wallClimbRadius);

        ManageAnimationState();
        if (isDashTimerDone && !isWallClimb) canDash = true;
        else canDash = false;
        moveDirection = playerMovement.ReadValue<Vector2>();
    }

    private new void FixedUpdate()
    {
        if (isWallClimb)
        {
            return;
        }
        // Dash Stats and Stuff
        //Debug.Log("Leak");
        if (isDashing || isWallJump) return;
        MovePlayer();
        transform.localScale = new Vector3(facingRight, transform.localScale.y, transform.localScale.z);
    }

    protected new void OnEnable()
    {
        base.OnEnable();
        
        playerAttack = playerControls.Player.Fire;
        playerAttack.Enable();
        playerAttack.performed += ActivateSwordDash;
    }

    protected new void OnDisable()
    {
        base.OnDisable();

        playerAttack.Disable();
    }

    private void ActivateSwordDash(InputAction.CallbackContext context)
    {
        //Debug.Log("ATTACK");
        if (canDash) StartCoroutine(SwordDash());
    }

    
    private void SetDashRayCast()
    {
        //Debug.DrawRay(transform.position, playerDirection, Color.red);
    }

    private IEnumerator SwordDash()
    {
        isDashing = true;
        isDashTimerDone = false;
        meleeController.SwitchMelee();
        float originalGravity = rb.gravityScale;
        rb.velocity = new Vector2(facingRight * dashingPower, 0);
        currentHorizontalVelocity = facingRight * dashingPower;
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        meleeController.SwitchMelee();
        yield return new WaitForSeconds(dashingCooldown);
        isDashTimerDone = true;
    }

    // Set the health of the player -- so other functions can manipulate player health
    protected new void MovePlayer()
    {
        if (isDashing)
        {
            currentHorizontalVelocity = rb.velocity.x;
            return;
        }
        // CanJump ==== the player is grounded
        // if (canJump) rb.velocity = new Vector2(rb.velocity.x, 0);
        // if wall jump, make current horizontal velocity instant for first frame
        FollowMouse();
        currentHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, moveDirection.x * moveSpeed, decay);
        if (currentHorizontalVelocity < 0.001 && currentHorizontalVelocity > -0.001) currentHorizontalVelocity = 0;
        rb.velocity = new Vector2(currentHorizontalVelocity, rb.velocity.y);
        SetDashRayCast();
        // Change direction based on input
        if (currentHorizontalVelocity > 0) facingRight = 1;
        else if (currentHorizontalVelocity < 0) facingRight = -1;
    }

    // Raycast 
}
