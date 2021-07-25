using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float moveSpeed, jumpHeight, shiftZ;
    [SerializeField] GameObject target;
    CharacterController controller;
    Gun gun;
    Vector3 move, jumpVelocity;
    float horizontalMove, verticalMove;
    bool isShifting;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        Move();
        Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift)) Shift();
        if (Input.GetKey(KeyCode.Mouse0) && Player.instance.GetHasGun()) gun.Shoot();
        if (Input.GetKey(KeyCode.Q) && Player.instance.GetHasGun()) gun.DropGun();
    }

    void Move()
    {
        //Movement START
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");
        move = new Vector3(horizontalMove, 0, verticalMove);
        move += Physics.gravity * Time.deltaTime;
        controller.Move(move * moveSpeed * Time.deltaTime);
        //Movement END
    }

    void Jump()
    {
        //Jump START
        if (controller.isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            jumpVelocity.y += Mathf.Sqrt(jumpHeight * -3f * Physics.gravity.y);
        }
        jumpVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(jumpVelocity * Time.deltaTime);
        //Jump END
    }
    void Aim()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100))
        {
            target.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    void Shift()
    {
        transform.Translate(Vector3.forward * shiftZ);
    }

    public void SetGun(Gun gun)
    {
        this.gun = gun;
    }
}
