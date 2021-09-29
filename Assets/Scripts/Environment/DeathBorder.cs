using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using Photon.Realtime;

public class DeathBorder : MonoBehaviourPunCallbacks
{
    public Transform selectedPlatformToRespawn;
    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if(other.CompareTag("Player"))
        {
            var platforms = GameObject.FindGameObjectsWithTag("Platform");
            selectedPlatformToRespawn = platforms[Random.Range(0, platforms.Length)].transform;
            other.transform.parent.localPosition = new Vector3(selectedPlatformToRespawn.localPosition.x, 2f, selectedPlatformToRespawn.localPosition.z);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")) other.GetComponent<Weapon>().DeactiveWeaponAndIncreaseCounter();
        if (other.CompareTag("Platform"))
        {
            Debug.Log("Platform reached deathBorder");
            other.GetComponent<Platform>().DestroyPlatform();
        }

    }
}
