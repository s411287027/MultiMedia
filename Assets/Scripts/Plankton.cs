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
        // 特效播放中禁止移動
        if (isShaking) return;

        if (Keyboard.current.upArrowKey.isPressed)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);

        if (Keyboard.current.downArrowKey.isPressed)
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.Self);

        if (Keyboard.current.leftArrowKey.isPressed)
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime, Space.Self);

        if (Keyboard.current.rightArrowKey.isPressed)
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime, Space.Self);

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

            float angle   = Mathf.Sin(elapsed * shakeSpeed)  * shakeAngle;
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
        rb.linearVelocity  = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        if (tag == "Patty")
        {
            Debug.Log("拿到蟹堡！皮老闆成功了！");
            other.gameObject.SetActive(false);
            if (normalModel != null) normalModel.SetActive(false);
            if (holdingModel != null) holdingModel.SetActive(true);

            // 拿到蟹堡才算真正成功
            if (healthManager != null)
                healthManager.Win();
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