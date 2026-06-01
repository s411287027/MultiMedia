using UnityEngine;

/// <summary>
/// 在地圖邊緣自動生成隱形牆（透明方塊），防止玩家掉下去。
/// 用法：建立一個空物件掛上此腳本，把地面（例如 Grass Ground）拖進 groundObject。
/// 開始遊戲時會沿著地面 bounds 的四邊生成 4 道隱形 BoxCollider 牆。
/// </summary>
public class MapBoundary : MonoBehaviour
{
    [Header("地面物件")]
    public GameObject groundObject;       // ⚠️ 把要包圍的地面（如 Grass Ground）拖進來！

    [Header("牆設定")]
    public float wallHeight = 5f;         // 牆的高度（要比玩家跳躍高度高，避免跳出去）
    public float wallThickness = 1f;      // 牆的厚度
    public float inset = 0f;              // 往內縮多少（正值讓牆稍微往地圖內，避免邊緣縫隙）

    [Header("除錯")]
    public bool showWalls = false;        // 勾選後牆會顯示成半透明紅色，方便在遊戲中檢查位置

    void Start()
    {
        if (groundObject == null)
        {
            Debug.LogError("⚠️ MapBoundary：Ground Object 沒有拖入！隱形牆不會生成。");
            return;
        }

        // 用 Collider.bounds 最準確（包含子物件的整體範圍）
        Vector3 center, size;
        Collider col = groundObject.GetComponent<Collider>();
        if (col != null)
        {
            center = col.bounds.center;
            size   = col.bounds.size;
        }
        else
        {
            Renderer rend = groundObject.GetComponent<Renderer>();
            if (rend != null)
            {
                center = rend.bounds.center;
                size   = rend.bounds.size;
            }
            else
            {
                center = groundObject.transform.position;
                size   = groundObject.transform.localScale;
                Debug.LogWarning("MapBoundary：Ground Object 沒有 Collider 或 Renderer，改用 Scale 估算範圍。");
            }
        }

        float halfX = size.x / 2f - inset;
        float halfZ = size.z / 2f - inset;
        // 牆中心的 Y：站在地面上方，往上長 wallHeight
        float wallY = center.y + wallHeight / 2f;

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
    }

    void CreateWall(string wallName, Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.SetParent(transform);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        // 隱形實體牆：只要 BoxCollider 就能擋住玩家（預設 size 1，靠 localScale 縮放）
        wall.AddComponent<BoxCollider>();

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

    // Scene 視窗顯示牆的位置（黃色框）
    private void OnDrawGizmosSelected()
    {
        if (groundObject == null) return;

        Vector3 center, size;
        Collider col = groundObject.GetComponent<Collider>();
        if (col != null) { center = col.bounds.center; size = col.bounds.size; }
        else
        {
            Renderer rend = groundObject.GetComponent<Renderer>();
            if (rend != null) { center = rend.bounds.center; size = rend.bounds.size; }
            else { center = groundObject.transform.position; size = groundObject.transform.localScale; }
        }

        float halfX = size.x / 2f - inset;
        float halfZ = size.z / 2f - inset;
        float wallY = center.y + wallHeight / 2f;

        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireCube(new Vector3(center.x, wallY, center.z + halfZ), new Vector3(size.x, wallHeight, wallThickness));
        Gizmos.DrawWireCube(new Vector3(center.x, wallY, center.z - halfZ), new Vector3(size.x, wallHeight, wallThickness));
        Gizmos.DrawWireCube(new Vector3(center.x + halfX, wallY, center.z), new Vector3(wallThickness, wallHeight, size.z));
        Gizmos.DrawWireCube(new Vector3(center.x - halfX, wallY, center.z), new Vector3(wallThickness, wallHeight, size.z));
    }
}
