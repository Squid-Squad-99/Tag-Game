using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RigiArcher;
using RigiArcher.StateMachineElement;

namespace RigiArcher.CharacterAction{


    [CreateAssetMenu(menuName = "State Machine/Actions/Transition By Game Event ActionSO")]
    public class TransitionByGameEventActionSO : ActionSO
    {

        public override Action GetAction(StateMachine stateMachine)
        {
            return new TransitionByGameEventAction(this, stateMachine);
        }
    }

    public class TransitionByGameEventAction : Action
    {
        public TransitionByGameEventAction(ActionSO actionSO, StateMachine stateMachine) : base(actionSO, stateMachine){}




    }
}