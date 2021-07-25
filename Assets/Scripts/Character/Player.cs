using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] Transform weaponSlot;
    CharacterController controller;
    float health;
    bool hasGun;
    public static Player instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        health = maxHealth;
        SetHasGun(false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Gun") && !GetHasGun() && hit.transform.GetComponent<Gun>().CanPickUp())
        {
            Debug.Log("Picked: " + hit.gameObject.transform.parent.name);
            hit.transform.parent.transform.parent = weaponSlot.transform;
            hit.transform.parent.transform.position = weaponSlot.transform.position;
            hit.transform.parent.transform.rotation = weaponSlot.transform.rotation;
            hit.gameObject.GetComponent<Gun>().PickUp();
            SetHasGun(true);
            GetComponent<InputManager>().SetGun(hit.gameObject.GetComponent<Gun>());
        }
    }

    public bool GetHasGun()
    {
        return hasGun;
    }

    public void SetHasGun(bool _hasGun)
    {
        hasGun = _hasGun;
    }
}
