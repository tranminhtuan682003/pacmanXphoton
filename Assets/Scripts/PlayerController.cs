using UnityEngine;
using Fusion;

public class PlayerController : BaseController
{
    private float moveSpeed = 6f;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.1f;

    [Networked] private bool isRunning { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        maxHealth = 200f;
        base.Start();
        CameraFollow();
    }

    public override void FixedUpdateNetwork()
    {
        CheckInput();
    }

    private void CameraFollow()
    {
        if (HasInputAuthority)
        {
            Camera.main.GetComponent<CameraController>().SetTarget(transform);
        }
    }

    private void CheckInput()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector3 newMoveDirection = new Vector3(data.direction.x, 0, data.direction.z).normalized;

            if (newMoveDirection != Vector3.zero)
            {
                if (!isRunning)
                {
                    animator.SetBool("Run", true);
                    isRunning = true;
                }

                moveDirection = newMoveDirection;
                rb.velocity = moveDirection * moveSpeed;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Runner.DeltaTime * 15f));
            }
            else
            {
                if (isRunning)
                {
                    animator.SetBool("Run", false);
                    isRunning = false;
                }
                rb.velocity = Vector3.zero;
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Dead()
    {
        if (Object != null && Runner != null)
        {
            Runner.Despawn(Object);
        }
    }
}
