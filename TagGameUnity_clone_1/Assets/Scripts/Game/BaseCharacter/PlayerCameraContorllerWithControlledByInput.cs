using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using RigiArcher.CharacterInput;

namespace Tag.Game.Character{

    /// <summary>
    /// main job
    /// 1. player vcam controller
    /// 2. control the camera by using look input
    /// </summary>
    public class PlayerCameraContorllerWithControlledByInput : MonoBehaviour
    {
        [Header("Setting")]
        public float MouseSensitivity;
        public int TopClamp;
        public int BottomClamp;
        [Header("Reference")]
        [SerializeField] Transform _playerVCamTarget;
        private UnityEvent<Vector2> _inputLookEvent;
        private Vector2 _inputValue;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private void Awake() {
            // cache user specific input look event
            _inputLookEvent = GetComponent<ICharacterInputBroadcaster>().InputLookEvent;
        }

        private void OnEnable() {
            _inputLookEvent.AddListener(OnInputLook);
        }

        private void OnDisable() {
            _inputLookEvent.RemoveListener(OnInputLook);
        }

        private void LateUpdate() {
            // rotate camera
            RotateCamera(_inputValue);
        }

        private void OnInputLook(Vector2 inputValue)
        {
            _inputValue = inputValue;
        }

        private void RotateCamera(Vector2 inputValue)
		{
            _cinemachineTargetYaw += inputValue.x * Time.deltaTime * MouseSensitivity;
            _cinemachineTargetPitch += inputValue.y * Time.deltaTime * MouseSensitivity;

            // clamp angle
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -BottomClamp, TopClamp);

            _playerVCamTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
		}

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}
    }

}
