using UnityEngine;

public class Jellyfish : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeTargetTime = 3f;

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

        if (timer >= changeTargetTime || Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            PickNewTarget();
            timer = 0f;
        }

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f);
    }

    void PickNewTarget()
    {
        float rx = Random.Range(-moveAreaSize.x / 2f, moveAreaSize.x / 2f);
        float rz = Random.Range(-moveAreaSize.z / 2f, moveAreaSize.z / 2f);
        targetPosition = new Vector3(
            moveAreaCenter.x + rx,
            transform.position.y,       // 保持自身高度
            moveAreaCenter.z + rz
        );
    }
}