using UnityEngine;

public class JellyfishSpawner : MonoBehaviour
{
    public GameObject jellyfishPrefab;
    public int spawnCount = 10;
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);

    // 🟢 生成邏輯只允許放在 Start 裡面！
    void Start()
    {
        // 加強防呆：檢查有沒有放 Prefab，沒放就直接跳出，避免出錯
        if (jellyfishPrefab == null)
        {
            Debug.LogError("水母的 Prefab 沒有設定！請在 Inspector 中拖曳進去！");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);
            
            Vector3 spawnPos = transform.position + new Vector3(randomX, 0.5f, randomZ); 
            GameObject newJellyfish = Instantiate(jellyfishPrefab, spawnPos, Quaternion.identity);

            Jellyfish jellyfishScript = newJellyfish.GetComponent<Jellyfish>();
            if (jellyfishScript != null)
            {
                jellyfishScript.Initialize(transform.position, spawnAreaSize);
            }
        }
    }

    // 🔴 畫輔助線的地方，絕對不可以有 Instantiate 或 for 迴圈！
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, spawnAreaSize);
    }
}