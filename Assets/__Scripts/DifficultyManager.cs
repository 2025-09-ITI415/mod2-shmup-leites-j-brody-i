using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // Singleton pattern to make it easily accessible
    static public DifficultyManager S;

    [Header("Difficulty Scaling")]
    [Tooltip("The number of enemy kills required to increase the spawn rate.")]
    public int killsForSpawnRateIncrease = 10;

    [Tooltip("How much to increase the enemySpawnPerSecond by.")]
    public float spawnRateIncreaseAmount = 0.1f;

    [Tooltip("The maximum spawn rate (e.g., 5.0 enemies per second). 0 means no limit.")]
    public float maxSpawnRate = 5.0f;

    // Private counter for destroyed enemies
    private int killCount = 0;

    // Reference to the Main script
    private Main mainScript;

    void Awake()
    {
        // Set up the singleton
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("DifficultyManager.Awake() - Attempted to create a second instance.");
            Destroy(this.gameObject);
            return;
        }

        // Find the Main script in the scene so we can modify its spawn rate
        mainScript = FindObjectOfType<Main>();
        if (mainScript == null)
        {
            Debug.LogError("DifficultyManager.Awake() - Could not find Main script in scene!");
        }
    }

    /// <summary>
    /// This method is called by Main whenever an enemy is destroyed.
    /// It increments the kill count and checks if the difficulty should be increased.
    /// </summary>
    public void HandleEnemyDestroyed()
    {
        // If we couldn't find the Main script, don't do anything
        if (mainScript == null) return;

        killCount++;

        // Check if the kill count is a multiple of the threshold
        // We use the modulo (%) operator for this.
        // We also check that killsForSpawnRateIncrease > 0 to avoid a divide-by-zero error.
        if (killsForSpawnRateIncrease > 0 && killCount % killsForSpawnRateIncrease == 0)
        {
            // Get the current spawn rate from Main
            float currentSpawnRate = mainScript.enemySpawnPerSecond;

            // Calculate the new spawn rate
            float newSpawnRate = currentSpawnRate + spawnRateIncreaseAmount;

            // Enforce the maximum spawn rate, if one is set (maxSpawnRate > 0)
            if (maxSpawnRate > 0 && newSpawnRate > maxSpawnRate)
            {
                newSpawnRate = maxSpawnRate;
            }

            // Only update if the spawn rate has actually changed (i.e., we haven't hit the cap)
            if (newSpawnRate > currentSpawnRate)
            {
                // Set the new spawn rate back on the Main script
                mainScript.enemySpawnPerSecond = newSpawnRate;
                Debug.Log("Spawn rate increased to: " + newSpawnRate);
            }
        }
    }
}
