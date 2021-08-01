using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shootTime, dropForce, timeToPickUp, rimeToDeactive;
    [SerializeField] int magazineSize;
    [SerializeField] Transform spawnPoint;
    Rigidbody rb;
    GameObject gunBullet;
    bool canShoot = true, isDropped, canPickUp = true;
    float bulletCounter = 0;

    public enum GunType
    {
        Pistol, SMG, MG, Shotgun, Sniper, RocketLauncher, GranadeLauncher
    }
    [SerializeField] GunType gunType;

    private void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (bulletCounter == magazineSize && !isDropped)
        {
            DropGun();
            StartCoroutine("DeactiveGun");
        }
    }

    public void Shoot()
    {
        if(canShoot && bulletCounter < magazineSize) StartCoroutine("SpawnBullet");
    }

    IEnumerator SpawnBullet()
    {
        canShoot = false;
        gunBullet = BulletManager.instance.GetBullet(bulletPrefab.GetComponent<Bullet>().GetBulletType());
        gunBullet.transform.position = spawnPoint.transform.position;
        gunBullet.transform.rotation = spawnPoint.transform.rotation;
        gunBullet.SetActive(true);
        bulletCounter++;
        yield return new WaitForSeconds(shootTime);
        canShoot = true;
    }

    public void DropGun()
    {
        transform.parent.transform.parent = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        GetComponent<Collider>().isTrigger = false;
        rb.AddForce(transform.parent.forward * dropForce, ForceMode.Impulse);
        isDropped = true;
        StartCoroutine("SetCanPickUp");
        InputManager.instance.hasGun = false;
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
        GetComponent<Collider>().isTrigger = true;
        rb.useGravity = false;
        canPickUp = false;
        Debug.Log("Gun: " + transform.parent.name);
    }

    public bool CanPickUp()
    {
        return canPickUp;
    }

    IEnumerator DeactiveGun()
    {
        yield return new WaitForSeconds(rimeToDeactive);
        transform.parent.gameObject.SetActive(false);
    }
}
