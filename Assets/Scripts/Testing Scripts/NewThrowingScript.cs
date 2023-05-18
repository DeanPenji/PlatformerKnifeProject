using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewThrowingScript : MonoBehaviour
{

    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject knife1;
    public GameObject knife2;

    [Header("Settings")]
    public int TotalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode throwKeyAlt = KeyCode.Mouse1;
    public KeyCode recallKey = KeyCode.R;
    public float throwPower;
    public float throwUpwardForce;

    public bool knife1NotBeenThrown = true;
    public bool knife2NotBeenThrown = true;

    public Vector3 oldPos;

    // Start is called before the first frame update
    void Start()
    {
        knife1.GetComponent<Rigidbody>().position = attackPoint.position;
        knife2.GetComponent<Rigidbody>().position = attackPoint.position;
        knife1.GetComponent<Collider>().enabled = false;
        knife2.GetComponent<Collider>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        //New Lines include throwKeyAlt, knifeToBeThrown
        if (Input.GetKeyDown(throwKey) && knife1NotBeenThrown)
            Throw(knife1);

        if (Input.GetKeyDown(throwKeyAlt) && knife2NotBeenThrown)
            Throw(knife2);

        if (Input.GetKeyDown(recallKey))
            RecallKnives();
    }
    private void Throw(GameObject knifeToBeThrown)
    {

        //spawn in knife at knife throwing point
        knifeToBeThrown.GetComponent<Collider>().enabled = true;
        knifeToBeThrown.transform.position = attackPoint.position;

        //get rigidbody of object
        Rigidbody knifeRb = knifeToBeThrown.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // add force
        Vector3 forcetoAdd = forceDirection * throwPower + transform.up * throwUpwardForce;

        knifeRb.AddForce(forcetoAdd, ForceMode.Impulse);
        knifeRb.transform.parent = null;
        //check which knife has been thrown
        if (knifeToBeThrown == knife1)
        {
            knife1NotBeenThrown = false;
        }
        else if (knifeToBeThrown == knife2)
        {
            knife2NotBeenThrown = false;
        }

        //implement cooldown
        //Invoke(nameof(ResetThrow), throwCooldown);
    }
    private void RecallKnives()
    {

        if(knife1.GetComponent<Rigidbody>().transform.parent !=  gameObject)
        {
            knife1.GetComponent<Rigidbody>().transform.parent = null;
            knife2.GetComponent<Rigidbody>().transform.parent = null;

            knife1.GetComponent<Rigidbody>().velocity = Vector3.zero;
            knife2.GetComponent<Rigidbody>().velocity = Vector3.zero;

            //knife1.GetComponent<Rigidbody>().isKinematic = true;
            //knife2.GetComponent <Rigidbody>().isKinematic = true;
        }

        knife1.GetComponent <Rigidbody>().position = attackPoint.position;
        knife2.GetComponent <Rigidbody>().position = attackPoint.position;

        knife1.GetComponent<Collider>().enabled = false;
        knife2.GetComponent<Collider>().enabled = false;

        knife1NotBeenThrown = true;
        knife2NotBeenThrown = true;
    }
}
