using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Managers;

namespace Character
{
    public class Player : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields
        [SerializeField] float maxHealth;
        Rigidbody rigidbody;
        PhotonView photonView;
        float health;
        bool isStunned;
        #endregion

        #region
        [Tooltip("The local player instance. use this to know if the local player is represented in the scene")]
        public static GameObject localPlayerInstance;
        #endregion

        // Start is called before the first frame update
        void Awake()
        {
            health = maxHealth;
            rigidbody = GetComponent<Rigidbody>();
            photonView = GetComponent<PhotonView>();
            isStunned = false;
            //used by GameManager: we keep track of the local player instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine) Character.Player.localPlayerInstance = this.gameObject;

            //we flag as don't destroy on load so that instance survives level synchronization
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            if(photonView.IsMine){
                if (health <= 0f) GameManager.instance.LeaveRoom();
            }
        }

        public void Heal(float healValue)
        {
            if (health >= maxHealth) return;

            health += healValue;
            if (health > maxHealth) health = maxHealth;
        }

        public void GetDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                GetStunned();
            }
        }

        void GetStunned()
        {
            if (!isStunned)
            {
                isStunned = true;
                InputManager.instance.SetEnableControl(false);
                rigidbody.isKinematic = true;
                //TO REMOVE
                Debug.Log("STUNNED");
                transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
            }
        }

        public void Unstun()
        {
            isStunned = false;
            InputManager.instance.SetEnableControl(true);
            rigidbody.isKinematic = false;
            Debug.Log("UNSTUNNED");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(0, 255, 0);
        }

        public bool GetIsStunned() { return isStunned; }

        public void SetIsStunned(bool isStunned) { this.isStunned = isStunned; }

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine) return;
            if(other.gameObject.layer.Equals(LayerMask.NameToLayer("Bullet")))
            {
                GetDamage(other.GetComponent<Bullet>().GetDamage());
                Debug.Log("Hit: " + health);
            }
        }

        /// <summary>
        /// If we want to syncronize variables, we need to implements IPunObservable and method OnPhotonSerializeView
        /// </summary>
        /// <param name="stream">Stream is used to send data to all players</param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                //Local player: send data to other player
                stream.SendNext(health);
            }
            else
            {
                //Network Player: receive data
                this.health = (float)stream.ReceiveNext();
            }
        }
    }

}