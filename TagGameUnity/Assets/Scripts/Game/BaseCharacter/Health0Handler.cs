using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RigiArcher.StateMachineElement;

public class Health0Handler : MonoBehaviour
{
    // referecne
    StateMachine _stateMachine;

    private void Awake() {
        _stateMachine = GetComponent<StateMachine>();
    }

    private void Start() {
        // change to health 0 state when die
        GetComponent<IDamagable>().DieEvent += () => {
            if(_stateMachine.enabled == true) _stateMachine.SwitchState(_stateMachine.Health0State);
        };
    }
}
