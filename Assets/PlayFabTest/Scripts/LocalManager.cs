using UnityEngine;
using UnityEngine.UI;

namespace PhotonTesting {
    public class LocalManager : MonoBehaviour {
        #region Private Serializable Fields

        [SerializeField]
        private Text debugLog;

        //[SerializeField]
        //private InputField roomName;

        [SerializeField]
        private GameObject authPanel;

        [SerializeField]
        private GameObject roomPanel;

        #endregion


        #region Utilities

        // writes to the debug log
        public void WriteDebugLog(string log) {
            Debug.Log(log);
            debugLog.text += "\nDEBUG: " + log;
        }

        // makes game objects active and inactive in scene
        public void ToggleActive(string obj, bool isActive) {
            switch (obj) {
                case "authPanel":
                    authPanel.SetActive(isActive);
                    break;
                case "roomPanel":
                    roomPanel.SetActive(isActive);
                    break;
            }
        }

        #endregion
    }
}