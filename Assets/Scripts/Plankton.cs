using UnityEngine;
using UnityEngine.InputSystem;

public class Plankton : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float turnSpeed = 12.0f;
    private Vector3 initialPosition;
    public GameObject normalModel;
    public GameObject holdingModel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
        normalModel.SetActive(true);
        holdingModel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    // 當皮老闆碰到其他實體碰撞器時（如果你的車子沒勾選 Is Trigger）
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("被車撞了！");
            transform.position = initialPosition; // 回到原點

            // 可選：如果你希望被撞後，蟹堡會掉下來（恢復原狀）
            // normalModel.SetActive(true);
            // holdingModel.SetActive(false);
        }
    }

    // 當皮老闆碰到觸發器時（例如放在地上的美味蟹堡）
    private void OnTriggerEnter(Collider other)
    {
        // 確保你的美味蟹堡物件有 Tag 叫 "Patty"，並且 Collider 勾選了 "Is Trigger"
        if (other.CompareTag("Patty"))
        {
            Debug.Log("拿到美味蟹堡了！");

            // 隱藏地上的蟹堡（或者直接 Destroy 它）
            other.gameObject.SetActive(false);
            // Destroy(other.gameObject); // 兩種寫法都可以，SetActive(false) 效能比較好

            // 切換皮老闆的造型
            normalModel.SetActive(false);
            holdingModel.SetActive(true);
        }
    }
}
