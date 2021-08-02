using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    [SerializeField] protected float shootTime, dropForce, timeToPickUp, timeToDeactive;
    [SerializeField] protected int magazineSize;
    [SerializeField] protected Collider collider;
    public enum WeaponType
    {
        Pistol, SMG, MG, Shotgun, Sniper, RocketLauncher, GranadeLauncher
    }
    [SerializeField] protected WeaponType weaponType;

    protected Rigidbody rb;
    protected bool canShoot = true, isDropped, canPickUp = true;
    protected float bulletCounter = 0;

    public virtual void Shoot() { }
    public virtual void DropGun() {
        collider.isTrigger = false;
        transform.parent.transform.parent = SpawnWeaponManager.instance.GetWeaponContainer(GetWeaponType());
        rb.useGravity = true;
        rb.isKinematic = false;
        isDropped = true;
        StartCoroutine("SetCanPickUp");
        InputManager.instance.hasGun = false;
    }
    public virtual void PickUp() {
        isDropped = false;
        rb.isKinematic = true;
        collider.isTrigger = true;
        rb.useGravity = false;
        canPickUp = false;
        Debug.Log("Gun: " + transform.parent.name);
    }
    public virtual bool CanPickUp() { return canPickUp; }

    public string GetWeaponType()
    {
        return weaponType.ToString();
    }

}
