using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PlayerType : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float setJumpForce = 50f;

    [Header("Player Stats")]
    public Vector2 moveDirection = Vector2.right;
    public bool canJump;
    public Vector2 playerDirection;
    public float playerAngle;
    public float currentHorizontalVelocity;
    public float currentVerticalVelocity;
    protected int facingRight = 1;

    [Header("Player Health")]
    public int maxPlayerHealth = 100;
    [SerializeField] protected int currentPlayerHealth;

    [Header("Player Wall Climbing")]
    [SerializeField] protected bool isWallHugging;
    [SerializeField] protected bool canWallClimb;
    [SerializeField] protected bool isWallClimb;
    [SerializeField] protected Vector2 climbPoint;
    [SerializeField] protected bool isWallJump;
    [SerializeField] protected bool canWallJump;
    public float wallClimbSpeed = 100f;
    public float wallClimbRadius = 1;

    [Header("Player Animations")]
    [SerializeField] protected Animator playerAnimator;

    public PlayerInputInteractions playerControls;
    internal Rigidbody2D rb;

    // Player Input Actions
    protected InputAction playerMovement;
    protected InputAction playerAttack;
    protected InputAction playerJump;

    // States distinguishing player actions
    // isAlive = true
    [SerializeField] protected bool isAlive;

    // Player Audio
    [Header("Player Sounds")]
    public AudioSource audioSource;
    public AudioClip[] jumpAudioClips;

    [Header("Player Death/Spore Spawning")]
    public GameObject sporeObject;

    // Start is called before the first frame update
    protected void Awake()
    {
        playerControls = new PlayerInputInteractions();
        rb = GetComponent<Rigidbody2D>();
        canJump = false;
        isAlive = true;
        isWallHugging = false;
        
        // Player gets full health at start of the scene
        SetPlayerHealth(maxPlayerHealth);
        
        // Player set animator
        playerAnimator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected void Update()
    {
        ManageWallRays(transform.position, facingRight, wallClimbRadius);

        ManageAnimationState();
        // if (isDashTimerDone && !isWallClimb) canDash = true;
        // else canDash = false;
        moveDirection = playerMovement.ReadValue<Vector2>();
    }

    protected void FixedUpdate()
    {
        if (isWallClimb)
        {
            return;
        }
        // Dash Stats and Stuff
        //Debug.Log("Leak");
        if (isWallJump) return;
        MovePlayer();
        transform.localScale = new Vector3(facingRight, transform.localScale.y, transform.localScale.z);
    }

    protected void OnEnable()
    {
        playerMovement = playerControls.Player.Move;
        playerMovement.Enable();

        playerJump = playerControls.Player.Jump;
        playerJump.Enable();
        playerJump.performed += ActivateJump;
        playerJump.performed += ManageWallJump;
    }

    protected void OnDisable()
    {
        playerMovement.Disable();
        playerAttack.Disable();
        playerJump.Disable();
    }

    protected void MovePlayer()
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

    protected void FollowMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 relativeMousePos = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
        playerAngle = Mathf.Atan2(relativeMousePos.y, relativeMousePos.x);
        playerDirection = new Vector2(Mathf.Cos(playerAngle), Mathf.Sin(playerAngle));
    }

    protected void SetPlayerHealth(int newPlayerHealth)
    {
        currentPlayerHealth = newPlayerHealth;
    }

    protected void DecrementPlayerHealth(int decPlayerHealth)
    {
        currentPlayerHealth -= decPlayerHealth;
    }


    protected void CheckPlayerDeath()
    {
        if (currentPlayerHealth <= 0)
        {
            isAlive = false;
            CreateSpore();

            Destroy(gameObject);
        }
    }

    protected void ActivateJump(InputAction.CallbackContext context)
    {
        if (canJump)
        {
            rb.AddForce(Vector2.up * setJumpForce, ForceMode2D.Impulse);
            RandomJumpSound();
        }
    }

    protected void CreateSpore()
    {
        GameObject newSpore = Instantiate(sporeObject);
        newSpore.transform.position = transform.position;
    }

    protected void ManageAnimationState()
    {
        // moveDirection == the x input from the player
        if (Mathf.Abs(moveDirection.x) > 0.1) playerAnimator.Play("PlayerWalk");
        else playerAnimator.Play("PlayerIdle");
    }

    // Collision Handler:
    protected void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log(collider.gameObject.tag);

        if (collider.gameObject.tag == "EnemyProjectile")
        {
            DecrementPlayerHealth((int)collider.GetComponent<EnemyProjectile>().GetProjectileDamage()); // FIX ME -- make this a variable/constant at the top
            CheckPlayerDeath();
        }
    }

    // Wall Jump
    protected void ManageWallJump(InputAction.CallbackContext context)
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

    protected void ManageWallRays(Vector2 origin, int facingRight, float radius)
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
        Debug.DrawRay(topRayOrigin, Vector2.up * radius, Color.green);

        Debug.Log("Player Collision: " + botHit.collider);

        canWallClimb = (topHit.collider != null && botHit.collider != null) ? true : false;

        if (isWallClimb)
        {
            if
                (
                (topHit.collider != null && moveDirection.y > 0 && upHit.collider == null) ||
                (botHit.collider != null && moveDirection.y < 0)
                )
            {
                rb.velocity = new Vector2(0, moveDirection.y) * wallClimbSpeed;
            }
            else
            {
                // If the player is grounded, cancel the wall climb

                rb.velocity = Vector2.zero;
            }
            if (downHit.collider != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isWallClimb = false;
            }

        }


    }

    public void SetCanWallClimb(bool status)
    {
        canWallClimb = status;
    }

    public void SetCanWallJump(bool status)
    {
        canWallJump = status;
    }



    // Handle Sounds
    protected void RandomJumpSound()
    {
        audioSource.clip = jumpAudioClips[Random.Range(0, jumpAudioClips.Length)];
        audioSource.Play();
    }
}
