using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayFabAuth : MonoBehaviour {

    private string _playFabPlayerIdCache;

    [SerializeField]
    private Text debugLog;

    [Tooltip("Player email input field")]
    [SerializeField]
    private InputField emailInput;

    [SerializeField]
    private InputField passwordInput;

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
            Email = emailInput.text,
            Password = passwordInput.text,
            TitleId = titleID,
            RequireBothUsernameAndEmail = false,
//            DisplayName = nameInput.text
        }, RequestPhotonSignUpToken, OnPlayFabError);
    }

    public void OnLogin() {
        LogMessage("PlayFab login with email");
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest() {
            Email = emailInput.text,
            Password = passwordInput.text,
            TitleId = titleID
        }, RequestPhotonLoginToken, OnPlayFabError);
    }

    private void RequestPhotonSignUpToken(RegisterPlayFabUserResult obj) {
        LogMessage("successful sign up");

        // auto log in with sign up credentials
        OnLogin();
    }

    private void RequestPhotonLoginToken(LoginResult obj) {
        LogMessage("successful login");

        LogMessage("PlayFab authenticated. Requesting photon token...");

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

    public void OnPlay() {
        /*
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            PhotonNetwork.ConnectUsingSettings();
        }*/
    }

    /*
     * Step 1
     * We authenticate current PlayFab user normally. 
     * In this case we use LoginWithCustomID API call for simplicity.
     * You can absolutely use any Login method you want.
     * We use PlayFabSettings.DeviceUniqueIdentifier as our custom ID.
     * We pass RequestPhotonToken as a callback to be our next step, if 
     * authentication was successful.
     */
    /*
   private void AuthenticateWithPlayFab() {
       LogMessage("PlayFab authenticating using Custom ID...");

       PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest() {
           CreateAccount = true,
           CustomId = PlayFabSettings.DeviceUniqueIdentifier
       }, RequestPhotonToken, OnPlayFabError);
   }*/

    /*
    * Step 2
    * We request Photon authentication token from PlayFab.
    * This is a crucial step, because Photon uses different authentication tokens
    * than PlayFab. Thus, you cannot directly use PlayFab SessionTicket and
    * you need to explicitely request a token. This API call requires you to 
    * pass Photon App ID. App ID may be hardcoded, but, in this example,
    * We are accessing it using convenient static field on PhotonNetwork class
    * We pass in AuthenticateWithPhoton as a callback to be our next step, if 
    * we have acquired token succesfully
    */
    /*
    private void RequestPhotonToken(LoginResult obj) {
        LogMessage("PlayFab authenticated. Requesting photon token...");

        //We can player PlayFabId. This will come in handy during next step
        _playFabPlayerIdCache = obj.PlayFabId;

        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() {
            // NOTE FROM NICOLE: I had to change the line below because .PhotonServerSettings.AppID is incorrect even though that's what it says in the tutorial
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        }, AuthenticateWithPhoton, OnPlayFabError);
    }*/

    /*
     * Step 3
     * This is the final and the simplest step. We create new AuthenticationValues instance.
     * This class describes how to authenticate a players inside Photon environment.
     */
}