using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Com.TeamSolar.PhotonTesting {
    public class TestLocationService : MonoBehaviour {
        [SerializeField]
        private Text debugLog;

        IEnumerator Start() {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser) {
                debugLog.text += "\nDEBUG: location services are not enabled";
                yield break;
            }

            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            debugLog.text += "\nDEBUG: location services initializing...";
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1) {
                debugLog.text += "\nDEBUG: Timed out";
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed) {
                debugLog.text += "\nDEBUG: Unable to determine device location";
                yield break;
            } else {
                // Access granted and location value could be retrieved
                debugLog.text += "\nDEBUG: Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
            }

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }
    }
}