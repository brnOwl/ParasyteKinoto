using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace ClassTyping
{
    public enum ClassType
    {
        Archer,
        Swordman,
        Chemist
    }

    public enum EntityOwner
    {
        Player,
        Enemy
    }
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public Transform playerSpawnPoint;
    [SerializeField] GameObject currentPlayer;

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
    public List<GameObject> enemyTypeList;
    public List<GameObject> playerTypeList;
    //public List<GameObject> ;
    public List<Transform> spawnPointList;
    //public List<Transform> ;
    

    [Header("Inventory")]
    public GameObject inventory;
    public GameObject hotbar;

    [Header("Enemy List in Scene")]
    [SerializeField] GameObject enemyListInScene;

    private void Awake()
    {
        // Prevent multiple gameManagers from existing (keep existing game data)
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Data persistance
        Instance = this;
        DontDestroyOnLoad(this);

        SpawnPlayer(playerSpawnPoint);

        SpawnEnemyListInScene();
        SpawnEnemyList();

        gameOverScreen.SetActive(false);

        

    }

    // Update is called once per frame
    void Update()
    {
        currentPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    public void SpawnPlayer(Transform spawnPoint)
    {
        GameObject newPlayer = Instantiate(player);
        newPlayer.transform.position = spawnPoint.position;
    }

    public void SpawnEnemy(Transform spawnPoint, GameObject enemy)
    {
        GameObject newEnemy = Instantiate(enemy);
        newEnemy.transform.position = spawnPoint.position;
    }

    public void SpawnEnemyList()
    {
        foreach (Transform point in spawnPointList)
        {
            int index = Random.Range(0, enemyTypeList.Count);
            GameObject newEnemy = Instantiate(enemyTypeList[index], enemyListInScene.transform);
            newEnemy.transform.position = point.position;
        }
        
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

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void SpawnEnemyListInScene()
    {
        enemyListInScene = Instantiate(enemyListInScene);
    }

}
