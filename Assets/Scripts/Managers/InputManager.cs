using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    [SerializeField] float moveSpeed, rotationSpeed, jumpForce, dashDistance, timeToDash, targetMaxDist, rayCastMaxDistance, timeToUnstun;
    [SerializeField] GameObject target;
    [SerializeField] Transform weaponSlot, bodyTranform;
    [SerializeField] bool canDash;
    PlayerController controller; //Input system
    Rigidbody rigidBody;
    Gun gun;
    Player player;
    Vector3 move, targetPos;
    Vector2 inputMove, inputAim; //informazione presa da input system
    float horizontalMove, verticalMove;
    bool isGrounded, isShooting, isDashing, enableControl, hasGun, isStunned;
    public static InputManager instance;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        controller = new PlayerController();
        player = GetComponent<Player>();
        canDash = true;
        enableControl = true;

        //input system - move START
        controller.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        controller.Player.Move.canceled += ctx => inputMove = Vector2.zero;
        //input system - move END

        //input system - aim START
        controller.Player.Aim.performed += ctx => inputAim = ctx.ReadValue<Vector2>();
        controller.Player.Aim.canceled += ctx => inputAim = Vector2.zero;
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

        //TO REMOVE
        controller.Player.Damage.performed += ctx => TakeDamage();
        controller.Player.Unstun.performed += ctx => StartUnstun();
        controller.Player.Unstun.canceled += ctx => EndUnstun();
    }

    //TEST
    void TakeDamage()
    {
        player.GetDamage(1f);
    }

    void StartUnstun()
    {
        if (player.GetIsStunned()) StartCoroutine("StartUnstunTimer");
    }

    IEnumerator StartUnstunTimer()
    {
        var timer = 0f;
        while(timer < timeToUnstun)
        {
            timer += Time.deltaTime;
            Debug.Log("Unstun timer: " + timer);
            yield return null;
        }
        player.SetIsStunned(false);
        player.Unstun();
        enableControl = true;
    }

    void EndUnstun()
    {
        if (player.GetIsStunned())
        {
            StopCoroutine("StartUnstunTimer");
            
        }
    }

    private void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enableControl)
        {
            Aim();
            Move();
            if (hasGun && isShooting) Shoot();
            if (isDashing && canDash) Dash();
            /* OLD INPUT SYSTEM - START
                if (Input.GetKey(KeyCode.Mouse0)) Shoot();
                if (Input.GetKey(KeyCode.Q) && hasGun) gun.DropGun();
                if (Input.GetKey(KeyCode.Space)) Jump();
                if (Input.GetKey(KeyCode.LeftShift) && canDash) Dash();
               OLD INPUT SYSTEM - END*/
        }
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
        var nextPos = transform.position + move * moveSpeed * Time.deltaTime;
        rigidBody.MovePosition(nextPos);
        if(inputAim == Vector2.zero && move != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(nextPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
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
        if(inputAim == Vector2.zero)
        {
            if(target.activeInHierarchy) target.SetActive(false);
            return;
        }

        if (!target.activeInHierarchy) target.SetActive(true);

        //KEYBOARD CONTROLS - START
        /*
        RaycastHit hit;
        //Input System
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(inputAim.x, inputAim.y, 0));

        //Old Input System
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayCastMaxDistance)) //da rendere efficiente considerando solo i layer necessari
        {
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
        */
        //KEYBOARD CONTROLS - END

        //TOUCH AND GAMEPAD CONTROL - START
        Debug.Log("GAMEPAD: " + inputAim);
        targetPos = new Vector3(bodyTranform.position.x + (inputAim.x * targetMaxDist), 1, bodyTranform.position.z + (inputAim.y * targetMaxDist));
        target.transform.position = targetPos;
        transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        //TOUCH AND GAMEPAD CONTRO - END
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

    public void SetHasGun(bool hasGun)
    {
        this.hasGun = hasGun;
    }

    public bool GetHasGun()
    {
        return hasGun;
    }

    public void SetEnableControl(bool enableControl)
    {
        this.enableControl = enableControl;
    }

    public bool GetEnableControl()
    {
        return enableControl;
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
