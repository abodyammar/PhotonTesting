using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PhotonRunning
{
    public class MultiplayerSetting : MonoBehaviour
    {

        // Properties //
        public static MultiplayerSetting multiplayerSetting;

        public bool delayedStart;
        public int maxPlayer;

        public int menuScene;
        public int multiplayerScene;
        // Class Methods //
        // Default Methods //

        public void Awake()
        {
            // Create singeltion of the object

            if (MultiplayerSetting.multiplayerSetting == null)
            {
                MultiplayerSetting.multiplayerSetting = this;
            }
            else
            {
                if (MultiplayerSetting.multiplayerSetting != this)
                {
                    //Destroy any other instance
                    Destroy(this.gameObject);
                }
            }
            // Presisent Gameobject between scenes
            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // Class Methods //
    }
}