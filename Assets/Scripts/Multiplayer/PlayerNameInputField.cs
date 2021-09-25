using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

namespace Multiplayer
{
    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game
    /// </summary>
    
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private constants

        //Store the playerPref key to avoid typos
        private string playerNamePrefKey = "PlayerName";

        #endregion

        #region Monobehaviout callbacks

        // Start is called before the first frame update
        void Start()
        {
            string defaultName = string.Empty;
            TMP_InputField _inputField = this.GetComponent<TMP_InputField>();
            if(_inputField != null)
            {
                if(PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the name of the player, and save it in playerPrefs for future sessions
        /// </summary>
        /// <param name="name">The name of the player</param>
        public void SetPlayerName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                Debug.LogError("Player name is null or empty");
                return;
            }

            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString(playerNamePrefKey, name);
        }
        #endregion
    }
}
