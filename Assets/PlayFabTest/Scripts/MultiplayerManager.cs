using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PhotonTesting {
    public class MultiplayerManager : MonoBehaviourPunCallbacks {
        #region PUN Callbacks


//        private float latitude = 0.0f;
//        private float longitude = 0.0f;

        IEnumerator Start() {
//        public IEnumerator SetNearby() {
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
                Utilities.WriteDebugLog("Device location retrieved");
                //    latitude = Mathf.Round(Input.location.lastData.latitude * 100000.0f) / 100000.0f;
                //    longitude = Mathf.Round(Input.location.lastData.longitude * 100000.0f) / 100000.0f;
                //    Utilities.WriteDebugLog("Location: " + latitude + " " + longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            }

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }




        public void OnLeaveRoom() {
            if (PhotonNetwork.InRoom == true) {
                Utilities.WriteDebugLog("Disconnecting from room");
                PhotonNetwork.LeaveRoom();
//                PhotonNetwork.Disconnect();
            }
        }

        // creates room with input name or null if empty
        public void OnCreateRoomByName() {
            if (PhotonNetwork.IsConnected) {
                float latitude = Mathf.Round(Input.location.lastData.latitude * 10000.0f) / 10000.0f;
                float longitude = Mathf.Round(Input.location.lastData.longitude * 10000.0f) / 10000.0f;
                Utilities.WriteDebugLog("Location: " + latitude + " " + longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

                Utilities.WriteDebugLog("creating room by name");

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 5;
                roomOptions.PublishUserId = true;
                roomOptions.CustomRoomPropertiesForLobby = new string[2]{ "lat", "lon" };
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "lat", latitude }, { "lon", longitude } };

                Utilities.WriteDebugLog("latitude = " + latitude + ", longitude = " + longitude);

                PhotonNetwork.CreateRoom(null, roomOptions);
                //PhotonNetwork.CreateRoom(roomName.text, new RoomOptions { MaxPlayers = 4, PublishUserId = true });
            } else {
                Utilities.WriteDebugLog("Not connected to online play");
            }
        }

        // joins room with unique name
        public void OnJoinRoomByName() {
            if (PhotonNetwork.IsConnected) {
                Utilities.WriteDebugLog("joining room by name");
                //PhotonNetwork.JoinRoom(roomName.text);
            } else {
                Utilities.WriteDebugLog("Not connected to online play");
            }
        }

        // joins any random room
        public void OnJoinRandom() {
            if (PhotonNetwork.IsConnected) {
                Utilities.WriteDebugLog("Joining random room...");
                PhotonNetwork.JoinRandomRoom();
            } else {
                Utilities.WriteDebugLog("Not connected to online play");
            }
        }

        public void OnJoinNearbyRoom() {
            if (PhotonNetwork.IsConnected) {
                float latitude = Mathf.Round(Input.location.lastData.latitude * 10000.0f) / 10000.0f;
                float longitude = Mathf.Round(Input.location.lastData.longitude * 10000.0f) / 10000.0f;
                Utilities.WriteDebugLog("Location: " + latitude + " " + longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

                Utilities.WriteDebugLog("Joining nearby room...");

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 5;
                roomOptions.PublishUserId = true;
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "lat", latitude }, { "lon", longitude } };

                Utilities.WriteDebugLog("latitude = " + latitude + ", longitude = " + longitude);

                PhotonNetwork.JoinRandomRoom(roomOptions.CustomRoomProperties, roomOptions.MaxPlayers);
            } else {
                Utilities.WriteDebugLog("Not connected to online play");
            }
        }

        // lists all players connected to the room in the debug log
        public void TotalPlayers() {
            Utilities.WriteDebugLog("current players in room = " + PhotonNetwork.CurrentRoom.PlayerCount);
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
                Utilities.WriteDebugLog("player " + player.Key + " = " + player.Value.ToStringFull() + ". userID = " + player.Value.UserId);
            }
        }

        #endregion


        #region PUN Callback Overrides

        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        public override void OnConnectedToMaster() {
            Utilities.WriteDebugLog("OnConnectedToMaster");
        }

        /// <summary>
        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        /// </summary>
        /// <remarks>
        /// Most likely all rooms are full or no rooms are available. <br/>
        /// </remarks>
        public override void OnJoinRandomFailed(short returnCode, string message) {
            Utilities.WriteDebugLog("OnJoinRandomFailed. Could not join room because: " + returnCode + ". " + message);
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnected(DisconnectCause cause) {
            Utilities.WriteDebugLog("OnDisconnected " + cause);
        }

        /// <summary>
        /// Called after joining a room.
        /// </summary>
        public override void OnJoinedRoom() {
            Utilities.WriteDebugLog("OnJoinedRoom with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        }

        #endregion
    }
}