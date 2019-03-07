using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PhotonTesting {
    public class AuthManager : MonoBehaviour {

        #region Private Variables

        private string _playFabPlayerIdCache;
        private string titleID = "4339";

        #endregion


        #region Monobehaviour Callbacks

        // on start, check to see if login token is already present so players don't have to log in every time

        private void Awake() {
            Utilities.ToggleActive("authPanel", true);
            Utilities.ToggleActive("roomPanel", false);
        }

        public void OnAnonymousAndroidLogin() {
            AnonymousAuthenticateWithPlayFab();
        }

        #endregion


        #region PlayFab/Photon Authentication Methods

        // check for playfab login token

        /* Step 1
        * We authenticate current PlayFab user normally. 
        * In this case we use LoginWithCustomID API call for simplicity.
        * You can absolutely use any Login method you want.
        * We use PlayFabSettings.DeviceUniqueIdentifier as our custom ID.
        * We pass RequestPhotonToken as a callback to be our next step, if 
        * authentication was successful.
        */
        private void AnonymousAuthenticateWithPlayFab() {
            Utilities.WriteDebugLog("PlayFab anonymous login with android ID: " + SystemInfo.deviceUniqueIdentifier);
            PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest() {
                TitleId = titleID,
                CreateAccount = true,
                AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
            }, RequestPhotonToken, OnPlayFabError);
        }

        // prepare for cases in which a user's phone is sold after they already added an email to recover account

        /* Step 2
         * We request Photon authentication token from PlayFab.
         * This is a crucial step, because Photon uses different authentication tokens
         * than PlayFab. Thus, you cannot directly use PlayFab SessionTicket and
         * you need to explicitely request a token. This API call requires you to 
         * pass Photon App ID. App ID may be hardcoded, but, in this example,
         * We are accessing it using convenient static field on PhotonNetwork class
         * We pass in AuthenticateWithPhoton as a callback to be our next step, if 
         * we have acquired token successfully
         */
        private void RequestPhotonToken(LoginResult obj) {
            Utilities.WriteDebugLog("PlayFab authenticated with account: " + obj.PlayFabId + ". Requesting photon token...");

            _playFabPlayerIdCache = obj.PlayFabId;

            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() {
                // NOTE: I had to change the line below because .PhotonServerSettings.AppID is incorrect even though that's what it says in the tutorial
                PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
            }, AuthenticateWithPhoton, OnPlayFabError);
        }

        /* Step 3
         * This is the final and the simplest step.We create new AuthenticationValues instance.
         * This class describes how to authenticate a players inside Photon environment.
         */
        private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj) {
            Utilities.WriteDebugLog("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

            //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
            var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

            //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
            customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service

            //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
            customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

            //We finally tell Photon to use this authentication parameters throughout the entire application.
            PhotonNetwork.AuthValues = customAuth;


            Utilities.ToggleActive("roomPanel", true);
            Utilities.ToggleActive("authPanel", false);

//            if (!PhotonNetwork.IsConnected) {
                Utilities.WriteDebugLog("Connecting...");
                PhotonNetwork.ConnectUsingSettings();
//            }
        }

        // callback when playfab authentication encounters an error
        private void OnPlayFabError(PlayFabError obj) {
            Utilities.WriteDebugLog(obj.GenerateErrorReport());
        }

        #endregion
    }
}