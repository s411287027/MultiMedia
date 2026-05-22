using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Plankton : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 120f;
    public float jumpForce = 5f;

    [Header("跳躍手感")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 2f;

    [Header("音效")]
    public AudioClip jellyfishStingSound;

    [Header("被電特效")]
    public float shakeDuration = 3f;
    public float shakeAngle = 20f;
    public float shakeSpeed = 10f;
    public float bounceHeight = 0.15f;
    public float bounceSpeed = 15f;

    public HealthManager healthManager;
    public GameObject normalModel;
    public GameObject holdingModel;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // 目前的重生點，預設是出生點，通過檢查點後會被換成該關起點
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private Vector3 pendingMoveDir = Vector3.zero;

    // 🌟 新增：用來記錄目前有沒有帶着蟹堡
    private bool hasPatty = false;

    // 🌟 新增：提供給終點區域讀取的公開屬性
    public bool HasPatty => hasPatty;
    private CameraController cameraController;

    // 給 Checkpoint 呼叫：把重生點換成第三關（或任何關）的起點
    public void SetCheckpoint(Vector3 pos, Quaternion rot)
    {
        respawnPosition = pos;
        respawnRotation = rot;
        Debug.Log("檢查點更新！死掉會回到這裡。");
    }
    private bool isGrounded = true;
    private bool isShaking = false;
    private Rigidbody rb;
    private AudioSource audioSource;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        respawnPosition = initialPosition;
        respawnRotation = initialRotation;
        cameraController = Camera.main.GetComponent<CameraController>();

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.LogWarning("Plankton 沒有 Rigidbody，已自動加上。");
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (healthManager == null)
            Debug.LogError("⚠️ HealthManager 沒有設定！請在 Inspector 拖入！");

        if (normalModel != null) normalModel.SetActive(true);
        if (holdingModel != null) holdingModel.SetActive(false);
    }

    void Update()
    {
        if (isShaking) return;

        Vector3 forward = cameraController.GetCameraForward();
        Vector3 right = cameraController.GetCameraRight();

        pendingMoveDir = Vector3.zero;

        if (Keyboard.current.upArrowKey.isPressed) pendingMoveDir += forward;
        if (Keyboard.current.downArrowKey.isPressed) pendingMoveDir -= forward;
        if (Keyboard.current.leftArrowKey.isPressed) pendingMoveDir -= right;
        if (Keyboard.current.rightArrowKey.isPressed) pendingMoveDir += right;

        // 皮老闆朝向跟相機 yaw 同步
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0f, cameraController.GetYaw(), 0f),
            Time.deltaTime * 10f
        );

        // 跳躍
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.linearVelocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isShaking) return;
        if (pendingMoveDir != Vector3.zero)
            rb.MovePosition(rb.position + pendingMoveDir * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }

        if (tag == "Jellyfish" && !isShaking)
        {
            Debug.Log("被水母電到！");

            if (jellyfishStingSound != null)
                audioSource.PlayOneShot(jellyfishStingSound);

            if (healthManager != null)
                healthManager.TakeDamage();
            else
                Debug.LogError("HealthManager 是 null！");

            // 特效結束後才 Respawn
            StartCoroutine(ShakeThenRespawn());
        }

        if (tag == "Car")
        {
            Debug.Log("被車撞！");
            if (healthManager != null)
                healthManager.TakeDamage();
            Respawn();
        }
    }

    private IEnumerator ShakeThenRespawn()
    {
        isShaking = true;
        float elapsed = 0f;

        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY
                       | RigidbodyConstraints.FreezeRotationZ;
        rb.isKinematic = true;

        Quaternion baseRotation = transform.rotation;
        Vector3 basePosition = transform.position;

        // 左右晃動 + 上下震動
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float angle = Mathf.Sin(elapsed * shakeSpeed) * shakeAngle;
            float offsetY = Mathf.Sin(elapsed * bounceSpeed) * bounceHeight;

            transform.rotation = baseRotation * Quaternion.Euler(0f, 0f, angle);
            transform.position = basePosition + new Vector3(0f, offsetY, 0f);

            yield return null;
        }

        // 特效結束 → 還原物理 → 回到原點
        transform.rotation = baseRotation;
        transform.position = basePosition;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
        isShaking = false;

        Respawn();
    }

    private void Respawn()
    {
        Debug.Log($"Respawn 到 {respawnPosition}");
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        Debug.Log($"【物理偵測】皮老闆碰到了：{other.name}，它的 Tag 是：{other.tag}");
        if (tag == "Patty")
        {

            // 直接用自己腳本裡的 SetCheckpoint，把位置設到該 Patty 的位置
            SetCheckpoint(other.transform.position, other.transform.rotation);

            Debug.Log("主動偵測：踩到蟹堡，重設重生點！");

            hasPatty = true;

            //Debug.Log("拿到蟹堡！皮老闆成功了！");
            other.gameObject.SetActive(false);
            if (normalModel != null) normalModel.SetActive(false);
            if (holdingModel != null) holdingModel.SetActive(true);

            

            // 拿到蟹堡才算真正成功
            //if (healthManager != null)
            //    healthManager.Win();
        }

        if (tag == "WinZone")
        {
            if (hasPatty)
            {
                Debug.Log("皮老闆主動回報：我帶著蟹堡回到終點了！");
                if (healthManager != null)
                {
                    healthManager.Win(); // 這裡會去呼叫 HealthManager，並由它打開 Win Panel
                }
                else
                {
                    Debug.LogError("⚠️ 皮老闆身上找不到 HealthManager！請檢查 Inspector 有沒有拖入！");
                }
            }
            else
            {
                Debug.Log("皮老闆：我回到了終點，但我還沒拿到蟹堡...");
            }
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