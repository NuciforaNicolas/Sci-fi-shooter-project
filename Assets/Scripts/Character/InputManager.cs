using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float moveSpeed, jumpForce, dashDistance, timeToDash, rayCastMaxDistance;
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

        if (Physics.Raycast(ray, out hit, rayCastMaxDistance, 1 << 8))
        {
            target.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    void Dash()
    {
        canDash = false;
        //Vector3 dashVelocity = Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * rigidBody.drag + 1)) / -Time.deltaTime)));
        //rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
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
