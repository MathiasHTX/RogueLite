using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] PlayerMovementAdvanced playerMovementAdvanced;
    [SerializeField] float scaleFactor = 100;
    [SerializeField] float time = 10;

    public void Shoot(Vector3 direction)
    {
        Debug.Log("Shooting" + direction);
        // Vector3 projectilePosition = new Vector3(direction.x * scaleFactor, direction.y - 0.1f * (float)System.Math.Pow(Time.deltaTime, 2), direction.z);
        transform.DOMove(direction, time).OnComplete(() => Destroy(this.gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovementAdvanced.TakeDamage();

            Destroy(this.gameObject);
        }
    }
}
