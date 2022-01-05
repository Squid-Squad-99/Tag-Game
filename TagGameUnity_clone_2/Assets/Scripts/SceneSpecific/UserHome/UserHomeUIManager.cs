using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tag.Backend;
using System;

public class UserHomeUIManager : MonoBehaviour
{
    public static UserHomeUIManager Singleton;
    private void Awake() {
        Singleton = this;
    }

    [SerializeField] Button _battleButton;
    [SerializeField] Button _cancelButton;
    [SerializeField] Button _switchButton;
    [SerializeField] TextMeshProUGUI _switchText;
    [SerializeField] GameObject Ghost;
    [SerializeField] GameObject Human;
    [SerializeField] TextMeshProUGUI _findingText;

    [SerializeField] TextMeshProUGUI _usernameText;

    public WebSDK.Account Account;


    public MatchMakingSDK.CharacterTypeEnum ChoosedCharacter = MatchMakingSDK.CharacterTypeEnum.Human;
    public bool IsFindingMatch = false;


    private async void Start() {
        // hook battle button
        _battleButton.onClick.AddListener(OnBattleButtonClick);
        _switchButton.onClick.AddListener(OnSwitch);

        // set ui
        Account = await WebSDK.GetUserAccount();
        _usernameText.text = Account.Username;
    }

    private void OnSwitch()
    {
        if(IsFindingMatch) return;

        if(ChoosedCharacter == MatchMakingSDK.CharacterTypeEnum.Human){
            ChoosedCharacter = MatchMakingSDK.CharacterTypeEnum.Ghost;
            _switchText.text = "Police";
            Ghost.SetActive(true);
            Human.SetActive(false);
        }
        else{
            ChoosedCharacter = MatchMakingSDK.CharacterTypeEnum.Human;
            _switchText.text = "Theif";
            Ghost.SetActive(false);
            Human.SetActive(true);
        }
    }   

    private void OnBattleButtonClick(){
        // set state
        IsFindingMatch = true;

        // set UI
        _findingText.gameObject.SetActive(true);
        _cancelButton.gameObject.SetActive(true);
        _battleButton.gameObject.SetActive(false);

        // start finding match
        MatchMakingManager.Singleton.FindMatch();

    }
}
