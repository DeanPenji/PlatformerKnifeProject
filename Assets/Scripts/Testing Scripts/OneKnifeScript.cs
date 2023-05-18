using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneKnifeScript : MonoBehaviour
{

    [Header("References")]
    public Transform cam;
    public ProjectileAddon knife1Script;
    public ProjectileAddon knife2Script;

    [Header("Throwing")]
    public KeyCode recallKey = KeyCode.R;
    public KeyCode swapKey = KeyCode.X;
    public float throwForce;
    public float throwUpwardForce;

    private void Start()
    {
        Physics.IgnoreCollision(knife1Script.knifeCollider, knife2Script.knifeCollider, true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(knife1Script.throwKey) && knife1Script.knifeStateToggle == ProjectileAddon.KnifeState.Equipped)
            ThrowKnife(knife1Script);

        if (Input.GetKeyDown(knife2Script.throwKey) && knife2Script.knifeStateToggle == ProjectileAddon.KnifeState.Equipped)
            ThrowKnife(knife2Script);

        if (Input.GetKeyDown(recallKey))
            RecallKnives();

        if (Input.GetKeyDown(swapKey))
            SwapMechanicCheck();
    }
    private void ThrowKnife(ProjectileAddon knifeToBeThrown)
    {
        knifeToBeThrown.gameObject.transform.position = knifeToBeThrown.attackPoint.position;

        knifeToBeThrown.knifeRb.isKinematic = false;


        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - knifeToBeThrown.attackPoint.position).normalized;
        }

        // add force
        Vector3 forcetoAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        knifeToBeThrown.knifeRb.AddForce(forcetoAdd, ForceMode.Impulse);

        knifeToBeThrown.knifeRb.transform.parent = null;
        knifeToBeThrown.knifeStateToggle = ProjectileAddon.KnifeState.Throwing;

    }
    private void RecallKnives()
    {
        if (knife1Script.knifeStateToggle != ProjectileAddon.KnifeState.Equipped)
        {
            knife1Script.knifeStateToggle = ProjectileAddon.KnifeState.Recalling;

            knife1Script.RemoveKnifeParent();
        }

        if (knife2Script.knifeStateToggle != ProjectileAddon.KnifeState.Equipped)
        {
            knife2Script.knifeStateToggle = ProjectileAddon.KnifeState.Recalling;

            knife2Script.RemoveKnifeParent();
        }
       
    }

    private void SwapMechanicCheck()
    {
        if (knife1Script.knifeStateToggle == ProjectileAddon.KnifeState.Attached && knife1Script.isSwappable)
        {
            if (knife2Script.knifeStateToggle == ProjectileAddon.KnifeState.Attached && knife2Script.isSwappable)
                SwapPosMechanic(knife1Script.knifeParent.gameObject, knife2Script.knifeParent.gameObject);

            else if (knife2Script.knifeStateToggle == ProjectileAddon.KnifeState.Equipped)
                SwapPosMechanic(knife1Script.knifeParent.gameObject, gameObject);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        else if (knife2Script.knifeStateToggle == ProjectileAddon.KnifeState.Attached && knife2Script.isSwappable)
        {
            if (knife1Script.knifeStateToggle == ProjectileAddon.KnifeState.Attached && knife1Script.isSwappable)
                SwapPosMechanic(knife2Script.knifeParent.gameObject, knife1Script.knifeParent.gameObject);

            else if (knife1Script.knifeStateToggle == ProjectileAddon.KnifeState.Equipped)
                SwapPosMechanic(knife2Script.knifeParent.gameObject, gameObject);
        }
    }

    private void SwapPosMechanic(GameObject objToSwap1, GameObject objToSwap2)
    {
        Vector3 objToSwap1Pos = new Vector3(objToSwap1.transform.position.x, objToSwap1.transform.position.y, objToSwap1.transform.position.z);
        Vector3 objToSwap2Pos = new Vector3(objToSwap2.transform.position.x, objToSwap2.transform.position.y, objToSwap2.transform.position.z);

        objToSwap1.transform.position = objToSwap2Pos;
        objToSwap2.transform.position = objToSwap1Pos;


    }

}
