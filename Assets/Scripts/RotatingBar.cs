using UnityEngine;

public class RotatingBar : MonoBehaviour
{
    [Header("旋轉設定")]
    public float rotationSpeed = 90f;        // 每秒旋轉角度
    public Vector3 rotationAxis = Vector3.up; // 旋轉軸 (預設繞 Y 軸 = top-down 風車)

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
