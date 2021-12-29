using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RigiArcher.StateMachineElement;

namespace RigiArcher.CharacterAction{

    [CreateAssetMenu(menuName = "State Machine/Actions/Face Camera Forward ActionSO")]
    public class FaceCameraForwardActionSO : ActionSO
    {
        [Header("Setting")]
        public float RotateSmoothTime;

        public override Action GetAction(StateMachine stateMachine)
        {
            return new FaceCameraForwardAction(this, stateMachine);
        }
    }

    public class FaceCameraForwardAction : Action
    {
        // reference
        private Rigidbody _rigidbody;
        private Transform _vCam;

        public FaceCameraForwardAction(ActionSO actionSO, StateMachine stateMachine) : base(actionSO, stateMachine){}

        public override void Awake()
        {
            // set up reference
            _rigidbody = ThisStateMachine.GetComponent<Rigidbody>();
            _vCam = ThisStateMachine.VCamTarget;
        }

        float _rotationVelocity = 0f;
        public override void FixedUpdate(){

            // get horizontal look direction
            Vector3 horizontalLookDir = new Vector3(_vCam.forward.x, 0, _vCam.forward.z);
            float targetYRotation = Quaternion.LookRotation(horizontalLookDir).eulerAngles.y;

            // get new rotation
            float rotation = Mathf.SmoothDampAngle(
                ThisStateMachine.transform.eulerAngles.y,
                targetYRotation,
                ref _rotationVelocity,
                ((FaceCameraForwardActionSO)OriginSO).RotateSmoothTime
            );

            // change rotation
            _rigidbody.MoveRotation(Quaternion.Euler(0f, rotation, 0f));
        }
        
    }

}
