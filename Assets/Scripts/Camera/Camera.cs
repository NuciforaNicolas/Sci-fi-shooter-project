using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class Camera : NetworkBehaviour
{
    private void Start()
    {
        if (IsLocalPlayer) return;
        gameObject.SetActive(false);
    }
}
