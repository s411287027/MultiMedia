using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 3f;

    private float yaw = 0f;
    private float pitch = 1f;

    public float pitchMin = 1f;
    public float pitchMax = 10f;
    public float GetYaw() => yaw;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 用皮老闆目前的朝向初始化 yaw
        yaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 只保留左右
        yaw += mouseDelta.x * mouseSensitivity * Time.deltaTime * 10f;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
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