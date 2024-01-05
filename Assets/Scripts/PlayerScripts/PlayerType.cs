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
    protected SpriteRenderer playerSprite;

    [Header("Player Settings")]
    public float moveSpeed = 10f;
    public float setJumpForce = 50f;
    public float decay = 0.2f;

    [Header("Player Stats")]
    public Vector2 moveDirection = Vector2.right;
    public bool canJump;
    public Vector2 playerDirection;
    public float playerAngle;
    public float currentHorizontalVelocity;
    public float currentVerticalVelocity;
    protected int facingRight = 1;
    [Header("Player Invincibility")]
    protected bool isInvincible;
    protected float incivibilityTime = 1f;

    [Header("Player Health")]
    protected int maxPlayerHealth = 100;
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

    [Header("Player Rotation Stats")]
    public float rotationAngle;

    [Header("Player Animations")]
    [SerializeField] protected Animator playerAnimator;

    public PlayerInputInteractions playerControls;
    internal Rigidbody2D rb;

    // Player Input Actions
    protected InputAction playerMovement;
    protected InputAction playerAttack;
    protected InputAction playerJump;
    protected InputAction playerEject;
    protected InputAction playerPause;

    // States distinguishing player actions
    // isAlive = true
    [SerializeField] protected bool isAlive;

    // Player Audio
    [Header("Player Sounds")]
    public AudioSource audioSource;
    public AudioClip[] jumpAudioClips;

    [Header("Player Death/Spore Spawning")]
    public GameObject playerSpore;
    public GameObject playerCorpse;
    [SerializeField] private Vector2 killingProjectileVelocity;
    

    // Start is called before the first frame update
    protected void Awake()
    {
        playerControls = new PlayerInputInteractions();
        rb = GetComponent<Rigidbody2D>();
        canJump = false;
        isAlive = true;
        isWallHugging = false;
        isInvincible = false;
        killingProjectileVelocity = Vector2.zero;

        // Player gets full health at start of the scene
        maxPlayerHealth = (int)GameStatistics.Instance.playerHealth;
        SetPlayerHealth(maxPlayerHealth);

        // Player Speed
        moveSpeed = GameStatistics.Instance.playerMovementSpeed;
        
        // Player set animator
        playerAnimator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    protected void Update()
    {
        ManageWallRays(transform.position, wallClimbRadius);

        ManageAnimationState();
        // if (isDashTimerDone && !isWallClimb) canDash = true;
        // else canDash = false;
        moveDirection = playerMovement.ReadValue<Vector2>();
    }

    protected void FixedUpdate()
    {
        FollowMouse();
        if (isWallClimb || isWallJump) return;

        MovePlayer();
    }

    protected void OnEnable()
    {
        OnGameControlEnable();

        playerPause = playerControls.Player.Pause;
        playerPause.Enable();
        playerPause.performed += PauseGame;
    }

    public void OnGameControlEnable()
    {
        playerMovement = playerControls.Player.Move;
        playerMovement.Enable();

        playerJump = playerControls.Player.Jump;
        playerJump.Enable();
        playerJump.performed += ActivateJump;
        playerJump.performed += ManageWallJump;
        playerJump.canceled += cancelWallJump;

        playerAttack = playerControls.Player.Fire;
        playerAttack.Enable();
        playerAttack.performed += ActivateAttack;

        playerEject = playerControls.Player.Eject;
        playerEject.Enable();
        playerEject.performed += EjectSpore;
    }

    protected void OnDisable()
    {
        OnGameControlDisable();
        playerPause.Disable();
    }

    public void OnGameControlDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
        playerAttack.Disable();
        playerEject.Disable();
    }

    protected virtual void ActivateAttack(InputAction.CallbackContext context)
    {
    }

    protected virtual void EjectSpore(InputAction.CallbackContext context)
    {
        ActivateDeath();
    }

    protected void MovePlayer()
    {
        currentHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, moveDirection.x * moveSpeed, decay);
        if (currentHorizontalVelocity < 0.001 && currentHorizontalVelocity > -0.001) currentHorizontalVelocity = 0;
        rb.velocity = new Vector2(currentHorizontalVelocity, rb.velocity.y);

        // Change direction based on input
        ChangeSpriteDirection();
        // Oldish?? method to flip sprite
        //if (currentHorizontalVelocity > 0) playerSprite.flipX = false;
        //else if (currentHorizontalVelocity < 0) playerSprite.flipX = true;
    }

    protected void ChangeSpriteDirection()
    {
        if (currentHorizontalVelocity > 0) facingRight = 1;
        else if (currentHorizontalVelocity < 0) facingRight = -1;
        playerSprite.gameObject.transform.localScale = new Vector3(-facingRight, 1, 1);
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
        if (currentPlayerHealth <= 0 && isAlive)
        {
            ActivateDeath();
        }
        else if (isAlive)
        {
            StartCoroutine(StartInvincibleFrames(incivibilityTime));
        }
    }

    private void ActivateDeath()
    {
        isAlive = false;
        CreateSpore();
        CreateCorpse();

        Destroy(gameObject);
    }

    private IEnumerator StartInvincibleFrames(float iTime)
    {
        isInvincible = true;
        playerSprite.color = Color.red;
        yield return new WaitForSeconds(iTime);
        playerSprite.color = Color.white;
        isInvincible = false;
    }

    protected void ActivateJump(InputAction.CallbackContext context)
    {
        if (canJump)
        {
            rb.AddForce(Vector2.up * setJumpForce, ForceMode2D.Impulse);
            //RandomJumpSound();
        }
    }

    protected void CreateSpore()
    {
        GameObject newSpore = Instantiate(playerSpore);
        newSpore.transform.position = transform.position;

        // Handle velocity of new spore (ejection speed)
        Rigidbody2D sporerb = newSpore.GetComponent<Rigidbody2D>();

        sporerb.velocity = (killingProjectileVelocity == Vector2.zero) ? new Vector2(0, 10) : killingProjectileVelocity;
    }

    protected void CreateCorpse()
    {
        GameObject newCorpse = Instantiate(playerCorpse);
        newCorpse.transform.position = transform.position;
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
            Destroy(collider);
            if (!isInvincible)
            {
                DecrementPlayerHealth((int)collider.GetComponent<Projectile>().GetProjectileDamage()); // FIX ME -- make this a variable/constant at the top
                killingProjectileVelocity = collider.GetComponent<Rigidbody2D>().velocity;
                CheckPlayerDeath();
            }
            
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

    protected void cancelWallJump(InputAction.CallbackContext context)
    {
        
    }

    protected void ManageWallRays(Vector2 origin,  float radius)
    {
        //facingRight = (playerSprite.flipX) ? -1 : 1;
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

        //Debug.Log("Player Collision: " + botHit.collider);

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

    protected void PauseGame(InputAction.CallbackContext context)
    {
        MenuManager menu = MenuManager.Instance;

        // Pause game - if not already paused, disable controls -> enable controls otherwise

        if (menu.isPaused)
        {
            menu.ResumeGame();
            OnGameControlEnable();
        }
        else
        {
            menu.PauseGame();
            OnGameControlDisable();
        }
    }

}
