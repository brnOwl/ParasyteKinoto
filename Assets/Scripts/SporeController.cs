using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SporeController : MonoBehaviour
{
    public PlayerInputInteractions playerControls;
    internal Rigidbody2D rb;

    // Player Spore Input Actions
    private InputAction playerMovement;
    private InputAction playerAttack;
    private InputAction playerJump;

    // Player Spore Movement
    public Vector2 moveDirection = Vector2.right;
    public float moveSpeed = 10f;
    public float decay = 5f;
    float currentHorizontalVelocity = 0f;
    float currentVerticalVelocity = 0f;

    // Player Regeneration
    public GameObject player;


    private void Awake()
    {
        playerControls = new PlayerInputInteractions();
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = playerMovement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        MovePlayerSpore();
    }

    private void OnEnable()
    {
        playerMovement = playerControls.Player.Move;
        playerMovement.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
    }

    private void MovePlayerSpore()
    {
        currentHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, moveDirection.x * moveSpeed, decay);
        currentVerticalVelocity = Mathf.Lerp(currentVerticalVelocity, moveDirection.y * moveSpeed, decay);
        if (currentHorizontalVelocity < 0.001 && currentHorizontalVelocity > -0.001) currentHorizontalVelocity = 0;
        if (currentVerticalVelocity < 0.001 && currentVerticalVelocity > -0.001) currentVerticalVelocity = 0;
        rb.velocity = new Vector2(currentHorizontalVelocity, currentVerticalVelocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            GameObject newPlayer = Instantiate(player);
            newPlayer.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
