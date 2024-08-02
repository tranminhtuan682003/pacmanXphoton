using Fusion;
using UnityEngine;

public class ClientMovement : NetworkBehaviour
{
    public float speed = 5f;
    private Vector3 movementDirection;

    void Update()
    {
        if (Object.HasStateAuthority)
        {
            Move();
        }
    }

    public void SetMovementDirection(Vector2 direction)
    {
        movementDirection = new Vector3(direction.x, 0, direction.y).normalized;
    }

    void Move()
    {
        transform.position += movementDirection * speed * Time.deltaTime;
    }
}
