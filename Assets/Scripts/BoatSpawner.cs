using UnityEngine;
using System.Collections;

public class BoatSpawner : MonoBehaviour
{
    public GameObject[] boatPrefabs;
    [Header("生成時間範圍")]
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 4f;
    public float laneOffset = 2f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            SpawnBoat();
        }
    }

    void SpawnBoat()
    {
        // 1. 檢查陣列裡有沒有東西
        if (boatPrefabs.Length == 0) return;

        // 2. 隨機選一個索引（Index）
        int randomIndex = Random.Range(0, boatPrefabs.Length);

        // 3. 隨機位置
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-laneOffset, laneOffset), 0, 0);

        // 4. 產生隨機選中的那台船
        Instantiate(boatPrefabs[randomIndex], spawnPos, transform.rotation);
    }
}
