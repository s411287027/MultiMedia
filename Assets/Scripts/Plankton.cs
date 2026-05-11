using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // 為了使用 Coroutine (協程)

public class Plankton : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 12.0f;
    public float jumpForce = 5f;      // 跳躍力道
    public float stunDuration = 2.0f; // 暈眩時間（你可以在 Inspector 中自由調整）

    private Vector3 initialPosition;
    public GameObject normalModel;
    public GameObject holdingModel;

    private bool isStunned = false; // 是否被暈眩
    private bool isGrounded = true; // 是否在地面上
    private Rigidbody rb;

    void Start()
    {
        initialPosition = transform.position;
        normalModel.SetActive(true);
        holdingModel.SetActive(false);
        rb = GetComponent<Rigidbody>(); // 抓取剛體元件
    }

    void Update()
    {
        // 如果處於暈眩狀態，直接跳出 Update，不執行移動與跳躍
        if (isStunned) return; 

        // 移動邏輯
        if (Keyboard.current.upArrowKey.isPressed)
        {
            transform.Translate(moveSpeed * Time.deltaTime * transform.forward, Space.World);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Rotate(new Vector3(0, 1, 0), turnSpeed * Time.deltaTime, Space.Self);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            transform.Rotate(new Vector3(0, 1, 0), -turnSpeed * Time.deltaTime, Space.Self);
        }

        // 跳躍邏輯 (按下空白鍵 且 在地面上)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 跳起後狀態改為不在地面
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 碰到地板，恢復跳躍能力
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // 碰到水母，觸發暈眩
        if (collision.gameObject.CompareTag("Jellyfish"))
        {
            Debug.Log("被水母電到了！");
            StartCoroutine(StunRoutine());
        }

        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("被車撞了！");
            transform.position = initialPosition; 
        }
    }

    // 處理暈眩倒數的協程
    private IEnumerator StunRoutine()
    {
        isStunned = true; // 開啟暈眩
        yield return new WaitForSeconds(stunDuration); // 等待設定的秒數
        isStunned = false; // 解除暈眩
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Patty"))
        {
            Debug.Log("拿到美味蟹堡了！");
            other.gameObject.SetActive(false);
            normalModel.SetActive(false);
            holdingModel.SetActive(true);
        }
    }
}