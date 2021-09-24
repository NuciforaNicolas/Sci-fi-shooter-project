using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class CameraManager : NetworkBehaviour
{
    [SerializeField] CinemachineVirtualCamera playerCamera;
    private void Start()
    {
        if (IsLocalPlayer) return;

        playerCamera.enabled = false;
    }
}
