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
    protected bool canShoot, isDropped;
    [SerializeField] protected bool canPickUp;
    protected float bulletCounter;

    protected virtual void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        collider = transform.GetChild(0).GetComponent<Collider>();
        canPickUp = true;
        canShoot = true;
        bulletCounter = 0;
    }

    public virtual void Shoot() { }
    public virtual void DropGun() {
        collider.isTrigger = false;
        transform.parent = SpawnWeaponManager.instance.GetWeaponContainer(GetWeaponType());
        //rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(transform.forward * dropForce, ForceMode.Impulse);
        isDropped = true;
        StartCoroutine("SetCanPickUp");
        InputManager.instance.SetHasGun(false);
    }
    public virtual void PickUp() {
        isDropped = false;
        rb.isKinematic = true;
        collider.isTrigger = true;
        //rb.useGravity = false;
        canPickUp = false;
        Debug.Log("Gun: " + transform.parent.name);
    }
    public virtual bool CanPickUp() { return canPickUp; }

    public string GetWeaponType()
    {
        return weaponType.ToString();
    }

    protected IEnumerator SetCanPickUp()
    {
        yield return new WaitForSeconds(timeToPickUp);
        canPickUp = bulletCounter < magazineSize ? true : false;
    }

    protected IEnumerator DeactiveGun()
    {
        yield return new WaitForSeconds(timeToDeactive);
        SpawnWeaponManager.instance.IncreaseWeaponCounter(weaponType.ToString());      
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        canPickUp = true;
        canShoot = true;
        bulletCounter = 0;
    }
}
