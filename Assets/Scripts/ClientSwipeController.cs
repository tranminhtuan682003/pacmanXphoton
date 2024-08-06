using UnityEngine;
using Fusion;

public class ClientSwipeController : NetworkBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    public float swipeThreshold = 50f;
    private ClientMovement clientMovement;

    public override void Spawned()
    {
        clientMovement = GetComponent<ClientMovement>();
    }

    void Update()
    {
        if (Object.HasInputAuthority)
        {
            HandleTouch();
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    HandleSwipe();
                    break;
            }
        }
    }

    void HandleSwipe()
    {
        Vector2 swipeDirection = endTouchPosition - startTouchPosition;

        if (swipeDirection.magnitude >= swipeThreshold)
        {
            swipeDirection.Normalize();
            clientMovement.SetMovementDirection(swipeDirection);
        }
    }
}
