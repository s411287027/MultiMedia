using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float destroyDistance = 50f;
    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
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
