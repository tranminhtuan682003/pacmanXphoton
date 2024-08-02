using UnityEngine;
using Fusion;

public class PlayerController : BaseController
{
    public Transform SpawnPoint;
    private float moveSpeed = 6f;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.1f;
    private bool isRunning;
    private int tapCount = 0;

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
        if (Object.HasInputAuthority)
        {
            CheckInput();
            Move();
            //CheckAttack();
        }
    }

    private void CameraFollow()
    {
        if (Object.HasInputAuthority)
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
                moveDirection = newMoveDirection;
                isRunning = true;
            }
        }
    }

    protected override void Move()
    {
        if (isRunning)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Runner.DeltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Runner.DeltaTime * 15f));

            animator.SetBool("Run", true);
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetBool("Run", false);
        }
    }

    private void CheckAttack()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                float currentTime = Time.time;
                if (currentTime - lastTapTime <= doubleTapThreshold)
                {
                    tapCount++;
                    if (tapCount == 2)
                    {
                        if (SpawnPoint != null && Object.HasInputAuthority)
                        {
                            // Call RPC to notify all clients to fire the bullet
                            RPC_FireBullet(SpawnPoint.position, SpawnPoint.rotation);
                        }
                        tapCount = 0; // Reset tap count after shooting
                    }
                }
                else
                {
                    tapCount = 1; // Reset tap count if time between taps is too long
                }
                lastTapTime = currentTime;
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_FireBullet(Vector3 position, Quaternion rotation)
    {
        if (ObjectPool.instance != null && Runner != null)
        {
            if (Runner.IsServer)
            {
                ObjectPool.instance.Fire("BulletPlayer", position, rotation, Runner, Object.InputAuthority);
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
            GameManager.instance.Exit();
        }
    }
}
