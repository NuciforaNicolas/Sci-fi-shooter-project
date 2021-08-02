using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float moveSpeed, jumpForce, dashDistance, timeToDash, targetMaxDist, rayCastMaxDistance;
    [SerializeField] GameObject target;
    [SerializeField] Transform weaponSlot;
    [SerializeField] bool canDash;
    Rigidbody rigidBody;
    Gun gun;
    Vector3 move;
    float horizontalMove, verticalMove, dashTimer;
    bool isGrounded;
    public bool hasGun { get; set; }
    public static InputManager instance;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        dashTimer = 0;
        canDash = true;
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

        if (Input.GetKey(KeyCode.Mouse0) && hasGun) gun.Shoot();
        if (Input.GetKey(KeyCode.Q) && hasGun) gun.DropGun();
        if (Input.GetKey(KeyCode.Space)) Jump();
        if (Input.GetKey(KeyCode.LeftShift) && canDash) Dash();
    }

    void Move()
    {
        //Movement START
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");
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

    void Aim()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayCastMaxDistance))
        {
            Vector3 targetPos;
            if(Vector3.Distance(hit.point, transform.position) <= targetMaxDist)
            {
                targetPos = hit.point;
            }
            else
            {
                targetPos = new Vector3(transform.position.x + (transform.forward.x * targetMaxDist), 1, transform.position.z + (transform.forward.z * targetMaxDist));
            }
            //target.transform.position = new Vector3(transform.position.x + targetPos.x, 1, transform.position.z + targetPos.z);
            target.transform.position = targetPos;
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    void Dash()
    {
        canDash = false;
        rigidBody.MovePosition(transform.position + move * dashDistance);
        StartCoroutine("DashCoolDown");
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(timeToDash);
        canDash = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Platform")) isGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gun") && !hasGun && other.transform.GetComponent<Gun>().CanPickUp())
        {
            other.transform.parent.transform.parent = weaponSlot.transform;
            other.transform.parent.transform.position = weaponSlot.transform.position;
            other.transform.parent.transform.rotation = weaponSlot.transform.rotation;
            other.gameObject.GetComponent<Gun>().PickUp();
            hasGun = true;
            gun = other.gameObject.GetComponent<Gun>();
        }
    }
}
