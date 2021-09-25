using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Character
{
    public class Target : MonoBehaviourPunCallbacks
    {
        [SerializeField] PhotonView photonView;

        private void Start()
        {
            if (photonView.IsMine) return;
            gameObject.SetActive(false);
        }
    }
}
