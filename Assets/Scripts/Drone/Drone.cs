using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] float speed, timeToDeactive, timeToDrop, rayCastMaxDist;
    [SerializeField] Transform weaponSlot;
    [SerializeField] bool lockCountDown, lockDrop;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!lockCountDown) DeactiveDrone();
        transform.Move(speed);
        
        if(Physics.Raycast(transform.position, -transform.up, out var hit, rayCastMaxDist, 1 << 8))
        {
           if(!lockDrop && hit.transform.CompareTag("Platform")) DropWeapon();
        }
    }

    void DeactiveDrone()
    {
        lockCountDown = true;
        StartCoroutine("CountDown");
    }

    void DropWeapon()
    {
        lockDrop = true;
        StartCoroutine("LetDropWeapon");
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(timeToDeactive);
        gameObject.SetActive(false);
        SpawnWeaponManager.instance.canSpawn = true;
        lockCountDown = false;
        lockDrop = false;
    }

    IEnumerator LetDropWeapon()
    {
        yield return new WaitForSeconds(timeToDrop);
        GameObject weapon = weaponSlot.GetChild(0).gameObject; //WeaponSlot -> Weapon
        Debug.Log("Drone Slot: " + weapon.name);
        if (weapon != null)  
        {
            Rigidbody weaponRb = weapon.GetComponent<Rigidbody>();
            weaponRb.isKinematic = false;
            weaponRb.useGravity = true;
            string weaponType = weapon.transform.GetChild(0).GetComponent<Gun>().GetWeaponType();
            Transform par = SpawnWeaponManager.instance.GetWeaponContainer(weaponType);
            weapon.transform.parent = par;
        }
    }
}
