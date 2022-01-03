using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RigiArcher;
using RigiArcher.StateMachineElement;
using Tag.Game.Character;
using Unity.Netcode;

namespace RigiArcher.CharacterAction{
    [CreateAssetMenu(menuName = "State Machine/Actions/Health 0 ActionSO")]
    public class Health0ActionSO : ActionSO
    {
        public override Action GetAction(StateMachine stateMachine)
        {
            return new Health0Action(this, stateMachine);
        }
    }

    public class Health0Action : Action
    {
        // referecne
        CharacterObject _characterObject;
        Animation _animation;
        NetworkAnimation _networkAnimation;
        int _animParamDie;

        public override void Awake()
        {
            _characterObject = ThisStateMachine.GetComponent<CharacterObject>();
            _animation = ThisStateMachine.GetComponent<Animation>();
            _networkAnimation = ThisStateMachine.GetComponent<NetworkAnimation>();
            _animParamDie = Animator.StringToHash("Die");

        }

        public override void OnStateEnter()
        {
            Debug.Log("Health 0 State");
            if(NetworkManager.Singleton.IsServer){
                _networkAnimation.SetBool(_animParamDie, true);
            }

            if(_characterObject.CharacterType == CharacterObject.CharacterTypeEnum.Human){

            }
            else{

            }
        }

        public Health0Action(ActionSO actionSO, StateMachine stateMachine) : base(actionSO, stateMachine){}
    }
}