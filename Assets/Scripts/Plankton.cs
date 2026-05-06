using UnityEngine;
using UnityEngine.InputSystem;

public class Plankton : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float turnSpeed = 8000.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
}
