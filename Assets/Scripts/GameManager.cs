using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public Transform playerSpawnPoint;
    //private 

    //public HealthBar playerHealthBar;
    //public StaminaBar playerStaminaBar;

    public GameObject explosionObject;

    [Header("Game Over")]
    public float gameOverTime = 3.0f;
    public GameObject gameOverScreen;
    //public Image gameOverScreen;

    [Header("Enemy Count")]
    public int getEnemyCount;
    public int setEnemyCount;

    [Header("Enemy Prefab and Spawnpoint")]
    public GameObject enemy;
    public Transform enemySpawnPoint;

    [Header("Inventory")]
    public GameObject inventory;
    public GameObject hotbar;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SpawnPlayer(playerSpawnPoint);
        SpawnEnemy(enemySpawnPoint);
    }

    private void Awake()
    {
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer(Transform spawnPoint)
    {
        GameObject newPlayer = Instantiate(player);
        newPlayer.transform.position = spawnPoint.position;
    }

    public void SpawnEnemy(Transform spawnPoint)
    {
        GameObject newEnemy = Instantiate(enemy);
        newEnemy.transform.position = spawnPoint.position;
    }

    // Loads a scene based on the scene Index -- can be called by other scripts
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void PlayerDeathControl(Transform newTransform)
    {
        //Explode(newTransform);
        StartCoroutine(GameOver());
    }

    public void Explode(Transform newTransform)
    {
        GameObject newExplosion = Instantiate(explosionObject);
        //StartCoroutine(newExplosion.GetComponent<ExplosionController>().Explode(newTransform)); FIXME
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        gameOverScreen.SetActive(true);
        yield return new WaitForSeconds(gameOverTime);
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TakeDamage(float damage, GameObject target)
    {
        if (target.tag == "PlayerBody")
        {
            //playerController.playerTakeDamage(damage); FIXME
        }
        else if (target.tag == "EnemyBody")
        {
            //EnemyController enemyController = target.GetComponentInParent<EnemyController>(); FIXME
            //enemyController.EnemyTakeDamage(damage); FIXME
        }

    }
}
