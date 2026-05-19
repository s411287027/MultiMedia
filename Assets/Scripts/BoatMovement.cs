using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float destroyDistance = 50f;

    public float spawnRotationY = 0f;
    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            spawnRotationY,
            transform.eulerAngles.z
        );
    }

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) > destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
