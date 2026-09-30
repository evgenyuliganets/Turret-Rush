using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private Transform target;
        [SerializeField] private Transform enemiesParent;

        [SerializeField] private int enemyCount = 20;

        [SerializeField] private float minX = -20f;
        [SerializeField] private float maxX = 20f;

        [SerializeField] private float minZ = 15f;
        [SerializeField] private float maxZ = 280f;

        private void Start()
        {
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 position = new Vector3(
                    Random.Range(minX, maxX),
                    0f,
                    Random.Range(minZ, maxZ)
                );

                EnemyController enemy = Instantiate(
                    enemyPrefab,
                    position,
                    Quaternion.identity,
                    enemiesParent
                );

                enemy.Initialize(target);
            }
        }
    }
}