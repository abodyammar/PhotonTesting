using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace PhotonRunning
{
    public class PhotonPlayer : MonoBehaviour
    {
        // Properties //

        private PhotonView PV;
        public GameObject myAvatar;

        // Default Methods //
        private void Awake()
        {
            // Best practice to keep this here in Awake
            PV = GetComponent<PhotonView>();
        }
        // Start is called before the first frame update
        void Start()
        {
            int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
            if(PV.IsMine)
            {
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonAvatar"), 
                    GameSetup.GS.spawnPoints[spawnPicker].position, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Class Methods //
    }
}