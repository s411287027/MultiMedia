using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Plankton : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 120f;
    public float jumpForce = 5f;
    public HealthManager healthManager;

    public GameObject normalModel;
    public GameObject holdingModel;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isGrounded = true;
    private Rigidbody rb;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.LogWarning("Plankton 沒有 Rigidbody，已自動加上。");
        }

        // 鎖定 X/Z 旋轉，防止被 Boat 撞倒翻滾
        // 但不鎖 Y 速度，保留跳躍能力
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;

        if (healthManager == null)
            Debug.LogError("⚠️ HealthManager 沒有設定！請在 Inspector 拖入！");

        if (normalModel != null) normalModel.SetActive(true);
        if (holdingModel != null) holdingModel.SetActive(false);
    }

    void Update()
    {
        // ↑ 前進
        if (Keyboard.current.upArrowKey.isPressed)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);

        // ↓ 後退
        if (Keyboard.current.downArrowKey.isPressed)
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.Self);

        // ← 左轉
        if (Keyboard.current.leftArrowKey.isPressed)
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime, Space.Self);

        // → 右轉
        if (Keyboard.current.rightArrowKey.isPressed)
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime, Space.Self);

        // A 左平移
        if (Keyboard.current.aKey.isPressed)
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);

        // D 右平移
        if (Keyboard.current.dKey.isPressed)
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);

        // 空白鍵跳躍
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        Debug.Log("碰到Tag：" + tag);

        // ===== 地板：恢復跳躍 =====
        // 用 Layer 名稱比較，避免 Tag 未定義的 Error
        // 同時也支援 Tag = "Ground"（如果你有設定的話）
        if (tag == "Ground" || tag == "Untagged")
        {
            // 只要是從下方碰到（法向量朝上），就視為落地
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        // ===== 水母：扣血 + 回原點 =====
        if (tag == "Jellyfish")
        {
            Debug.Log("被水母電到！扣血並重生。");
            if (healthManager != null)
                healthManager.TakeDamage();
            else
                Debug.LogError("HealthManager 是 null！請在 Inspector 拖入！");

            Respawn();
        }

        // ===== 車：扣血 + 回原點，但不飛起來 =====
        if (tag == "Car")
        {
            Debug.Log("被車撞！");
            if (healthManager != null)
                healthManager.TakeDamage();

            Respawn();
        }
    }

    // 回到初始位置，同時清除所有速度避免繼續飛
    private void Respawn()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.linearVelocity  = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        if (tag == "Patty")
        {
            Debug.Log("拿到蟹堡！");
            other.gameObject.SetActive(false);
            if (normalModel != null) normalModel.SetActive(false);
            if (holdingModel != null) holdingModel.SetActive(true);
        }

        if (tag == "RotatingBar")
        {
            Debug.Log("被旋轉棒打飛！");
            if (healthManager != null)
                healthManager.TakeDamage();
            Respawn();
        }
    }
}