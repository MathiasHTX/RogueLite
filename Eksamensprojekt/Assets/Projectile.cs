using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] float scaleFactor = 100;
    [SerializeField] float time = 10;

    public void Shoot(Vector3 direction)
    {
        Debug.Log("Shooting" + direction);
        transform.DOMove(direction * scaleFactor, time).OnComplete(() => Destroy(this.gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovementAdvanced.instance.TakeDamage();

            Destroy(this.gameObject);
        }
    }
}
