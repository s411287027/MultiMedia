using UnityEngine;
using System.Collections; // 必須引用這個才能用協程

public class BoatSpawner : MonoBehaviour
{
    public GameObject boatPrefab;

    [Header("生成時間範圍")]
    public float minSpawnTime = 1f; // 最短間隔
    public float maxSpawnTime = 4f; // 最長間隔

    public float laneOffset = 0f;

    void Start()
    {
        // 開始執行協程
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true) // 無限循環
        {
            // 隨機等待一段時間
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // 執行生成
            SpawnBoat();
        }
    }

    void SpawnBoat()
    {
        // 隨機位置
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-laneOffset, laneOffset), 0.2f, 0);

        // 產生船
        Instantiate(boatPrefab, spawnPos, transform.rotation);
    }
}