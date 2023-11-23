using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float setJumpForce = 50f;
    // Sword Dash
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;
    public float decay = 0.1f;

    [Header("Player Stats")]
    public Vector2 moveDirection = Vector2.right;
    public bool canJump;
    public Vector2 playerDirection;
    public float playerAngle;
    public bool isDashing = false;
    public bool canDash;
    public float currentHorizontalVelocity;
    public float currentVerticalVelocity;
    private int facingRight = 1;

    [Header("Player Dash")]
    public bool isDashTimerDone;

    [Header("Player Health")]
    public int maxPlayerHealth = 100;
    [SerializeField] private int currentPlayerHealth;

    [Header("Player Wall Climbing")]
    [SerializeField] private bool isWallHugging;
    [SerializeField] private bool canWallClimb;
    [SerializeField] private bool isWallClimb;
    [SerializeField] private Vector2 climbPoint;
    [SerializeField] private bool isWallJump;
    [SerializeField] private bool canWallJump;
    public float wallClimbSpeed = 100f;
    public float wallClimbRadius = 1;

    [Header("Player Animations")]
    [SerializeField] private Animator playerAnimator;

    // Get Scripts in children
    private MeleeController meleeController;

    public PlayerInputInteractions playerControls;
    internal Rigidbody2D rb;

    // Player Input Actions
    private InputAction playerMovement;
    private InputAction playerAttack;
    private InputAction playerJump;

    // States distinguishing player actions
    // isAlive = true
    [SerializeField] private bool isAlive;



    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerInputInteractions();
        rb = GetComponent<Rigidbody2D>();
        canJump = false;
        isAlive = true;
        isWallHugging = false;
        meleeController = GetComponentInChildren<MeleeController>();
        // Player gets full health at start of the scene
        SetPlayerHealth(maxPlayerHealth);
        isDashTimerDone = true;
        // Player set animator
        playerAnimator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        playerMovement = playerControls.Player.Move;
        playerMovement.Enable();

        playerAttack = playerControls.Player.Fire;
        playerAttack.Enable();
        playerAttack.performed += ActivateSwordDash;

        playerJump = playerControls.Player.Jump;
        playerJump.Enable();
        playerJump.performed += ActivateJump;
        playerJump.performed += ManageWallJump;
    }

    private void OnDisable()
    {
        playerMovement.Disable();
        playerAttack.Disable();
        playerJump.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        ManageWallRays(transform.position, facingRight, wallClimbRadius);

        ManageAnimationState();
        if (isDashTimerDone && !isWallClimb) canDash = true;
        else canDash = false;
        moveDirection = playerMovement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
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

    private void MovePlayer()
    {
        if (isDashing)
        {
            currentHorizontalVelocity = rb.velocity.x;
            return;
        }
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

    private void ActivateJump(InputAction.CallbackContext context)
    {
        if (canJump)
        {
            rb.AddForce(Vector2.up * setJumpForce, ForceMode2D.Impulse);
        }
    }

    private void ActivateSwordDash(InputAction.CallbackContext context)
    {
        //Debug.Log("ATTACK");
        if (canDash) StartCoroutine(SwordDash());
    }

    private void FollowMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 relativeMousePos = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
        playerAngle = Mathf.Atan2(relativeMousePos.y, relativeMousePos.x);
        playerDirection = new Vector2(Mathf.Cos(playerAngle), Mathf.Sin(playerAngle));
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
    private void SetPlayerHealth(int newPlayerHealth)
    {
        currentPlayerHealth = newPlayerHealth;
    }

    private void DecrementPlayerHealth(int decPlayerHealth)
    {
        currentPlayerHealth -= decPlayerHealth;
    }


    private void CheckPlayerDeath()
    {
        if (currentPlayerHealth <= 0)
        {
            isAlive = false;
            Destroy(gameObject);
        }
    }

    // Wall jump and climb Handler

    public void SetCanWallClimb(bool status)
    {
        canWallClimb = status;
    }

    public void SetCanWallJump(bool status)
    {
        canWallJump = status;
    }

    private void ManageWallJump(InputAction.CallbackContext context)
    {
        if (isWallClimb)
        {
            isWallClimb = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(Vector2.up * setJumpForce, ForceMode2D.Impulse);
            currentHorizontalVelocity = -setJumpForce / 25 * facingRight;   
        }
        else if (canWallClimb && !canJump)
        {
            isWallClimb = true;
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    // Collision Handler:
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log(collider.gameObject.tag);

        if (collider.gameObject.tag == "EnemyProjectile")
        {
            DecrementPlayerHealth(10); // FIX ME -- make this a variable/constant at the top
            CheckPlayerDeath();
        }
    }

    private void ManageAnimationState()
    {
        // moveDirection == the x input from the player
        if (Mathf.Abs(moveDirection.x) > 0.1) playerAnimator.Play("PlayerWalk");
        else playerAnimator.Play("PlayerIdle");
    }

    // Raycast Handler
    private void ManageWallRays(Vector2 origin, int facingRight, float radius)
    {
        Vector2 angleDirection = new Vector2(facingRight, 0);


        // Top and bottom locations for raycast
        Vector2 topRayOrigin = new Vector2(origin.x, origin.y + (float)(radius / 1.5));
        Vector2 botRayOrigin = new Vector2(origin.x, origin.y - radius / 2);

        RaycastHit2D topHit = Physics2D.Raycast(topRayOrigin, angleDirection, (radius), LayerMask.GetMask("Ground"));
        RaycastHit2D botHit = Physics2D.Raycast(botRayOrigin, angleDirection, (radius), LayerMask.GetMask("Ground"));
        RaycastHit2D downHit = Physics2D.Raycast(botRayOrigin, Vector2.down, (radius), LayerMask.GetMask("Ground"));
        RaycastHit2D upHit = Physics2D.Raycast(topRayOrigin, Vector2.up, (radius), LayerMask.GetMask("Ground"));


        // Debug by drawing in editor view
        Debug.DrawRay(topRayOrigin, angleDirection * radius, Color.green);
        Debug.DrawRay(botRayOrigin, angleDirection * radius, Color.green);
        Debug.DrawRay(botRayOrigin, Vector2.down * radius, Color.green);
        Debug.DrawRay(topRayOrigin, Vector2.up* radius, Color.green);

        Debug.Log("Collision: " + botHit.collider);

        canWallClimb = (topHit.collider != null && botHit.collider != null) ? true : false;

        if (isWallClimb)
        {
            if (
                (topHit.collider != null && moveDirection.y > 0 && upHit.collider == null) ||
                (botHit.collider != null && moveDirection.y < 0 && downHit.collider == null)
                )
            {
                rb.velocity = new Vector2(0, moveDirection.y) * wallClimbSpeed;
            } else
            {
                rb.velocity = Vector2.zero;
            }
            isWallClimb = (downHit.collider == null);
        }
    }




}
