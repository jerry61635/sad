using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player_Movement : NetworkBehaviour
{
    public CharacterController controller;

    //Input
    public float horizontal;   //default AD
    public float vertical;     //WS
    public bool jump;          //"Space Bar"
    public bool dash;          //"Left Shift"
    public bool pause;         //"Esc"
    public bool interact;      //E

    //Moving & physcis reference
    float turnSmoothTime = 0.1f;
    float SmoothVelocity;
    [SerializeField]
    float jumpHeight;
    [SerializeField]
    float speed;
    [SerializeField]
    float runningSpeed;
    public float autoRuntime;
    float runTime = 0f;
    float current_speed;
    float currentDashTime;
    public float maxDashTime;
    public float dashStopSpeed;
    public float dashSpeed;
    float dashCooldown;
    [SerializeField]
    float gravity;
    Vector3 velocity;
    bool isground;
    public Transform groundCheck;
    public LayerMask ground;

    public void CharacterMove()
    {
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        //auto run system
        if (runTime < autoRuntime && (Mathf.Abs(horizontal) == 1 || Mathf.Abs(vertical) == 1))
        {
            current_speed = speed;
            runTime += Time.deltaTime;
        }
        else if (runTime < autoRuntime)
            current_speed = speed;
        else if (runTime >= autoRuntime)
            current_speed = runningSpeed;
        if (direction.x == 0 && direction.z == 0) runTime = 0;

        //moving
        isground = Physics.CheckSphere(groundCheck.position, 0.5f, ground);
        if (direction.magnitude >= 0.1f && IsLocalPlayer)
        {
            float turn = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + GameManager.Instance.Cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, turn, ref SmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 movedir = Quaternion.Euler(0f, turn, 0f) * Vector3.forward;
            controller.Move(movedir.normalized * current_speed * Time.deltaTime);
        }

        //jumping
        if (jump && isground)
        {
            velocity.y = Mathf.Sqrt(2 * -gravity * jumpHeight);
        }

        if (isground && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //dash
        if (dash)
            currentDashTime = 0f;
        if (currentDashTime < maxDashTime)
        {
            currentDashTime += dashStopSpeed;
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
        }
    }
}


