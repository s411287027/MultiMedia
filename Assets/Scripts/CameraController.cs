using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 1f;

    private float yaw = 0f;
    private float pitch = 0f; // 初始調整為 0 比較直覺

    // 建議將 pitch 的限制改為角度（例如 -30 度 到 60 度）
    public float pitchMin = -30f;
    public float pitchMax = 60f;

    public float GetYaw() => yaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 初始化 yaw 為目標的水平旋轉角度
        yaw = target.eulerAngles.y;

        // 也可以初始化 pitch 為目前相機的角度
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 左右旋轉 (Yaw)
        yaw += mouseDelta.x * mouseSensitivity * Time.deltaTime * 10f;

        // 上下旋轉 (Pitch) 
        // 註：滑鼠往上移動時 mouseDelta.y 是正數，通常我們希望相機往上看，
        // 但在 Unity 的 Euler 角度中，數值變小才是往上看，所以這裡用「減法」才符合直覺。
        pitch -= mouseDelta.y * mouseSensitivity * Time.deltaTime * 10f;

        // 限制上下旋轉的角度，防止相機翻過去
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // 計算旋轉與位置
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);

        // 更新相機位置與朝向
        transform.position = target.position + Vector3.up * height + offset;
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }

    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    public Vector3 GetCameraRight()
    {
        Vector3 right = transform.right;
        right.y = 0f;
        return right.normalized;
    }
}