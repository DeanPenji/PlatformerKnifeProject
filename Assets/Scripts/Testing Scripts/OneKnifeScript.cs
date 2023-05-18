using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneKnifeScript : MonoBehaviour
{

    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject knifeObj;

    [Header("Settings")]
    public int TotalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode recallKey = KeyCode.R;
    public KeyCode swapKey = KeyCode.X;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;
    public bool isEquipped = true;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        //New Lines include throwKeyAlt, knifeToBeThrown
        if (Input.GetKeyDown(throwKey) && isEquipped)
            ThrowKnife();
        if (Input.GetKeyDown(recallKey))
            ResetThrow();
        if (Input.GetKeyDown(swapKey) && knifeObj.transform.parent.CompareTag("IsSwappable"))
            SwapPosMechanic(knifeObj.transform.parent.gameObject);
    }
    private void ThrowKnife()
    {
        knifeObj.transform.position = attackPoint.position;
        //get rigidbody of object
        Rigidbody knifeRb = knifeObj.GetComponent<Rigidbody>();

        knifeRb.isKinematic = false;


        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // add force
        Vector3 forcetoAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        knifeRb.AddForce(forcetoAdd, ForceMode.Impulse);

        knifeRb.transform.parent = null;
        isEquipped = false;

    }
    private void ResetThrow()
    {
        isEquipped = true;
        Rigidbody knifeRb = knifeObj.GetComponent<Rigidbody>();
        knifeRb.isKinematic = true;
        knifeObj.transform.position = attackPoint.position;
        knifeObj.transform.parent = attackPoint;
    }

    private void SwapPosMechanic(GameObject ObjToSwap)
    {
        Vector3 objToSwapPos = new Vector3(ObjToSwap.transform.position.x, ObjToSwap.transform.position.y, ObjToSwap.transform.position.z);
        Vector3 PlayerPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);

        ObjToSwap.transform.position = PlayerPos;
        gameObject.transform.position = objToSwapPos;


    }

}
