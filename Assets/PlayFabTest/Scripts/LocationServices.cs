using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PhotonTesting {
    public class LocationServices : MonoBehaviour {

        public float latitude;
        public float longitude;

        IEnumerator Start() {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser) {
                Utilities.WriteDebugLog("location services are not enabled");
                // request access here
                yield break;
            }

            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            Utilities.WriteDebugLog("location services initializing...");
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1) {
                Utilities.WriteDebugLog("Timed out");
                yield break; 
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed) {
                Utilities.WriteDebugLog("Unable to determine device location");
                yield break;
            } else {
                // Access granted and location value could be retrieved
                Utilities.WriteDebugLog("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
            }

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }
    }
}