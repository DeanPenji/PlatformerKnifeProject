using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    private bool targetHit;
    private Rigidbody rb;
    public float lifeTimer = 5f;
    public int damage;

    private bool inAir = false;
    public float rotationSpeed;
    public TypeOfThrowable type;
    public enum TypeOfThrowable 
    {
        Sticky,
        Deletion,

    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //GameObject projectilesToIgnore = GameObject.FindGameObjectWithTag("Player");
        //Physics.IgnoreCollision(projectilesToIgnore.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void Update()
    {
        if(inAir)
        {
            transform.localEulerAngles += transform.forward * rotationSpeed * Time.deltaTime;
        }
        // timer till despawn
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        inAir = false;

        if (collision.gameObject.GetComponent<EnemyScript>() != null)
        {
            if (enemy.TakeDamage(damage) == false)
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                transform.parent = null;
                return;
            }
        }

        if (targetHit)
        {
            return;
           
        }
        else
        {
            targetHit = true;
                if (type == TypeOfThrowable.Sticky && !collision.transform.CompareTag("Player"))
                {
                    rb.isKinematic = true;
                transform.SetParent(collision.transform);

                }
                else if (type == TypeOfThrowable.Deletion)
                {
                    Destroy(gameObject);
                }

        }

    }

    private void OnCollisionExit(Collision collision)
    {
        targetHit = false;
    }

}
