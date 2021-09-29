using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using Photon.Realtime;

public class Gun : Weapon, IWeapon
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform spawnPoint;
    GameObject gunBullet;

    private void FixedUpdate()
    {   if (!PhotonNetwork.IsMasterClient) return;
        
        if (bulletCounter == magazineSize && !isDropped)
        {
            DropGun();
            StartCoroutine(nameof(StartDeactiveGunTimer));
        }
    }

    public override void Shoot()
    {
        if(canShoot && bulletCounter < magazineSize) StartCoroutine("SpawnBullet");
    }

    IEnumerator SpawnBullet()
    {
        canShoot = false;
        gunBullet = BulletManager.instance.GetBullet(bulletPrefab.GetComponent<Bullet>().GetBulletType() + 's'); //Es. PistolBullet + s = PistolBullets (passed to BulletManager to refer map)
        gunBullet.transform.position = spawnPoint.transform.position;
        gunBullet.transform.rotation = spawnPoint.transform.rotation;
        gunBullet.GetComponent<Bullet>().ActiveBullet();
        bulletCounter++;
        yield return new WaitForSeconds(shootTime);
        canShoot = true;
    }

    public override void DropGun()
    {
        base.DropGun();
    }

    public override void PickUp()
    {
        base.PickUp();
    }

    public override bool CanPickUp()
    {
        return base.CanPickUp();
    }
}
