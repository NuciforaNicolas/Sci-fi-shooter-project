
using UnityEngine;
using TMPro;
using System.Collections;

namespace Managers
{
    public class DebugManager : MonoBehaviour
    {
        TMP_Text log;
        [SerializeField] float timeToClear;
        public static DebugManager instance;

        void Awake()
        {
            instance = this;
            log = GameObject.FindGameObjectWithTag("DebugLog").GetComponent<TMP_Text>();
            if (!log) Debug.LogError("Missing Log component");
            DontDestroyOnLoad(this.gameObject);
        }

        public void Log(string msg)
        {
            log.text = msg;
            StartCoroutine(ClearLog());
        }

        IEnumerator ClearLog()
        {
            yield return new WaitForSeconds(timeToClear);
            log.text = string.Empty;
        }

        private void OnLevelWasLoaded(int level)
        {
            log = GameObject.FindGameObjectWithTag("DebugLog").GetComponent<TMP_Text>();
        }
    }

}