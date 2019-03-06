using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PhotonRunning
{
    public class AvatarSetup : MonoBehaviour
    {
        // Properties //
        private PhotonView PV;
        public int characterValue;

        // Default Methods //
        private void Awake()
        {
            // Best practice to keep this here in Awake
            PV = GetComponent<PhotonView>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if (PV.IsMine)
            {
                // RpcTarget.AllBuffered all who join will recive this messege no matter when
                PV.RPC("PRC_SetCharacterColor", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        [PunRPC]
        void PRC_SetCharacterColor(int whichCharacter)
        {
            characterValue = whichCharacter;
            GetComponent<Renderer>().material = PlayerInfo.PI.allMaterials[whichCharacter];
        }

        // Class Methods //
    }
}
