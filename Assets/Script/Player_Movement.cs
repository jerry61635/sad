using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    float turnSmoothTime = 0.1f;
    float SmoothVelocity;

    [SerializeField]
    float jumpHeight = 5f;

    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float runningSpeed = 8f;

    [SerializeField]
    float gravity = -9.8f;
    Vector3 velocity;

    bool isground;
    public Transform groundCheck;
    public LayerMask ground;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float current_speed;
        if (Input.GetKey("left shift"))
        {
            current_speed = runningSpeed;
        }
        else
        {
            current_speed = speed;
        }

        isground = Physics.CheckSphere(groundCheck.position, 0.5f, ground);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float turn = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, turn, ref SmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 movedir = Quaternion.Euler(0f, turn, 0f) * Vector3.forward;
            controller.Move(movedir.normalized * current_speed * Time.deltaTime);
        }

        if (Input.GetButton("Jump") && isground)
        {
            velocity.y = Mathf.Sqrt(2 * -gravity * jumpHeight);
        }

        if(isground && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
