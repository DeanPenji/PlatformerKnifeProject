using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    [HideInInspector] public Collider knifeCollider;
    [HideInInspector] public GameObject knifeParent;
    [HideInInspector] public Rigidbody knifeRb;

    public Material knifeMat;
    public Color knifeColor;

    public Transform attackPoint;

    public OneKnifeScript OKScript;

    public enum KnifeState {Equipped, Throwing, Attached, Recalling };
    public KnifeState knifeStateToggle;
    public KeyCode throwKey;

    private bool targetHit;
   
    public float lifeTimer = 5f;
    public int damage;
    public bool isTimed;
    public bool isSwappable;

    private Outline outlineRef;

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
        knifeRb = GetComponent<Rigidbody>();
        knifeCollider = GetComponent<Collider>();
        knifeMat.SetColor("_EmissionColor", knifeColor);
    }

    private void Update()
    {
        if(knifeStateToggle == KnifeState.Throwing)
        {
            transform.localEulerAngles += transform.right * rotationSpeed * Time.deltaTime;
        }
        else if (knifeStateToggle == KnifeState.Recalling)
        {
            if(Vector3.Distance(transform.position, attackPoint.position) < 2f)
            {
                knifeStateToggle = ProjectileAddon.KnifeState.Equipped;
                knifeRb.isKinematic = true;
                transform.position = attackPoint.position;
                transform.parent = attackPoint;
            }

            transform.position = Vector3.Lerp(transform.position, attackPoint.position, 0.15f);
            //transform.localEulerAngles -= transform.forward * rotationSpeed * Time.deltaTime;
        }

        // timer till despawn
        if(isTimed)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer < 0)
                Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        knifeStateToggle = KnifeState.Attached;

        if (collision.gameObject.GetComponent<EnemyScript>() != null)
        {
            if (enemy.TakeDamage(damage) == false)
            {
                knifeRb.isKinematic = false;
                knifeRb.velocity = Vector3.zero;
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
                    knifeRb.isKinematic = true;
                    transform.SetParent(collision.transform);
                    SetKnifeParent(collision.gameObject);

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

    public void SetKnifeParent(GameObject parent)
    {
        if(knifeParent == null)
        {
            knifeParent = parent;
            if(!knifeParent.GetComponent<Outline>())
                outlineRef = knifeParent.AddComponent<Outline>();

            if (parent.CompareTag("IsSwappable"))
            {
                isSwappable = true;
                outlineRef.OutlineColor = knifeColor;
                outlineRef.OutlineWidth = 10f;
            }

            else
            {
                outlineRef.OutlineColor = Color.red;
                outlineRef.OutlineWidth = 5f;
            }  
        }       

    }

    public void RemoveKnifeParent()
    {
        if(knifeParent != null)
        {
            Destroy(knifeParent.GetComponent<Outline>());
            knifeParent = null;
            isSwappable = false;
        }
        
    }
}
