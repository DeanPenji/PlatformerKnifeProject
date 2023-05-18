using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour
{

    public int enemyHealth;
 
    public bool TakeDamage(int damage)
    {
        enemyHealth -= damage;

        if(enemyHealth <= 0)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }
}
