using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using Photon.Realtime;

public class Drone : MonoBehaviourPunCallbacks
{
    [SerializeField] float speed, timeToDeactive, timeToDrop, rayCastMaxDist;
    [SerializeField] Transform weaponSlot;
    [SerializeField] bool lockCountDown, lockDrop;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!lockCountDown) DeactiveDroneWithCountdown();
        transform.Move(speed);

        if (Physics.Raycast(transform.position, -transform.up, out var hit, rayCastMaxDist, 1 << 8))
        {
            if (!lockDrop && hit.transform.CompareTag("Platform")) DropWeapon();
        }
    }

    public void ActiveDrone()
    {
        Debug.Log("Drone: calling ActiveDroneRPC");
        photonView.RPC(nameof(ActiveDroneRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ActiveDroneRPC()
    {
        gameObject.SetActive(true);
    }

    void DeactiveDroneWithCountdown()
    {
        lockCountDown = true;
        StartCoroutine("CountDown");
    }

    public void DeactiveDrone()
    {
        photonView.RPC(nameof(DeactiveDroneRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DeactiveDroneRPC()
    {
        gameObject.SetActive(false);   
    }

    void DropWeapon()
    {
        lockDrop = true;
        StartCoroutine("LetDropWeapon");
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(timeToDeactive);
        SpawnWeaponManager.instance.SetCanSpawn(true);
        lockCountDown = false;
        lockDrop = false;
        photonView.RPC(nameof(DeactiveDroneRPC), RpcTarget.AllBuffered);
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
            string weaponType = weapon.transform.GetComponent<Gun>().GetWeaponType();
            Transform container = SpawnWeaponManager.instance.GetWeaponContainer();
            weapon.transform.parent = container;
        }
    }
}
