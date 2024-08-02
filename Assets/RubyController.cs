using System;
using System.Collections;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(0,0,1);
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
            GameManager.instance.Score(100);
            GameManager.instance.StartReActiveRUby(this.gameObject, 10f);
    }
}
