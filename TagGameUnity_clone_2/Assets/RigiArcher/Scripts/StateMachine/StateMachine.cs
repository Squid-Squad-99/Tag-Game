using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace RigiArcher.StateMachineElement{


    public class StateMachine : NetworkBehaviour
    {
        [Tooltip("The first StateSO will be initial state")]
        [SerializeField] List<StateSO> _stateSOs; 
        [SerializeField] StateSO GameStartState;
        [SerializeField] StateSO GameEndState;
        public StateSO Health0State;
        private List<State> _states;
        private Dictionary<StateSO, State> _stateSO2StateDict;
        private Dictionary<State, StateSO> _state2SODict;

        [Header("State")]
        private State _currentState;
        public State PreviousState;
        public State CurrentState{
            get{return _currentState;}
            set{
                if(PreviousState == null) PreviousState = value;
                else {PreviousState = _currentState;}
                _currentState = value;
                _currentStateSO = _state2SODict[value];
            }
        }
        [SerializeField] StateSO _currentStateSO;

        // some common reference for state/action to use
        [Header("Common Reference")]
        public Transform VCamTarget;
        public NetworkAnimation CharacterAnimator;

        protected void Awake() {
            // init variable
            _states = new List<State>();
            _stateSO2StateDict = new Dictionary<StateSO, State>();
            _state2SODict = new Dictionary<State, StateSO>();

            // instantiate state from state SO
            foreach (StateSO stateSO in _stateSOs)
            {
                State state = stateSO.GetState(this); 
                // add to list
                _states.Add(state);
                // ad to dict
                _stateSO2StateDict.Add(stateSO, state);
                _state2SODict.Add(state, stateSO);
            }

            // call awake for each state
            foreach (State state in _states)
            {
                state.Awake();
            }
        }

        protected void Start() {
            // call start for each state
            foreach(State state in _states){
                state.Start();
            }

            // init state
            CurrentState = _states[0];
            CurrentState.OnStateEnter();

            HookGameEvent();
        }

        private void HookGameEvent()
        {
            if(GameManager.Singleton == null) return;

            GameManager.Singleton.StartGameEvent += () => {
                SwitchState(GameStartState);
            };

            GameManager.Singleton.GameEndEvent += (reason) => {
                SwitchState(GameEndState);
            };

        }

        public void SwitchState(StateSO newStateSO){
            // get new state
            State newState = _stateSO2StateDict[newStateSO];
            // state change
            CurrentState.OnStateExit();
            CurrentState = newState;
            CurrentState.OnStateEnter();
        }

        public void SwitchState(State newState){
            // state change
            CurrentState.OnStateExit();
            CurrentState = newState;
            CurrentState.OnStateEnter();
        }

        protected void Update() {
            CurrentState.Update();
        }

        protected void FixedUpdate() {
            CurrentState.FixedUpdate();
        }

        protected void LateUpdate() {
            CurrentState.LateUpdate();
        }

    }

}