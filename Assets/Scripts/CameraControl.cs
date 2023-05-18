using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Both sensitivity for x and y axis'
    public float sensX;
    public float sensY;

    // the orientation of the player
    public Transform orientation;

    float xRotation;
    float yRotation;
    void Start()
    {
        // Locking and hiding the cursor in the midlle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get both the players x and y inputs and multiplyl them by the sensitivty

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;

        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // set both the inputs data into the rotation 
        yRotation += mouseX;

        xRotation -= mouseY;
        
        //Clamp the players rotation so they can 360
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


    // Set the values into the rotation to allow the player to look around
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);  
    }
}
