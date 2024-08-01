using System.Collections;
using UnityEngine;
using Fusion;

public class BulletController : NetworkBehaviour
{
    public float speed = 200f;
    private string tagBullet;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        tagBullet = gameObject.tag;
    }

    private void OnEnable()
    {
        StartCoroutine(ActiveBullet());
    }

    public override void FixedUpdateNetwork()
    {
        Move();
    }

    private void Move()
    {
        rb.velocity = Vector3.forward * speed;
    }
    private IEnumerator ActiveBullet()
    {
        yield return new WaitForSeconds(3f);
        if (gameObject.activeInHierarchy)
        {
            ObjectPool.instance.ReturnPooledObject(tagBullet, GetComponent<NetworkObject>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        var player = other.GetComponentInParent<BaseController>();
        if (player != null)
        {
            player.TakeDamage(200f);
            ObjectPool.instance.ReturnPooledObject(tagBullet, GetComponent<NetworkObject>());
        }
    }
}
