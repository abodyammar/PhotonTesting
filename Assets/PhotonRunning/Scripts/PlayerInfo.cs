using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PhotonRunning
{

    public class PlayerInfo : MonoBehaviour
    {
        // Properties //

        public static PlayerInfo PI;

        public int mySelectedCharacter;

        public GameObject[] allCharacters;

        public Material[] allMaterials;

        // Default Methods //
        private void OnEnable()
        {
            // Create singeltion of the object

            if (PlayerInfo.PI == null)
            {
                PlayerInfo.PI = this;
            }
            else
            {
                if (PlayerInfo.PI != this)
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
            if (PlayerPrefs.HasKey("MyCharacter"))
            {
                mySelectedCharacter = PlayerPrefs.GetInt("MyCharacter");
            }
            else
            {
                mySelectedCharacter = 0;
                PlayerPrefs.SetInt("MyCharacter", mySelectedCharacter);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        // Class Methods //
    }
}
