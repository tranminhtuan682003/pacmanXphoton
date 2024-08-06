using UnityEngine;
using Fusion;

public class RubyController : NetworkBehaviour
{
    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(0, 0, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        GameManager.instance.StartReActiveRUby(this.gameObject, 15f);
    }
}
