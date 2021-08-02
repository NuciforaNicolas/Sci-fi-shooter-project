using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void Shoot();
    void DropGun();
    void PickUp();
    bool CanPickUp();
}
