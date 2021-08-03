using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    [SerializeField] float moveSpeed, jumpForce, dashDistance, timeToDash, targetMaxDist, rayCastMaxDistance;
    [SerializeField] GameObject target;
    [SerializeField] Transform weaponSlot, bodyTranform;
    [SerializeField] bool canDash;
    [SerializeField] PlayerController controller; //Input system
    Rigidbody rigidBody;
    Gun gun;
    Vector3 move;
    Vector2 inputMove, inputMousePosition; //informazione presa da input system
    float horizontalMove, verticalMove, dashTimer;
    bool isGrounded, isShooting, isDashing;
    public bool hasGun { get; set; }
    public static InputManager instance;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        controller = new PlayerController();
        dashTimer = 0;
        canDash = true;

        //input system - move START
        controller.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        controller.Player.Move.canceled += ctx => inputMove = Vector2.zero;
        //input system - move END

        //input system - aim START
        controller.Player.Aim.performed += ctx => inputMousePosition = ctx.ReadValue<Vector2>();
        //input system - aim END

        //input system - fire START
        controller.Player.Shoot.performed += ctx => isShooting = true;
        controller.Player.Shoot.canceled += ctx => isShooting = false;
        //input system - fire END

        //input system - jump
        controller.Player.Jump.performed += ctx => Jump();

        //input system - Dash
        controller.Player.Dash.performed += ctx => isDashing = true;
        controller.Player.Dash.canceled += ctx => isDashing = false;

        //input system - DropGun
        controller.Player.DropGun.performed += ctx => DropGun();
    }

    private void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Aim();
        Move();
        if(hasGun && isShooting) Shoot();
        if (isDashing && canDash) Dash();

        /* OLD INPUT SYSTEM - START
            if (Input.GetKey(KeyCode.Mouse0)) Shoot();
            if (Input.GetKey(KeyCode.Q) && hasGun) gun.DropGun();
            if (Input.GetKey(KeyCode.Space)) Jump();
            if (Input.GetKey(KeyCode.LeftShift) && canDash) Dash();
           OLD INPUT SYSTEM - END*/
    }

    void Move()
    {
        //Movement START

        //OLD INPUT SYSTEM
        //horizontalMove = Input.GetAxis("Horizontal");
        //verticalMove = Input.GetAxis("Vertical");

        horizontalMove = inputMove.x; //input system - l'asse delle ascisse rimane la x
        verticalMove = inputMove.y; //input system - essendo vector2, l'asse z corrisponde all'asse y di input system

        move = new Vector3(horizontalMove, 0, verticalMove);
        rigidBody.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
        //Movement END
    }

    void Jump()
    {
        //Jump START
        if (isGrounded)
        {
            isGrounded = false;
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        //Jump END
    }

    void Shoot()
    {
       gun.Shoot();
    }

    void DropGun()
    {
        if (hasGun) gun.DropGun();
    }

    void Aim()
    {
        RaycastHit hit;
        //Input System
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(inputMousePosition.x, inputMousePosition.y, 0));

        //Old Input System
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayCastMaxDistance)) //da rendere efficiente considerando solo i layer necessari
        {
            Vector3 targetPos;
            if(Vector3.Distance(hit.point, transform.position) <= targetMaxDist)
            {
                targetPos = hit.point;
            }
            else
            {
                targetPos = new Vector3(bodyTranform.position.x + (bodyTranform.forward.x * targetMaxDist), 1, bodyTranform.position.z + (bodyTranform.forward.z * targetMaxDist));
            }
            //target.transform.position = new Vector3(transform.position.x + targetPos.x, 1, transform.position.z + targetPos.z);
            target.transform.position = targetPos;
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    void Dash()
    {
        canDash = false;
        rigidBody.MovePosition(transform.position + move * dashDistance * Time.deltaTime);
        StartCoroutine("DashCoolDown");
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(timeToDash);
        canDash = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gun") && !hasGun)
        {
            gun = other.transform.GetComponent<Gun>(); //Se da errore, assicurarsi che il parent dell'arma abbia tag "gun" mentre i figli siano untagged
            if (gun.CanPickUp())
            {
                other.transform.parent = weaponSlot.transform;
                other.transform.position = weaponSlot.transform.position;
                other.transform.rotation = weaponSlot.transform.rotation;
                gun.PickUp();
                hasGun = true;
            }
        }
        if (other.transform.CompareTag("Platform") && !isGrounded) isGrounded = true;
    }

    //Input System - Enablle Input System 
    private void OnEnable()
    {
        controller.Player.Enable();
    }

    //Input System - Disablle Input System
    private void OnDisable()
    {
        controller.Player.Disable();
    }
}
