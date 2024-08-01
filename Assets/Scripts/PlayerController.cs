using Fusion;
using UnityEngine;

public class PlayerController : BaseController
{
    public Transform spawnPoint;
    private float moveSpeed = 7f;
    private Rigidbody rb;

    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f; // Thời gian tối đa giữa hai lần chạm để được xem là double tap

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        maxHealth = 200f;
        base.Start();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            Move();
            CheckAttack();
        }
    }

    protected override void Move()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector3 moveDirection = new Vector3(data.direction.x, 0, data.direction.z).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Runner.DeltaTime);

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Runner.DeltaTime * 15f));
            }

            Debug.Log($"Moving: {moveDirection}"); // Để kiểm tra hướng di chuyển
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
                    if (spawnPoint != null)
                    {
                        if (Object.HasInputAuthority)
                        {
                            RPC_FireBullet(spawnPoint.position, spawnPoint.rotation);
                        }
                    }
                }
                lastTapTime = currentTime;
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_FireBullet(Vector3 position, Quaternion rotation)
    {
        if (ObjectPool.instance != null)
        {
            if (Runner.IsServer) // Chỉ server mới có thể spawn
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
        }
    }
}
