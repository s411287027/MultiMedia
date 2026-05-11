using UnityEngine;

public class Jellyfish : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeTargetTime = 3f; // 每幾秒換一次方向
    
    private Vector3 targetPosition;
    private float timer;
    private Vector3 moveAreaCenter;
    private Vector3 moveAreaSize;

    // 接收生成器傳來的範圍資訊
    public void Initialize(Vector3 center, Vector3 size)
    {
        moveAreaCenter = center;
        moveAreaSize = size;
        PickNewTarget();
    }

    void Update()
    {
        timer += Time.deltaTime;
        // 如果時間到了，或者已經走到目標點附近，就換新目標
        if (timer >= changeTargetTime || Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            PickNewTarget();
            timer = 0;
        }

        // 往目標移動
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // 鎖定 Y 軸避免水母往地下鑽或飛走
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 讓水母面朝移動方向 (可選)
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }

    void PickNewTarget()
    {
        // 在指定的草地範圍內隨機取一個 X 和 Z 座標
        float randomX = Random.Range(-moveAreaSize.x / 2, moveAreaSize.x / 2);
        float randomZ = Random.Range(-moveAreaSize.z / 2, moveAreaSize.z / 2);
        targetPosition = moveAreaCenter + new Vector3(randomX, 0, randomZ);
    }
}