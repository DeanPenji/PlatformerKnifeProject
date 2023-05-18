using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    private float moveSpeed;

    public float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplayer;
    public float slopeIncreaseMultiplayer;
    public bool sliding;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundDrag;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Text")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI groundCheckText;
    public TextMeshProUGUI movementStateText;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air,
        standing
        
        
    }


    private void Start()
    {
        // store the rigidbody into the variable and free the rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //checking if there is a ground underneath the player
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        UpdateText();
        StateHandler();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0f;

    }

    public void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        //Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }
        //Mode - Crouching
        else if (Input.GetKey(crouchKey) && moveSpeed < sprintSpeed)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        //Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        //Mode - Walking
        else if (grounded && (horizontalInput != 0 || verticalInput != 0))
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        //Mode - In Air
        else if (!grounded)
        {
            state = MovementState.air;
        }

        //Mode - Standing
        else
        {
            state = MovementState.standing;
        }
        //check if desiredMoveSpeed has changed drastically

        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //smoothly lerp movespeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplayer * slopeIncreaseMultiplayer * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplayer;

            yield return null;
        }
        
        moveSpeed = desiredMoveSpeed;
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when ready to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        //crouching
        if (Input.GetKeyDown(crouchKey) && moveSpeed < sprintSpeed)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        //stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void MovePlayer()
    {

        // Calculate the movement direction, basing it off of the direction your looking
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        //on a slope
        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // add force into the directional key pressed, multiplied by the movespeed 
        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        
        // turn off gravity while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatvel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if movespeed is lower than the players speed
            if (flatvel.magnitude > moveSpeed)
            {
                //calculate what your max movespeed WOULD be
                Vector3 limitVel = flatvel.normalized * moveSpeed;
                //set that as your velocity
                rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
            }
        } 
    }

    private void Jump()
    {
        exitingSlope = true;
        // reset the y velocity to consistantly jump the same height
        rb.velocity = new Vector3(rb.velocity.x,0f,rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void UpdateText()
    {
        speedText.text = "Velocity: " + "<br>" + rb.velocity.magnitude.ToString("0.#");
        groundCheckText.text = "Ground Check: " + "<br>" + grounded.ToString();
        movementStateText.text = "Movement State: " + "<br>" + state;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
