﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PhotonRunning
{

    public class MenuController : MonoBehaviour
    {

        public void OnClickCharacterPicker(int whichCharacter)
        {
            if (PlayerInfo.PI != null)
            {
                PlayerInfo.PI.mySelectedCharacter = whichCharacter;
                PlayerPrefs.SetInt("MyCharacter", whichCharacter);
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
