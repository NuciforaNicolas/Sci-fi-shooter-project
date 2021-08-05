using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon, IWeapon
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform spawnPoint;
    GameObject gunBullet;

    private void FixedUpdate()
    {
        if (bulletCounter == magazineSize && !isDropped)
        {
            DropGun();
            StartCoroutine("DeactiveGun");
        }
    }

    public override void Shoot()
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

    public override void DropGun()
    {
        base.DropGun();
    }

    protected IEnumerator SetCanPickUp()
    {
        yield return new WaitForSeconds(timeToPickUp);
        canPickUp = bulletCounter < magazineSize ? true : false;
    }

    public override void PickUp()
    {
        base.PickUp();
    }

    public override bool CanPickUp()
    {
        return base.CanPickUp();
    }

    IEnumerator DeactiveGun()
    {
        yield return new WaitForSeconds(timeToDeactive);
        SpawnWeaponManager.instance.IncreaseWeaponCounter(weaponType.ToString());
        gameObject.SetActive(false); 
    }
}
