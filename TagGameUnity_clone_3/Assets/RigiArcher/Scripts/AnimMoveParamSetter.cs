using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RigiArcher.CharacterInput;
using System;
using Unity.Netcode;

public class AnimMoveParamSetter : NetworkBehaviour
{
    [Header("Setting")]
    [SerializeField] float _smoothChangeTime;
    private Animator _animator;
    private NetworkAnimation _networkAnimation;
    private ICharacterInputBroadcaster _characterInputBroadcaster;
    private Rigidbody _rigidbody;

    private int _animIdHorizontalSpeed;
    private int _animIdXInput;
    private int _animIdYInput;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _networkAnimation = GetComponent<NetworkAnimation>();
        _characterInputBroadcaster = GetComponent<ICharacterInputBroadcaster>();
        _rigidbody = GetComponent<Rigidbody>();


        _animIdHorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
        _animIdXInput = Animator.StringToHash("XInput");
        _animIdYInput = Animator.StringToHash("YInput");

    }

    private void OnEnable() {
        _characterInputBroadcaster.InputMoveEvent.AddListener(OnInputMove);
    }

    private void OnDisable() {
        _characterInputBroadcaster.InputMoveEvent.RemoveListener(OnInputMove);
    }

    private void Update() {
        float horizontalSpeed = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z).magnitude;
        if(IsServer) _networkAnimation.SetFloat(_animIdHorizontalSpeed, horizontalSpeed);
    }

    private float f1, f2;
    private void OnInputMove(Vector2 inputvalue)
    {
        if(inputvalue.x != _animator.GetFloat(_animIdXInput)){
            float v = Mathf.SmoothDamp(_animator.GetFloat(_animIdXInput), inputvalue.x, ref f1, _smoothChangeTime);
            if(IsServer) _networkAnimation.SetFloat(_animIdXInput, v);

        }
        if(inputvalue.y != _animator.GetFloat(_animIdYInput)){
            float v = Mathf.SmoothDamp(_animator.GetFloat(_animIdYInput), inputvalue.y, ref f2, _smoothChangeTime);
            if(IsServer) _networkAnimation.SetFloat(_animIdYInput, v);
        }
    }
}
