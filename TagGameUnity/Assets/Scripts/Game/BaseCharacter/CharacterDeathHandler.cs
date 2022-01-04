using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using RigiArcher.StateMachineElement;
using Tag.Game.Character;
using RigiArcher.Magic;

public class CharacterDeathHandler : NetworkBehaviour
{
    // referecne
    StateMachine _stateMachine;
    CharacterObject _characterObject;
    Animation _animation;
    NetworkAnimation _networkAnimation;
    MagicManager _magicManager;
    CharacterGameState _characterGameState;
    
    int _animParamDie;

    private void Awake() {
        _stateMachine = GetComponent<StateMachine>();
        _characterObject = GetComponent<CharacterObject>();
        _animation = GetComponent<Animation>();
        _networkAnimation = GetComponent<NetworkAnimation>();
        _magicManager = GetComponent<MagicManager>();
        _characterGameState = GetComponent<CharacterGameState>();
        _animParamDie = Animator.StringToHash("Die");
    }

    private void Start() {
        // change to health 0 state when die
        _characterGameState.DieEvent += Died;
        _characterGameState.RebornEvent += ReBorn;
    }

    private void Died(){
        // set state
        if(_stateMachine.enabled == true) _stateMachine.SwitchState(_stateMachine.Health0State);

        if(IsServer){
            //  set animation
            _networkAnimation.SetBool(_animParamDie, true);

            // Set Mana
            _magicManager.CurrentMana.Value = 0;

            // set Health
            _characterGameState.Health.Value = 0;
        }
    }

    private void ReBorn(){
        // set state
        if(_stateMachine.enabled == true) _stateMachine.SwitchState(_stateMachine.PreviousState);

        if(IsServer){
            // set animation
            _networkAnimation.SetBool(_animParamDie, false);

            // set Health
            _characterGameState.Health.Value = _characterGameState.InitialHealth;
        }
    }
}
