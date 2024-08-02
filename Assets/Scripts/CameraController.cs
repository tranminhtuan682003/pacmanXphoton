using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    [SerializeField] Vector3 offset;

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position - offset;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
