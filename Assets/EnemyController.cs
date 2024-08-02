using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    [Header("Cube Settings")]
    public Vector3 cubeCenter = new Vector3(25,0,25);
    public Vector3 cubeSize = new Vector3(50f, 0.1f, 50f);

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public LayerMask clientLayerMask;


    [Header("AI")]
    private NavMeshAgent navMeshAgent;
    private Vector3 currentDestination;

    [Header("Animaton")]
    private Animator animator;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        MoveToPoint();
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Transform closestClient = GetClosestClient();

        if (closestClient != null)
        {
            ChaseClient(closestClient);
        }
        else
        {
            ContinueRandomMovement();
        }
    }

    private Transform GetClosestClient()
    {
        Collider[] clientsInRange = Physics.OverlapSphere(transform.position, detectionRadius, clientLayerMask);
        return FindClosestClient(clientsInRange);
    }

    private Transform FindClosestClient(Collider[] clientsInRange)
    {
        Transform closestClient = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider client in clientsInRange)
        {
            Vector3 directionToClient = client.transform.position - currentPosition;
            float dSqrToClient = directionToClient.sqrMagnitude;
            if (dSqrToClient < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToClient;
                closestClient = client.transform;
            }
        }

        return closestClient;
    }

    private void ChaseClient(Transform client)
    {
        navMeshAgent.SetDestination(client.position);
    }

    private void ContinueRandomMovement()
    {
        if (navMeshAgent.remainingDistance < 5f && !navMeshAgent.pathPending)
        {
            MoveToPoint();
        }
    }

    private void MoveToPoint()
    {
        currentDestination = GetRandomPositionOnCube(cubeCenter, cubeSize);
        navMeshAgent.SetDestination(currentDestination);
    }

    private Vector3 GetRandomPositionOnCube(Vector3 center, Vector3 size)
    {
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        BaseController player = collision.gameObject.GetComponent<BaseController>();
        if (collision.gameObject.layer == 6)
        {
            animator.SetBool("Attack", true);
            player.TakeDamage(20f);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            animator.SetBool("Attack", false);
        }
    }
}
