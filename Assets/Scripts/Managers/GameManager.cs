using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;

namespace Managers
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Private field
        string selectedLevel;
        [SerializeField]
        string playerPrefabsPathName; //I need to specify the prefab path or it'll give a null reference error
        #endregion

        #region Private serializeable field
        [Tooltip("The player prefab to spawn")]
        [SerializeField] 
        GameObject playerPrefab;

        [SerializeField] float spawnX, spawnZ;
        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load MainMenu scene
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.LogFormat("Player {0} joined the game", newPlayer.NickName);
            if(PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("Master client joined the game: {0}", newPlayer.NickName);
                //LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.LogFormat("Player {0} left the game", otherPlayer.NickName);
            if(PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("Master client left the game.");
                //LoadArena();
            }
        }

        #endregion

        #region Private Methods
        void LoadArena()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("PhotonNetwork: trying to load a level but we are not the master client");
            }
            Debug.LogFormat("PhotonNetwork: Loading Level...");
            PhotonNetwork.LoadLevel(selectedLevel);
        }
        #endregion

        #region Public methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        //WILL BE DELETED
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SelectLevel(string levelName)
        {
            selectedLevel = levelName;
        }

        public string GetSelectedLevel()
        {
            return selectedLevel;
        }

        #endregion

        #region MonoBehaviour callbacks
        public static GameManager instance;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void SpawnPlayer()
        {
            if (!playerPrefab) Debug.LogError("Missing playerPref", this);
            //We are in a romm. Spawn a character for the local player. It gets synced by using PhotonNetwork.Istantiate
            Debug.LogFormat("Istantiating local player");
            if(Character.Player.localPlayerInstance == null)
                PhotonNetwork.Instantiate(Path.Combine(playerPrefabsPathName, playerPrefab.name), new Vector3(UnityEngine.Random.Range(-spawnX, spawnX), 1, UnityEngine.Random.Range(-spawnZ, spawnZ)), Quaternion.identity, 0);
        }

        private void OnLevelWasLoaded(int level)
        {
            //if (!PhotonNetwork.LocalPlayer.IsLocal) return;
            if(level > 0) SpawnPlayer();
        }

        #endregion

    }
}


