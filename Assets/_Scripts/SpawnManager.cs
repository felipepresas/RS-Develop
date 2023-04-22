using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
   public GameObject enemyPrefab;
    public Transform platformTransform;

    private float spawnRange = 50f;
    private float platformLimitX = 50f;
    private float platformLimitY = 14f;
    private float platformLimitZ = 50f;

    void Start()
    {
        GameObject enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
        Invoke("ChangeEnemyPosition", 1f);
    }

    void ChangeEnemyPosition()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            Vector3 newPos = GetSpawnPosition();
            enemy.transform.SetPositionAndRotation(newPos, Quaternion.identity);
            Invoke("ChangeEnemyPosition", 20f);
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(
            enemyPrefab,
            platformTransform.position,
            Quaternion.identity
        );
        enemy.transform.position = GetSpawnPosition();
    }

Vector3 GetSpawnPosition()
{
    float spawnPosX = Random.Range(-spawnRange, spawnRange);
    float spawnPosZ = Random.Range(-spawnRange, spawnRange);

    // Asegurarse de que la posición generada esté dentro de los límites de la plataforma
    spawnPosX = Mathf.Clamp(spawnPosX, -platformLimitX, platformLimitX);
    spawnPosZ = Mathf.Clamp(spawnPosZ, -platformLimitZ, platformLimitZ);

    Vector3 randomPos = new Vector3(spawnPosX, platformTransform.position.y + 1f, spawnPosZ);

    // Asegurarse de que la posición generada esté sobre la plataforma
    randomPos.y = Mathf.Clamp(randomPos.y, platformTransform.position.y, platformTransform.position.y + platformLimitY);

    return randomPos;
}
}
