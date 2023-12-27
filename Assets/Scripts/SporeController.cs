using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ClassTyping;

public class SporeController : MonoBehaviour
{
    GameManager gameManager = GameManager.Instance;

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
    float currentHorizontalVelocity;
    float currentVerticalVelocity;

    // Player Regeneration
    public GameObject playerList;
    bool isAlive;

    private void Awake()
    {
        playerControls = new PlayerInputInteractions();
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHorizontalVelocity = rb.velocity.x;
        currentVerticalVelocity = rb.velocity.y;
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
        Debug.Log("Spore collided with: " + collision.transform.tag);

        if (collision.transform.tag == "Enemy")
        {
            ClassType enemyType = collision.gameObject.GetComponent<EnemyType>().enemyTypeEnum;
            Debug.Log("Enemy Type: " + enemyType + " : " + (int)enemyType);

            if (isAlive)
            {
                Destroy(collision.gameObject);
                //
                GameObject newPlayer = Instantiate(gameManager.playerTypeList[(int)enemyType]);
                //GameObject newPlayer = Instantiate(playerList.transform.GetChild(1).gameObject);
                newPlayer.transform.position = collision.transform.position;
                isAlive = false;

                Destroy(gameObject);
                
            }
            
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Spore triggered with: " + collision.transform.tag);

        if (collision.transform.tag == "EnemyProjectile")
        {
            GameManager.Instance.PlayerDeathControl(transform);
            Destroy(gameObject);
        }
    }

    public Vector2 GetProjectileVelocity(Vector2 velocity) 
    {
        return velocity;
    }
}
