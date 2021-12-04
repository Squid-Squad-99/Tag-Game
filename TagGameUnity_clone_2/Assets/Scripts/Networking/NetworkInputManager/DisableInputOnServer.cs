using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tag.NetworkInputManager{

    /// <summary>
    /// Disable getting input in server (server should not have any input)
    /// </summary>
    [RequireComponent(typeof(PlayerInput), typeof(NetworkObject))]
    public class DisableInputOnServer : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private void Awake() {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start() {
            if(NetworkManager.Singleton.IsServer){
                // disable any action in server
                _playerInput.actions.Disable();
            }          
        }
    }

}
