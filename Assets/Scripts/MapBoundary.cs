using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在地圖邊緣自動生成隱形牆（透明方塊），防止玩家掉下去。
/// 用法：建立一個空物件掛上此腳本，把「所有」構成地圖的地面物件
/// （例如 Grass Ground、Floor、Road、Road (1)、Road (2)…）拖進 groundObjects。
/// 開始遊戲時會把這些地面的 bounds 全部合併，沿合併後的四邊生成 4 道隱形 BoxCollider 牆。
/// </summary>
public class MapBoundary : MonoBehaviour
{
    [Header("地面物件（可拖入多個，會自動合併成整張地圖範圍）")]
    public GameObject[] groundObjects;    // ⚠️ 把所有地面（草地 + 馬路 + 地板…）通通拖進來！

    [Header("牆設定")]
    public float wallHeight = 5f;         // 牆的高度（要比玩家跳躍高度高，避免跳出去）
    public float wallThickness = 1f;      // 牆的厚度
    public float inset = 0f;              // 往內縮多少（正值讓牆稍微往地圖內，避免邊緣縫隙）

    [Header("除錯")]
    public bool showWalls = false;        // 勾選後牆會顯示成半透明紅色，方便在遊戲中檢查位置

    // 生成出來的牆 Collider 清單，給船（BoatMovement）忽略碰撞用，避免船被牆彈飛
    public static readonly List<Collider> WallColliders = new List<Collider>();

    void Start()
    {
        // 場景重載時清掉舊的（避免 Restart 後殘留已被銷毀的參考）
        WallColliders.Clear();

        if (!TryGetCombinedBounds(out Bounds bounds))
        {
            Debug.LogError("⚠️ MapBoundary：Ground Objects 沒設定（或都沒有 Collider/Renderer）！隱形牆不會生成。");
            return;
        }

        Vector3 center = bounds.center;
        Vector3 size   = bounds.size;

        float halfX = size.x / 2f - inset;
        float halfZ = size.z / 2f - inset;
        // 牆中心的 Y：從地圖底部往上長 wallHeight，確保整片牆都在地面之上
        float wallY = bounds.min.y + wallHeight / 2f;

        // 北 / 南（沿 X 軸延伸的兩道牆，分布在 Z 的兩端）
        CreateWall("Boundary_North", new Vector3(center.x, wallY, center.z + halfZ),
                   new Vector3(size.x, wallHeight, wallThickness));
        CreateWall("Boundary_South", new Vector3(center.x, wallY, center.z - halfZ),
                   new Vector3(size.x, wallHeight, wallThickness));
        // 東 / 西（沿 Z 軸延伸的兩道牆，分布在 X 的兩端）
        CreateWall("Boundary_East", new Vector3(center.x + halfX, wallY, center.z),
                   new Vector3(wallThickness, wallHeight, size.z));
        CreateWall("Boundary_West", new Vector3(center.x - halfX, wallY, center.z),
                   new Vector3(wallThickness, wallHeight, size.z));

        Debug.Log($"MapBoundary：合併 {groundObjects.Length} 塊地面，地圖範圍中心 {center}，大小 {size}");
    }

    /// <summary>把 groundObjects 內所有物件的 bounds 合併成一個大 Bounds。</summary>
    bool TryGetCombinedBounds(out Bounds combined)
    {
        combined = new Bounds();
        bool hasAny = false;

        if (groundObjects == null) return false;

        foreach (GameObject go in groundObjects)
        {
            if (go == null) continue;
            if (!TryGetObjectBounds(go, out Bounds b)) continue;

            if (!hasAny) { combined = b; hasAny = true; }
            else { combined.Encapsulate(b); }
        }

        return hasAny;
    }

    /// <summary>取單一物件的世界 bounds，優先 Collider，其次 Renderer，最後用 Scale 估算。</summary>
    bool TryGetObjectBounds(GameObject go, out Bounds bounds)
    {
        Collider col = go.GetComponent<Collider>();
        if (col != null) { bounds = col.bounds; return true; }

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null) { bounds = rend.bounds; return true; }

        // 沒有 Collider 或 Renderer：用 Transform 估算
        bounds = new Bounds(go.transform.position, go.transform.localScale);
        Debug.LogWarning($"MapBoundary：{go.name} 沒有 Collider 或 Renderer，改用 Scale 估算範圍。");
        return true;
    }

    void CreateWall(string wallName, Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.SetParent(transform);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        // 隱形實體牆：只要 BoxCollider 就能擋住玩家（預設 size 1，靠 localScale 縮放）
        BoxCollider box = wall.AddComponent<BoxCollider>();
        WallColliders.Add(box);   // 登記起來，讓船忽略碰撞

        // 除錯時顯示半透明紅色方塊
        if (showWalls)
        {
            MeshFilter mf = wall.AddComponent<MeshFilter>();
            mf.mesh = BuildCubeMesh();
            MeshRenderer mr = wall.AddComponent<MeshRenderer>();
            // URP 用 Universal Render Pipeline/Lit；若取不到就退回標準 Shader
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = new Color(1f, 0f, 0f, 0.3f);
            mr.material = mat;
        }
    }

    // 用 Unity 內建 Cube primitive 的 mesh，只用於除錯顯示
    private static Mesh _cubeMesh;
    private Mesh BuildCubeMesh()
    {
        if (_cubeMesh != null) return _cubeMesh;
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _cubeMesh = temp.GetComponent<MeshFilter>().sharedMesh;
        Destroy(temp);
        return _cubeMesh;
    }

    // Scene 視窗顯示牆的位置（黃色框），方便在編輯器裡確認有沒有貼合整張地圖
    private void OnDrawGizmosSelected()
    {
        if (!TryGetCombinedBounds(out Bounds bounds)) return;

        Vector3 center = bounds.center;
        Vector3 size   = bounds.size;

        float halfX = size.x / 2f - inset;
        float halfZ = size.z / 2f - inset;
        float wallY = bounds.min.y + wallHeight / 2f;

        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireCube(new Vector3(center.x, wallY, center.z + halfZ), new Vector3(size.x, wallHeight, wallThickness));
        Gizmos.DrawWireCube(new Vector3(center.x, wallY, center.z - halfZ), new Vector3(size.x, wallHeight, wallThickness));
        Gizmos.DrawWireCube(new Vector3(center.x + halfX, wallY, center.z), new Vector3(wallThickness, wallHeight, size.z));
        Gizmos.DrawWireCube(new Vector3(center.x - halfX, wallY, center.z), new Vector3(wallThickness, wallHeight, size.z));
    }
}
