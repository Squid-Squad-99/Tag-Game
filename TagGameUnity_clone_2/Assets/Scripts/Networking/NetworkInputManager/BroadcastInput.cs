using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

using Ultility.Event;
using Ultility.Event.Network;

using Tag.GameServer;

namespace Tag.NetworkInputManager{

    /// <summary>
    /// Broad cast input as SO event to server and in local
    /// </summary>
    public class BroadcastInput : NetworkBehaviour 
    {
        [Header("BroadCast Local Event")]
        [SerializeField] Vector2EventChannelSO _onMoveChannel;
        [SerializeField] Vector2EventChannelSO _onLookChannel;
        [SerializeField] VoidEventChannelSO _onJumpChannel;
        [SerializeField] VoidEventChannelSO _onFireChannel;

        [Header("BroadCast Network Event")]
        [SerializeField] NetworkVector2EventChannelSO _userMoveChannel;
        [SerializeField] NetworkVector2EventChannelSO _userLookChannel;
        [SerializeField] NetworkVoidEventChannelSO _userJumpChannel;
        [SerializeField] NetworkVoidEventChannelSO _userFireChannel;

        [Header("Reference")]
        [SerializeField] PlayerManagerSO _playerManager;

        private NetworkManager _networkManager;
        private NetworkTimeSystem _networkTimeSystem;

        private void Start() {
            _networkManager = NetworkManager.Singleton;
            _networkTimeSystem = _networkManager.NetworkTimeSystem;
        }

        public void OnMove(InputValue inputValue){
            Vector2 value = inputValue.Get<Vector2>();

            // raise local event
            Debug.Log($"Move input: {value}");
            _onMoveChannel.RaiseEvent(value);

            // raise event on server
            RaiseNetMoveChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime, value);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetMoveChannelServerRpc(ulong clientId, double localTime, Vector2 value){
            Debug.Log("server Raise user move channel");
            _userMoveChannel.RaiseEvent(_playerManager.GetUserIdByClientId(clientId), localTime, value);
        }

        public void OnLook(InputValue inputValue){
            Vector2 value = inputValue.Get<Vector2>();

            Debug.Log($"look input: {value}");
            _onLookChannel.RaiseEvent(value);

            RaiseNetLookChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime, value);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetLookChannelServerRpc(ulong clientId, double localTime, Vector2 value){
            Debug.Log("server Raise user look channel");
            _userLookChannel.RaiseEvent(_playerManager.GetUserIdByClientId(clientId), localTime, value);
        }

        public void OnJump(InputValue inputValue){
            Debug.Log("Jump input");
            _onJumpChannel.RaiseEvent();

            RaiseNetJumpChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetJumpChannelServerRpc(ulong clientId, double localTime){
            Debug.Log("server Raise user jump channel");
            _userJumpChannel.RaiseEvent(_playerManager.GetUserIdByClientId(clientId), localTime);
        }

        public void OnFire(InputValue inputValue){
            Debug.Log("Fire input");
            _onFireChannel.RaiseEvent();
            
            RaiseNetFireChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetFireChannelServerRpc(ulong clientId, double localTime){
            Debug.Log("server Raise user fire channel");
            _userFireChannel.RaiseEvent(_playerManager.GetUserIdByClientId(clientId), localTime);
        }
    }
    
}
