using UnityEngine;
using Fusion;

public class PlayerController : BaseController
{
    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private float moveSpeed = 6.5f;
    public Transform spawnPoint;

    [Networked] private bool isRunning { get; set; }
    [Networked] private int playerScore { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        playerScore = 0;
        maxHealth = 200f;
        base.Start();
        CameraFollow();
    }

    public override void FixedUpdateNetwork()
    {
        CheckInput();
        //if (Object.HasInputAuthority)
        //{
        //    CheckAttack();
        //}
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
            Vector3 newMoveDirection = Vector3.zero;
            if (Mathf.Abs(data.direction.x) > Mathf.Abs(data.direction.z))
            {
                newMoveDirection = new Vector3(data.direction.x, 0, 0).normalized;
            }
            else
            {
                newMoveDirection = new Vector3(0, 0, data.direction.z).normalized;
            }

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
        animator.SetTrigger("Hit");
    }

    protected override void Dead()
    {
        base.Dead();
        animator.SetTrigger("Death");
        if (HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            SoundManager.instance.PlaySound("Collect", false);
            int scoreIncrease = 10;
            playerScore += scoreIncrease;
            UpdateScoreUI(); // Cập nhật điểm số UI
        }
    }

    private void UpdateScoreUI()
    {
        Debug.Log("Updating score UI with score: " + playerScore);
        RPC_UpdateScore(playerScore);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_UpdateScore(int newScore)
    {
        Debug.Log("RPC Update Score called with new score: " + newScore);
        playerScore = newScore;

        // Cập nhật điểm số trên server (nếu đối tượng có StateAuthority)
        if (HasStateAuthority)
        {
            GameManager.instance.UpdateScoreForPlayer(Object.Id, playerScore);
        }
    }



}
