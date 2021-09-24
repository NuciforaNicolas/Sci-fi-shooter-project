using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBorder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        if (other.CompareTag("Player")) GameManager.instance.RestartLevel();
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")) other.GetComponent<Weapon>().DeactiveGun();
    }
}
