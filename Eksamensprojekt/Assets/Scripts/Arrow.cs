using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            enemyAI.EnemyHit(damage);
            Destroy(this.gameObject);
        }
    }
}
