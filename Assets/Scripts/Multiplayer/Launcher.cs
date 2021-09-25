using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Managers;

namespace Multiplayer
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable fields
        [Tooltip("The maximum number of player per room. When a room is full, it can't be joined by new players, and so new room we'll be created")]
        [SerializeField]
        private byte maxPlayerPerRoom = 6;

        #endregion

        #region Private fields

        /// <summary>
        /// This client's version number. Users are separated from each other by game version
        /// </summary>
        string gameVersion = "1";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from photon,
        /// we need to keep track of this to properly adjust the behaviour when we receive callback by photon.
        /// Typically this is used for the OnConnectedToMaster() callback
        /// </summary>
        bool isConnecting;

        #endregion

        #region Public fields
        [Tooltip("The UI Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("The UI label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        #endregion

        #region Monobehaviour callbacks

        private void Awake()
        {
            //# Critical
            //this make sure that we can cuse Photno.LoadLevel() on master client and all clinets in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        // Start is called before the first frame update
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Start the connection process
        /// - if already connected, we attempt joining a random room
        /// - if not yet connected, connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            if(string.IsNullOrEmpty(GameManager.instance.GetSelectedLevel()))
            {
                Debug.LogError("You must select a level before play");
                return;
            }

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            //We check if we are connected or not, we join if we are, else we initiate the connection to the server
            if(PhotonNetwork.IsConnected)
            {
                //#Critical: we need at this point to attempt joining a Random room. If it fails, we''l get notified in OnJoinRandomFailed() and we'll create one
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                //#Critical: we must first and foremost connect to Photon Online Server
                //keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected , so we need to know what to do then
                isConnecting =  PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                Debug.Log("PUN launcher: OnConnectedToMaster() was called by PUN");
                //#Critical: the first we try to do is to join a potential existing room. If there is, good, else we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Launcher: OnDisconnected() by PUN with reason {0}", cause);
            isConnecting = false;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Launcher: ONJoinRandomFailed() called by PUN. No random room avaiable, so we crate one.\nCalling: PhotonNetwork.CreateRoom");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Launcher: OnJoinedRoom() called by PUN. Now this client is in the room");
            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.LogFormat("Loading level {0}", GameManager.instance.GetSelectedLevel());
                PhotonNetwork.LoadLevel(GameManager.instance.GetSelectedLevel());
            }
        }

        #endregion
    }
}
