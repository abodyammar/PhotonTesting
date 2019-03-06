using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhotonRunning
{

    public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        // Properties //
        // Room info
        public static PhotonRoom room;
        // Used to send RPC messeges
        private PhotonView PV;

        public bool isGameLoaded;
        public int currentScene;

        // Player info
        private Player[] photonPlayers;
        public int playersInRoom;
        public int myNumberInRoom;

        public int playerInGame;

        // Delayed Start
        private bool readyToCount;
        private bool readyToStart;
        public float startingTime;
        private float lessThanMaxPlayers;
        private float atMaxPlayers;
        private float timeToStart;

        // Default Methods //

        private void Awake()
        {
            // If going to double load this singleton, Destory it after user in privois scene, such as a Disconnet Player
            // Create singeltion of the object
            if (PhotonRoom.room == null)
            {
                PhotonRoom.room = this;
            }
            else
            {
                if (PhotonRoom.room != this)
                {
                    // Replace the instance game object with a new one as it is replaced
                    Destroy(PhotonRoom.room.gameObject);
                    PhotonRoom.room = this;
                }
            }
            // Presisent Gameobject between scenes
            DontDestroyOnLoad(this.gameObject);

            // Best practice to keep this here in Awake
            PV = GetComponent<PhotonView>();
        }
        // Class Methods //


        // Base override functions
        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
            // Event Listiner we are attaching function to
            SceneManager.sceneLoaded += OnSceneFinishedLoading; 
        }
        // Base override functions
        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
            // Removing Event Listner
            SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        }

        // Default Methods //

        // Start is called before the first frame update
        void Start()
        {
            // PV = GetComponent<PhotonView>(); moved to Awake for stablity issues 
            readyToCount = false;
            readyToStart = false;

            lessThanMaxPlayers = startingTime;
            atMaxPlayers = 6;
            timeToStart = startingTime;
         
        }

        // Update is called once per frame
        void Update()
        {
            // On Delayed Start
            if (MultiplayerSetting.multiplayerSetting.delayedStart)
            {
                // Check if players in room is greater than 1
                if (playersInRoom == 1)
                {
                    RestartTimer();
                }
                if (!isGameLoaded)
                {
                    if (readyToStart)
                    {
                        // Decrementing the timer to play
                        atMaxPlayers -= Time.deltaTime;
                        lessThanMaxPlayers = atMaxPlayers;
                        timeToStart = atMaxPlayers;
                        Debug.Log("Display time to start to the players " + timeToStart);
                    }
                    else if (readyToCount)
                    {
                        lessThanMaxPlayers -= Time.deltaTime;
                        timeToStart = lessThanMaxPlayers;
                        Debug.Log("Display time to start to the players " + timeToStart);
                    }

                    // When timer reaches 0 start game
                    if(timeToStart <= 0)
                    {
                        StartGame();
                    }
                }
            }
        }

        // Class Methods //

        // Based override class of Photon
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("We are now in a room");
            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom = photonPlayers.Length;
            myNumberInRoom = playersInRoom;

            // Name the room
            PhotonNetwork.NickName = myNumberInRoom.ToString();


            // Checking for delayed start
            if (MultiplayerSetting.multiplayerSetting.delayedStart)
            {
                Debug.Log("Display players in room out of max players possibel (" + playersInRoom + ":" + MultiplayerSetting.multiplayerSetting.maxPlayer + ")");
                // Check if players in room is greater than 1
                if (playersInRoom > 1)
                {
                    readyToCount = true;
                }
                // Check if is full then will be ready to start
                if (playersInRoom == MultiplayerSetting.multiplayerSetting.maxPlayer)
                {
                    readyToStart = true;
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }
                    // Room is CLOSED so no new players can join 
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                }
            }
            else
            {
                StartGame();
            }


        }


        // When player enters room
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log("A new player has joined the room");
            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom++;

            // Checking for delayed start
            if (MultiplayerSetting.multiplayerSetting.delayedStart)
            {
                Debug.Log("Display players in room out of max players possibel (" + playersInRoom + ":" + MultiplayerSetting.multiplayerSetting.maxPlayer + ")");
                // Check if players in room is greater than 1
                if (playersInRoom > 1)
                {
                    readyToCount = true;
                }
                // Check if is full then will be ready to start
                if (playersInRoom == MultiplayerSetting.multiplayerSetting.maxPlayer)
                {
                    readyToStart = true;
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }
                    // Room is CLOSED so no new players can join 
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                }
            }

        }


        // Will load the level and startGame
        void StartGame()
        {
            isGameLoaded = true;
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            if (MultiplayerSetting.multiplayerSetting.delayedStart)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }

            // can be just a number 
            PhotonNetwork.LoadLevel(MultiplayerSetting.multiplayerSetting.multiplayerScene);

        }

        void RestartTimer()
        {
            lessThanMaxPlayers = startingTime;
            timeToStart = startingTime;
            atMaxPlayers = 6;
            readyToCount = false;
            readyToStart = false;
        }

        void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            currentScene = scene.buildIndex;
            if (currentScene == MultiplayerSetting.multiplayerSetting.multiplayerScene)
            {
                isGameLoaded = true;
                if (MultiplayerSetting.multiplayerSetting.delayedStart)
                {
                    // Function RPC to send messeges, used with a target indentifer 
                    PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
                }
                else
                {
                    RPC_CleatePlayer();
                }
            }
        }

        [PunRPC] // MUST HAVE TAG FOR ALL RPC FUNCTIONS
        private void RPC_LoadedGameScene()
        {
            playerInGame++;
            if (playerInGame == PhotonNetwork.PlayerList.Length)
            {
                // Sends RPC to all clients to create Players
                PV.RPC("RPC_CleatePlayer", RpcTarget.All);
            }
        }

        [PunRPC] // MUST HAVE TAG FOR ALL RPC FUNCTIONS
        private void RPC_CleatePlayer()
        {
            // Objet is not the avatar the player is controlling
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
        }

        // End of Class
    }

}
