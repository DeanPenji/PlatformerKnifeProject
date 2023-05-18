using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingScript : MonoBehaviour
{

    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject knife1Prefab;
    public GameObject knife2Prefab;

    [Header("Settings")]
    public int TotalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode throwKeyAlt = KeyCode.Mouse1;
    public KeyCode recallKey = KeyCode.R;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;
    public bool knife1NotBeenThrown = true;
    public bool knife2NotBeenThrown = true;

    List<GameObject> knifeList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        //New Lines include throwKeyAlt, knifeToBeThrown
        if (Input.GetKeyDown(throwKey) && knife1NotBeenThrown)
            Throw(knife1Prefab);

        if (Input.GetKeyDown(throwKeyAlt) && knife2NotBeenThrown)
            Throw(knife2Prefab);

        if (Input.GetKeyDown(recallKey))
            ResetThrow();
    }
    private void Throw(GameObject knifeToBeThrown)
    {

        //spawn in object to throw

        GameObject projectile = Instantiate(knifeToBeThrown, attackPoint.position, cam.rotation);

        //get rigidbody of object
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // add force
        Vector3 forcetoAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forcetoAdd, ForceMode.Impulse);
        
        //check which projectile has been thrown
        if(knifeToBeThrown == knife1Prefab)
        {
            knife1NotBeenThrown = false;
        }
        else if (knifeToBeThrown == knife2Prefab)
        {
            knife2NotBeenThrown = false;
        }

        knifeList.Add(projectile);
        //implement cooldown
        //Invoke(nameof(ResetThrow), throwCooldown);
    }
    private void ResetThrow()
    {
        knife1NotBeenThrown = true;
        knife2NotBeenThrown = true;
    }

}
