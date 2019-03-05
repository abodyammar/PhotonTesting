using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

//public class PlayFabAuth : MonoBehaviour {
public class PlayFabAuth : MonoBehaviourPunCallbacks {

    private string _playFabPlayerIdCache;

    [SerializeField]
    private Text roomListUI;

    [SerializeField]
    private Text debugLog;

    [Tooltip("Player email input field")]
    [SerializeField]
    private InputField emailInput;

    [SerializeField]
    private InputField passwordInput;

    [SerializeField]
    private InputField roomName;

    [SerializeField]
    private GameObject authPanel;

    [SerializeField]
    private GameObject roomPanel;

    private string titleID = "4339";

    private void Awake() {
        ToggleActive("authPanel", true);
        ToggleActive("roomPanel", false);
    }

    public void OnSignUp() {
        LogMessage("PlayFab signing up new user");
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest() {
            TitleId = titleID,
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false,
//            DisplayName = nameInput.text
        }, RequestPhotonSignUpToken, OnPlayFabError);
    }

    public void OnEmailLogin() {
        LogMessage("PlayFab login with email: " + emailInput.text);
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest() {
            TitleId = titleID,
            Email = emailInput.text,
            Password = passwordInput.text
        }, RequestPhotonLoginToken, OnPlayFabError);
    }

    public void OnAnonymousAndroidLogin() {
        LogMessage("PlayFab anonymous login with android ID: " + SystemInfo.deviceUniqueIdentifier);
        PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest() {
            TitleId = titleID,
            CreateAccount = true,
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
        }, RequestPhotonLoginToken, OnPlayFabError);
    }

    public void OnDisconnect() {
        LogMessage("Disconnecting from Photon server");
        PhotonNetwork.Disconnect();
    }

    public void OnCreateRoomByName() {
        if (PhotonNetwork.IsConnected) {
            LogMessage("creating room by name");
            PhotonNetwork.CreateRoom(roomName.text, new RoomOptions { MaxPlayers = 4 });
        } else {
            LogMessage("Not connected to online play");
//            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnJoinRoomByName() {
        if (PhotonNetwork.IsConnected) {
            LogMessage("joining room by name");
            PhotonNetwork.JoinRoom(roomName.text);
        } 
        else {
            LogMessage("Not connected to online play");
//            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void RefreshRooms() {
        
    }

    public void OnPlayOnline() {
        if (!PhotonNetwork.IsConnected) {
            LogMessage("Connecting...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        LogMessage("updating room list");
        foreach (RoomInfo room in roomList) {
            roomListUI.text += room;
        }
    }

    private void RequestPhotonSignUpToken(RegisterPlayFabUserResult obj) {
        LogMessage("Successful sign up");

        // auto log in with sign up credentials
        OnEmailLogin();
    }

    private void RequestPhotonLoginToken(LoginResult obj) {
        LogMessage("PlayFab authenticated with account: " + obj.PlayFabId + ". Requesting photon token...");

        _playFabPlayerIdCache = obj.PlayFabId;

        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() {
            // NOTE FROM NICOLE: I had to change the line below because .PhotonServerSettings.AppID is incorrect even though that's what it says in the tutorial
            // double check that this is what we actually need
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        }, AuthenticateWithPhoton, OnPlayFabError);
    }

    private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj) {
        LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
        customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

        //We finally tell Photon to use this authentication parameters throughout the entire application.
        PhotonNetwork.AuthValues = customAuth;


        ToggleActive("roomPanel", true);
        ToggleActive("authPanel", false);
    }

    private void OnPlayFabError(PlayFabError obj) {
        LogMessage(obj.GenerateErrorReport());
    }

    public void LogMessage(string message) {
        Debug.Log("PlayFab + Photon Example: " + message);
        debugLog.text += "\nDEBUG: " + message;
    }

    private void ToggleActive(string obj, bool isOn) {
        switch (obj) {
            case "authPanel":
                authPanel.SetActive(isOn);
                break;
            case "roomPanel":
                roomPanel.SetActive(isOn);
                break;
        }
    }

    public void OnJoinRandom() {
        if (PhotonNetwork.IsConnected) {
            LogMessage("Joining room...");
            PhotonNetwork.JoinRandomRoom();
        } else {
            LogMessage("Connecting...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// Called after the connection to the master is established and authenticated
    /// </summary>
    public override void OnConnectedToMaster() {
        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
//        if (PhotonNetwork.IsConnected) {
            LogMessage("OnConnectedToMaster");
        
//        }
    }

    /// <summary>
    /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely all rooms are full or no rooms are available. <br/>
    /// </remarks>
    public override void OnJoinRandomFailed(short returnCode, string message) {
        LogMessage("OnJoinRandomFailed. Could not join room.");

//        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }


    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause) {
        LogMessage("OnDisconnected " + cause);
    }

    public override void OnJoinedRoom() {
        LogMessage("OnJoinedRoom with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {

        } else {

        }
    }

    public void TotalPlayers() {
        LogMessage("current players in room = " + PhotonNetwork.CurrentRoom.PlayerCount);
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
            LogMessage("player " + player.Key + " = " + player.Value);
        }
    }

    /*
     * Step 1
     * We authenticate current PlayFab user normally. 
     * In this case we use LoginWithCustomID API call for simplicity.
     * You can absolutely use any Login method you want.
     * We use PlayFabSettings.DeviceUniqueIdentifier as our custom ID.
     * We pass RequestPhotonToken as a callback to be our next step, if 
     * authentication was successful.
     * 
     * Step 2
     * We request Photon authentication token from PlayFab.
     * This is a crucial step, because Photon uses different authentication tokens
     * than PlayFab. Thus, you cannot directly use PlayFab SessionTicket and
     * you need to explicitely request a token. This API call requires you to 
     * pass Photon App ID. App ID may be hardcoded, but, in this example,
     * We are accessing it using convenient static field on PhotonNetwork class
     * We pass in AuthenticateWithPhoton as a callback to be our next step, if 
     * we have acquired token succesfully
     * 
     * Step 3
     * This is the final and the simplest step. We create new AuthenticationValues instance.
     * This class describes how to authenticate a players inside Photon environment.
     */
}