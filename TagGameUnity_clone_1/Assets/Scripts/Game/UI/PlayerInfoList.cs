using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using Tag.Game.Character;
using Unity.Netcode;

public class PlayerInfoList : MonoBehaviour
{
    [Serializable]
    public class PlayerInfo{
        public GameObject PlayerInfoGameObject;
        public TextMeshProUGUI RankText; 
        public TextMeshProUGUI UsernameText;
        public Slider HealthSlider;
        public Image HealthFill;
    }

    [SerializeField] List<PlayerInfo> _playerInfos;
    [SerializeField] Sprite _red, _blue;

    public void HookNInitCharacterUI(CharacterObject characterObject, CharacterGameState characterGameState){
        // find player info ui to insert
        PlayerInfo playerInfo = null;
        foreach(PlayerInfo p in _playerInfos){
            if(p.PlayerInfoGameObject.activeSelf == false){
                playerInfo = p;
                break;
            }
        }
        Debug.Assert(playerInfo != null);

        // set active
        playerInfo.PlayerInfoGameObject.SetActive(true);

        // set fill color
        playerInfo.HealthFill.sprite = (characterObject.CharacterType == CharacterObject.CharacterTypeEnum.Human)? _blue: _red;

        // init
        SetRankText(playerInfo, characterObject);
        SetUsernameText(playerInfo, characterObject);
        SetHealthSlider(playerInfo, characterGameState);

        // hook
        characterObject.OwnerUserRank.OnValueChanged += (p,n) => SetRankText(playerInfo, characterObject);
        characterObject.OwnerUsername.OnListChanged += (dum) => SetUsernameText(playerInfo, characterObject);
        characterGameState.Health.OnValueChanged += (p,n) => SetHealthSlider(playerInfo, characterGameState);
    }

    private void SetRankText(PlayerInfo playerInfo, CharacterObject characterObject){
        playerInfo.RankText.text = characterObject.OwnerUserRank.Value.ToString();
    }

    private void SetUsernameText(PlayerInfo playerInfo, CharacterObject characterObject){
        List<char> usernameTmp = new List<char>();
        for(int i = 0; i < characterObject.OwnerUsername.Count; i++){
            usernameTmp.Add(characterObject.OwnerUsername[i]);
        }
        string username = new string(usernameTmp.ToArray());
        playerInfo.UsernameText.text = username;
    }

    private void SetHealthSlider(PlayerInfo playerInfo, CharacterGameState characterGameState){
        playerInfo.HealthSlider.value = (float)characterGameState.Health.Value / (float)characterGameState.InitialHealth;
    }

}
