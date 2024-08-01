using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public Vector3 offset;
    [SerializeField] private float speed = 10f;

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.Slerp(transform.position, target.position + offset, speed * Time.deltaTime);
            transform.LookAt(target);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}