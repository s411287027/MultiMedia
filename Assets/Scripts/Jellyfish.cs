using UnityEngine;

public class Jellyfish : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 1.5f;          // 水平漂移速度
    public float verticalSpeed = 0.6f;      // 上下漂浮速度

    [Header("換方向時間")]
    public float changeTargetTime = 3f;     // 幾秒換一次目標，Inspector 可調整

    [Header("上下漂浮範圍")]
    public float minY = 0.5f;              // 最低高度（草地上方）
    public float maxY = 3.0f;             // 最高高度

    private Vector3 targetPosition;
    private float timer;
    private Vector3 moveAreaCenter;
    private Vector3 moveAreaSize;

    public void Initialize(Vector3 center, Vector3 size)
    {
        moveAreaCenter = center;
        moveAreaSize   = size;
        PickNewTarget();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeTargetTime || Vector3.Distance(transform.position, targetPosition) < 0.4f)
        {
            PickNewTarget();
            timer = 0f;
        }

        // 往目標平滑移動（X Y Z 全方向）
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 水平速度
        Vector3 horizontal = new Vector3(direction.x, 0f, direction.z) * moveSpeed;
        // 垂直速度（Y 方向單獨控速，模擬水母緩慢上下）
        Vector3 vertical   = new Vector3(0f, direction.y, 0f) * verticalSpeed;

        transform.position += (horizontal + vertical) * Time.deltaTime;

        // 面朝水平移動方向（不讓水母上下翻轉）
        Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 3f);
    }

    void PickNewTarget()
    {
        // X Z：在草地範圍內隨機
        float rx = Random.Range(-moveAreaSize.x / 2f, moveAreaSize.x / 2f);
        float rz = Random.Range(-moveAreaSize.z / 2f, moveAreaSize.z / 2f);

        // Y：在 minY ~ maxY 之間隨機，模擬水母上浮下沉
        float ry = Random.Range(minY, maxY);

        targetPosition = new Vector3(
            moveAreaCenter.x + rx,
            moveAreaCenter.y + ry,
            moveAreaCenter.z + rz
        );
    }
}