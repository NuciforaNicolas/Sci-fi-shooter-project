using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] CinemachineVirtualCamera playerCamera;
        [SerializeField] PhotonView photonView;
        private void Start()
        {
            if (photonView.IsMine) return;

            playerCamera.enabled = false;
        }
    }
}
