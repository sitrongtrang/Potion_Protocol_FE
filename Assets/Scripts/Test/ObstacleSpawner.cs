using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public BasicObstacle obstaclePrefab;
    public int numberOfObstacles = 10;
    public Rect spawnArea = new Rect(0, 0, 1920, 1080);

    private void Start()
    {
        SpawnObstacles();
    }

    private void SpawnObstacles()
    {
        for (int i = 0; i < numberOfObstacles; i++)
        {
            Vector2 position = new Vector2(
                Random.Range(spawnArea.xMin, spawnArea.xMax),
                Random.Range(spawnArea.yMin, spawnArea.yMax)
            );

            BasicObstacle obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
            obstacle.transform.SetParent(transform);
        }
    }
}