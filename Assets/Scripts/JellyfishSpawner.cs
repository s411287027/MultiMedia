using UnityEngine;

public class JellyfishSpawner : MonoBehaviour
{
    public GameObject jellyfishPrefab;
    public int spawnCount = 15;            // 水母數量，Inspector 可調整
    public GameObject grassGround;         // ⚠️ 把 Hierarchy 的 Grass Ground 拖進來！

    private Vector3 spawnCenter;
    private Vector3 spawnAreaSize;

    void Start()
    {
        if (jellyfishPrefab == null)
        {
            Debug.LogError("水母 Prefab 沒設定！");
            return;
        }

        if (grassGround == null)
        {
            Debug.LogError("⚠️ Grass Ground 沒有拖入！水母將不會生成。");
            return;
        }

        // 用 Collider.bounds 最準確（包含子物件的整體範圍）
        Collider col = grassGround.GetComponent<Collider>();
        if (col != null)
        {
            spawnCenter   = col.bounds.center;
            spawnAreaSize = col.bounds.size;
        }
        else
        {
            // 沒有 Collider，改用 Renderer
            Renderer rend = grassGround.GetComponent<Renderer>();
            if (rend != null)
            {
                spawnCenter   = rend.bounds.center;
                spawnAreaSize = rend.bounds.size;
            }
            else
            {
                // 最後備援：用 Transform Scale
                spawnCenter   = grassGround.transform.position;
                spawnAreaSize = grassGround.transform.localScale;
                Debug.LogWarning("Grass Ground 沒有 Collider 或 Renderer，改用 Scale 估算範圍。");
            }
        }

        // Y 軸不需要，生成在水平面
        spawnAreaSize.y = 0f;

        Debug.Log($"草地中心：{spawnCenter}　生成範圍：{spawnAreaSize}");

        SpawnJellyfish();
    }

    void SpawnJellyfish()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            float rx = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float rz = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);

            Vector3 spawnPos = new Vector3(
                spawnCenter.x + rx,
                spawnCenter.y + 0.5f,   // 稍微浮在草地上方
                spawnCenter.z + rz
            );

            GameObject obj = Instantiate(jellyfishPrefab, spawnPos, Quaternion.identity);
            Jellyfish script = obj.GetComponent<Jellyfish>();
            if (script != null)
                script.Initialize(spawnCenter, spawnAreaSize);
        }
    }

    // Scene 視窗顯示綠色生成框
    private void OnDrawGizmosSelected()
    {
        if (grassGround == null) return;

        Vector3 c, s;
        Collider col = grassGround.GetComponent<Collider>();
        if (col != null)
        {
            c = col.bounds.center;
            s = col.bounds.size;
        }
        else
        {
            Renderer rend = grassGround.GetComponent<Renderer>();
            if (rend != null) { c = rend.bounds.center; s = rend.bounds.size; }
            else { c = grassGround.transform.position; s = grassGround.transform.localScale; }
        }

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(c, s);
    }
}