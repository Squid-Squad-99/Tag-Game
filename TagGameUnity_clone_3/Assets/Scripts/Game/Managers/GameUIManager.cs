using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

using Tag.Game.Character;
using System;

public class GameUIManager : NetworkBehaviour
{
    #region Singleton
    static public GameUIManager Singleton = null;

    private void Awake() {
        GameUIManager.Singleton = this;
    }
         
    #endregion
    // UI animator
    [SerializeField] Animator _animator;
    // timer
    [SerializeField] TextMeshProUGUI _timerText;
    // skull box
    [SerializeField] TextMeshProUGUI _collectedSkullBoxText;
    [SerializeField] Slider _collectedSkullBoxSlider;
    // player info
    [SerializeField] PlayerInfoList _playerInfoList;
    // mana
    [SerializeField] Slider _manaSlider;
    [SerializeField] TextMeshProUGUI _manaText;
    // kill score
    [SerializeField] TextMeshProUGUI _HumanKillText;
    [SerializeField] TextMeshProUGUI _GhostKillText;
    // game result
    [SerializeField] GameObject ResultPage;
    [SerializeField] TextMeshProUGUI _dealDamage;
    [SerializeField] GameObject _redResult;
    [SerializeField] TextMeshProUGUI _redReasonText;
    [SerializeField] GameObject _blueResult;
    [SerializeField] TextMeshProUGUI _blueReasonText;
    [SerializeField] string TimoutReason;
    [SerializeField] string KillAllHumanReason;
    [SerializeField] string CollectAllSkullBoxReason;

    private void Start() {
        // hook timer
        GameManager.Singleton.TimeLeft.OnValueChanged += (p, n) => {SetTimerUI();};

        // hook collected skull box 
        HumanMissionManager.Singleton.Need2CollectCnt.OnValueChanged += (p,n) => {SetCollectedSkullUI();};
        HumanMissionManager.Singleton.CurrentCollectCnt.OnValueChanged += (p,n) => {SetCollectedSkullUI();};

        // hook kill cnt
        GameManager.Singleton.HumanKillCnt.OnValueChanged += (p,n) => _HumanKillText.text = $"{n}";
        GameManager.Singleton.GhostKillCnt.OnValueChanged += (p,n) => _GhostKillText.text = $"{n}";


        // init UI when start game
        GameFlowManager.Singleton.BeforeStarGameEvent += () =>{
            SetTimerUI();
            SetCollectedSkullUI();
            // hook character UI
            HookCharacterUI();
            HookManaUI();
        };
    }

    public void ShowGameResult(GameManager.GameEndReasonEnum gameEndReason){
        Debug.Assert(IsServer);
        ShowGameResult_Logic(gameEndReason);
        ShowGameResulTClientRpc(gameEndReason);
    }

    [ClientRpc]
    private void ShowGameResulTClientRpc(GameManager.GameEndReasonEnum gameEndReason){
        ShowGameResult_Logic(gameEndReason);
    }

    private void ShowGameResult_Logic(GameManager.GameEndReasonEnum gameEndReason){
        // enable result page
        ResultPage.SetActive(true);
        // set title
        switch(gameEndReason){
            case GameManager.GameEndReasonEnum.TimeOut:
            _blueResult.SetActive(true);
            _blueReasonText.text = TimoutReason;
            break;
            case GameManager.GameEndReasonEnum.CollectEnoughSkullBox:
            _blueResult.SetActive(true);
            _blueReasonText.text = CollectAllSkullBoxReason;
            break;
            case GameManager.GameEndReasonEnum.HumanAllDie:
            _redResult.SetActive(true);
            _redReasonText.text = KillAllHumanReason;   
            break;
        }
        // set stats
        _dealDamage.text = $"{HitManager.Singleton.LocalCharacterDealDamage}";
    }


    public void PlayStartGameCutScene(){
        if(IsServer) return;
        if(CharacterManager.Singleton.LocalCharacter.CharacterType == CharacterObject.CharacterTypeEnum.Human){
            _animator.SetBool("HumanStart", true);
        }
        else{
            _animator.SetBool("GhostStart", true);
        }
    }

    private void HookManaUI()
    {
        if(IsClient){
            var magicManager = CharacterManager.Singleton.LocalCharacter.GetComponent<RigiArcher.Magic.MagicManager>();
            // hook
            magicManager.CurrentMana.OnValueChanged += (p,n) => {
                _manaSlider.value = n / magicManager.MaxMana;
                _manaText.text = $"{(int)n}";
            };
        }
    }

    private void HookCharacterUI(){
        if(IsServer) return;

        // hook which charcter type
        var charcterType = CharacterManager.Singleton.LocalCharacter.CharacterType;
        List<CharacterObject> specifyCharacterList = ((charcterType == CharacterObject.CharacterTypeEnum.Human) ?  
            CharacterManager.Singleton.Humans : CharacterManager.Singleton.Ghostes
        );

        // hook character UI
        CharacterObject localCharacterObject = CharacterManager.Singleton.LocalCharacter; 
        _playerInfoList.HookNInitCharacterUI(localCharacterObject, localCharacterObject.GetComponent<CharacterGameState>());
        for(int i = 0; i < specifyCharacterList.Count; i++){
            if(specifyCharacterList[i] != localCharacterObject){
                CharacterObject characterObject = specifyCharacterList[i];
                CharacterGameState characterGameState = characterObject.GetComponent<CharacterGameState>();
                _playerInfoList.HookNInitCharacterUI(characterObject, characterGameState);
            }
        }
    }

    private void SetTimerUI(){
        int time = GameManager.Singleton.TimeLeft.Value;
        int min = time / 60;
        int sec = time % 60;
        _timerText.text = $"{min:00} : {sec:00}";
    }

    private void SetCollectedSkullUI(){
        int needCnt = HumanMissionManager.Singleton.Need2CollectCnt.Value;
        int currentCnt = HumanMissionManager.Singleton.CurrentCollectCnt.Value;
        float ratio = (float) currentCnt / (float) needCnt;
        _collectedSkullBoxText.text = $"{currentCnt}/{needCnt}";
        _collectedSkullBoxSlider.value = ratio;
    }

}
