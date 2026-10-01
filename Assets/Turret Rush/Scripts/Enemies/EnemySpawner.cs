using Turret_Rush.Scripts.Core;
using UnityEngine;
using System.Collections.Generic;

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

        [SerializeField] private GameManager gameManager;

        [SerializeField] private float despawnDistanceBehind = 20f;

        [SerializeField] private float cleanupInterval = 0.5f;

        [SerializeField] private Transform vfxParent;

        private readonly List<EnemyController> _spawnedEnemies = new();

        private float _nextCleanupTime;

        private void Start()
        {
            SpawnEnemies();
        }


        private void Update()
        {
            if (Time.time < _nextCleanupTime)
                return;

            _nextCleanupTime =
                Time.time + cleanupInterval;

            CleanupEnemiesBehindPlayer();
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

                enemy.Initialize(
                    target,
                    gameManager,
                    vfxParent
                );

                _spawnedEnemies.Add(enemy);
            }
        }


        private void CleanupEnemiesBehindPlayer()
        {
            for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
            {
                EnemyController enemy =
                    _spawnedEnemies[i];

                if (!enemy)
                {
                    _spawnedEnemies.RemoveAt(i);
                    continue;
                }

                Vector3 localPosition =
                    target.InverseTransformPoint(
                        enemy.transform.position
                    );

                if (localPosition.z >= -despawnDistanceBehind)
                    continue;

                Destroy(enemy.gameObject);

                _spawnedEnemies.RemoveAt(i);
            }
        }
    }
}