using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] obstaclePrefabs, powerUpPrefabs;
    GameManager gameManager;
    // Spawn Values
    readonly float xPos = 13, zLowPos = -7, zHighPos = 13;
    float xRange, zRange;
    float xConsumeRange, zConsumeRange;
    int posIndex, obstacleIndex;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        InvokeRepeating(nameof(SpawnObstacle), 1, 3);
        InvokeRepeating(nameof(SpawnConsumable), 1, 5);
    }
    // Spawn an Obstacle at a random position and rotation
    void SpawnObstacle() {
        if (gameManager.GameActive)
        {
            obstacleIndex = Random.Range(0, obstaclePrefabs.Length);

            xRange = Random.Range(-xPos, xPos);
            zRange = Random.Range(zLowPos, zHighPos);

            Instantiate(obstaclePrefabs[obstacleIndex], PickSpawnPos(), PickSpawnRotation());
        }
    }
    // Spawn a consumable at a random position
    void SpawnConsumable() {
        if (gameManager.GameActive && GameObject.FindGameObjectWithTag("Consumable") == null)
        {
            int index = Random.Range(0, powerUpPrefabs.Length);
            float yOffSet = (index == 0) ? 0.4f : 1;
            xConsumeRange = Random.Range(-xPos / 2, xPos / 2);
            zConsumeRange = Random.Range(zLowPos / 2, zHighPos / 2);
            Vector3 consumeArea = new(xConsumeRange, yOffSet, zConsumeRange);

            Instantiate(powerUpPrefabs[index], consumeArea, 
                powerUpPrefabs[index].transform.rotation);
        }
    }
    /* Pick a spawn position for the obstacle as follows: 
     * {top, bottom, left, right, top-left, top-right, bottom-left, bottom-right}
     */
    Vector3 PickSpawnPos() {
        float yOffSet = (obstacleIndex == 3)? 0.75f: 0;
        Vector3[] spawnPositions = {
            new Vector3(xRange, yOffSet, zHighPos), 
            new Vector3(xRange, yOffSet, zLowPos),
            new Vector3(-xPos, yOffSet, zRange), 
            new Vector3(xPos, yOffSet, zRange),
            new Vector3(-xPos, yOffSet, zHighPos),
            new Vector3(xPos, yOffSet, zHighPos),
            new Vector3(-xPos, yOffSet, zLowPos),
            new Vector3(xPos, yOffSet, zHighPos)
        };
        posIndex = Random.Range(0, spawnPositions.Length);
        return spawnPositions[posIndex];
    }
    /* Pick a spawn rotation for the obstacle as follows: 
     * {top, bottom, left, right, top-left, top-right, bottom-left, bottom-right}
     */
    Quaternion PickSpawnRotation() {
        Quaternion[] spawnRotations = {
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, -90, 0),
            Quaternion.Euler(0, 135, 0),
            Quaternion.Euler(0, -135, 0),
            Quaternion.Euler(0, 45, 0),
            Quaternion.Euler(0, -45, 0)
        };
        return spawnRotations[posIndex];
    }
}
