using UnityEngine;
using UnityEngine.EventSystems;

namespace PhotonTesting {
    public class Utilities : MonoBehaviour {
        private static LocalManager theMgr;

        public static LocalManager localMgr
        {
            get
            {
                if (theMgr == null)
                {
                    theMgr = EventSystem.current.GetComponent<LocalManager>();
                }
                return theMgr;
            }
        }

        public static void ToggleActive(string obj, bool isActive)
        {
            LocalManager mgr = Utilities.localMgr;
            if (mgr != null)
            {
                mgr.ToggleActive(obj, isActive);
            }
            else
            {
                Debug.LogWarning("Utilities script missing!");
            }
        }

        public static void WriteDebugLog(string log)
        {
            LocalManager mgr = Utilities.localMgr;
            if (mgr != null)
            {
                mgr.WriteDebugLog(log);
            }
            else
            {
                Debug.LogWarning("Utilities script missing!");
            }
        }
    }
}