using UnityEngine;
using Fusion;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    public Vector3 offset;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = player.transform.position + offset;
        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
    }
}
