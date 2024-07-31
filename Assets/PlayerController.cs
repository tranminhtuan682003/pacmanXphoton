using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    public Transform spawnPoint; 
    private float wheelRotationSpeed = 360f;
    private float moveSpeed = 7f;
    public List<Transform> wheels;
    private Rigidbody rb;

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
        Move();
        if (Object.HasInputAuthority)
        {
            CheckAttack();
        }
    }

    protected override void Move()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector3 moveDirection = new Vector3(data.direction.x, 0, data.direction.z).normalized;
            transform.position += moveDirection * moveSpeed * Runner.DeltaTime;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Runner.DeltaTime * 15f);
            }

            RotateWheels();
        }
    }

    private void RotateWheels()
    {
        if (wheels != null)
        {
            foreach (var wheel in wheels)
            {
                if (wheel != null)
                {
                    wheel.Rotate(Vector3.right, wheelRotationSpeed * Runner.DeltaTime);
                }
            }
        }
    }

    private void CheckAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (spawnPoint != null)
            {
                if (Object.HasInputAuthority)
                {
                    RPC_FireBullet(spawnPoint.position, spawnPoint.rotation);
                }
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)] // Chỉ server hoặc đối tượng có StateAuthority có thể thực hiện RPC
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
