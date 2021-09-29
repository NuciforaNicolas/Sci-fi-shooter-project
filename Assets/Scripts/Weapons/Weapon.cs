using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using Photon.Realtime;

public class Weapon : MonoBehaviourPunCallbacks, IWeapon
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
        transform.parent = SpawnWeaponManager.instance.GetWeaponContainer();
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

    protected IEnumerator StartDeactiveGunTimer()
    {
        yield return new WaitForSeconds(timeToDeactive);
        DeactiveWeaponAndIncreaseCounter();
    }

    public void ActiveWeapon()
    {
        Debug.Log("Weapon: calling ActiveWeaponRPC");
        photonView.RPC(nameof(ActiveWeaponRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    protected void ActiveWeaponRPC()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        gameObject.SetActive(true);
    }

    public void DeactiveWeapon()
    {
        photonView.RPC(nameof(DeactiveWeaponRPC), RpcTarget.AllBuffered);
    }

    public void DeactiveWeaponAndIncreaseCounter()
    {
        SpawnWeaponManager.instance.IncreaseWeaponCounter(weaponType.ToString() + 's');
        photonView.RPC(nameof(DeactiveWeaponRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    protected void DeactiveWeaponRPC()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        canPickUp = true;
        canShoot = true;
        bulletCounter = 0;
    }
}
