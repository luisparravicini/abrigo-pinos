using UnityEngine;

public class Movement : MonoBehaviour
{
    public float rotationSpeed;
    public float walkSpeed;

    public void Setup(Vector3 delta, Vector2Int gridPos)
    {
        transform.position = delta + new Vector3(gridPos.x, 0, gridPos.y);
    }

    void Update()
    {
        int moveDelta = 0;
        int rotationDelta = 0;

        if (Input.GetKey(KeyCode.W))
            moveDelta = 1;
        if (Input.GetKey(KeyCode.S))
            moveDelta = -1;
        if (Input.GetKey(KeyCode.A))
            rotationDelta = -1;
        if (Input.GetKey(KeyCode.D))
            rotationDelta = 1;

        if (moveDelta != 0)
            transform.Translate(Vector3.forward * moveDelta * walkSpeed * Time.deltaTime);
        if (rotationDelta != 0)
            transform.Rotate(0, rotationDelta * rotationSpeed * Time.deltaTime, 0);
    }
}
