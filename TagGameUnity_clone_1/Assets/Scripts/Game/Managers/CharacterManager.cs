using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.Event;

using Tag.Game.Character;
using Tag.Game.Managers;
using System;
using System.Threading.Tasks;

public class CharacterManager : NetworkBehaviour
{
    #region Singleton
    static public CharacterManager Singleton;
    private void Awake() {
        CharacterManager.Singleton = this;
    }
    #endregion

    [Header("Listening Channel")]
    [SerializeField] GameObjectEventChannelSO _characterSpawnChannel;

    // data
    public CharacterObject LocalCharacter;
    public List<CharacterObject> Characters = new List<CharacterObject>();
    public List<CharacterObject> Humans = new List<CharacterObject>();
    public List<CharacterObject> Ghostes = new List<CharacterObject>();

    public event Action AllCharacterReadiedEvent;
    

    private void Start() {
        // add new spawn character to characterManager
        _characterSpawnChannel.OnEventRaised += async (c)=>{
            Debug.Log("Add character to manager");
            CharacterObject characterObject = c.GetComponent<CharacterObject>();
            if(characterObject.OwnedByLocalUser){
                LocalCharacter = characterObject;
            }
            Characters.Add(characterObject);
            if(characterObject.CharacterType == CharacterObject.CharacterTypeEnum.Human){
                Humans.Add(characterObject);
            }
            else{
                Ghostes.Add(characterObject);
            }

            // check all character is readied
            if(IsServer){
                if(Characters.Count == PlayerManager.Singleton.Players.Count){
                    await Task.Delay(300);
                    AllCharacterReadiedEvent?.Invoke();
                }
            }
        };
    }
}
