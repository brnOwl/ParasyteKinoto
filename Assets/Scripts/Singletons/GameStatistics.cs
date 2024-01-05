using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatistics : MonoBehaviour
{
    public static GameStatistics Instance;

    // Needed Stats

    [Header("Global Player Statistics")]
    public float playerHealth = 0;
    public float playerMovementSpeed = 0;
    public float playerWallClimbSpeed = 0;
    public float playerDashSpeed = 0;

    [Header("Global Enemy Statistics")]
    public float globalEnemyHealth = 0;

    // Start is called before the first frame update
    void Awake()
    {
        // Prevent multiple gameManagers from existing (keep existing game data)
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    //
}
