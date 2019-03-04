using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PhotonTesting
{ 
    public class PhotonLobby : MonoBehaviourPunCallbacks
    {

        // Properties //

        // Singeltion of this class
        public static PhotonLobby lobby;

        // Button 
        public GameObject battleButton;
        public GameObject cancleButton;

        // Default Methods //

        private void Awake()
        {
            lobby = this; // Creates the singeltion, lives within the Main Menu scene
        }
        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); // Connects to Master photon server
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Working");
        }

        // Class Methods //

        public override void OnConnectedToMaster()
        {
            Debug.Log("Plyaer has connected to the Photon Master server");
            battleButton.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined Room");
            //base.OnJoinedRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Tried to join a randomgame but failed. There must be no open games avilable");
            CreateRoom();
            //base.OnJoinRandomFailed(returnCode, message);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Tried to join a new room but failed. There must be a room with the same name");
            CreateRoom();
            //base.OnCreateRoomFailed(returnCode, message);
        }

        private void CreateRoom()
        {
            Debug.Log("Create Room");
            int randomRoomName = Random.Range(0, 10000);
            RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
            PhotonNetwork.CreateRoom("Room_" + randomRoomName, roomOps);
        }

        public void OnBattleButtonClicked()
        {
            Debug.Log("Battle Button Clicked");
            battleButton.SetActive(false);
            cancleButton.SetActive(true);
            PhotonNetwork.JoinRandomRoom(); // Try to join a random room
        }

        public void OnCancleButtonClicked()
        {
            Debug.Log("Cancle Button Clicked");
            cancleButton.SetActive(false);
            battleButton.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }



    }
}
