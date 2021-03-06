using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] float shootTime, dropForce, timeToPickUp, timeToDestroy;
    [SerializeField] int magazineSize;
    [SerializeField] Transform spawnPoint;
    [SerializeField] bool canPickUp;
    Rigidbody rb;
    bool canShoot = true, isDropped;
    float bulletCounter = 0;

    private void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (bulletCounter == magazineSize && !isDropped)
        {
            DropGun();
            StartCoroutine("DestroyGun");
        }
    }

    public void Shoot()
    {
        if(canShoot && bulletCounter < magazineSize) StartCoroutine("SpawnBullet");
    }

    IEnumerator SpawnBullet()
    {
        canShoot = false;
        Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
        bulletCounter++;
        yield return new WaitForSeconds(shootTime);
        canShoot = true;
    }

    public void DropGun()
    {
        transform.parent.transform.parent = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(transform.parent.forward * dropForce, ForceMode.Impulse);
        isDropped = true;
        StartCoroutine("SetCanPickUp");
        Player.instance.SetHasGun(false);
    }

    IEnumerator SetCanPickUp()
    {
        yield return new WaitForSeconds(timeToPickUp);
        canPickUp = bulletCounter < magazineSize ? true : false;
    }

    public void PickUp()
    {
        isDropped = false;
        rb.isKinematic = true;
        rb.useGravity = false;
        canPickUp = false;
    }

    public bool CanPickUp()
    {
        return canPickUp;
    }

    IEnumerator DestroyGun()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(transform.parent.gameObject);
    }
}
