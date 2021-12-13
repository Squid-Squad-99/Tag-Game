using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

using Ultility.Event;
using Ultility.Event.Network;

using Tag.Game.Managers;

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


        [Header("Setting")]
        [SerializeField] bool _logLevel = false;

        private NetworkManager _networkManager;
        private NetworkTimeSystem _networkTimeSystem;

        private Vector2 _moveInputValue;
        private Vector2 _lookInputValue;

        private void Start() {
            _networkManager = NetworkManager.Singleton;
            _networkTimeSystem = _networkManager.NetworkTimeSystem;
        }

        private void Log(string s){
            if(_logLevel == true) Debug.Log(s);
        }

        private void Update() {
            StartCoroutine(Foo());
        }

        IEnumerator Foo(){
            yield return new WaitForSeconds(2f);
            // move
            // raise local event
            _onMoveChannel.RaiseEvent(_moveInputValue);
            // raise event on server
            RaiseNetMoveChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime, _moveInputValue);

            // look
            // raise local event
            _onLookChannel.RaiseEvent(_lookInputValue);
            // raise event on server
            RaiseNetLookChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime, _lookInputValue);

            yield return new WaitForSeconds(0.1f);
        }

        public void OnMove(InputValue inputValue){
            _moveInputValue = inputValue.Get<Vector2>();

        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetMoveChannelServerRpc(ulong clientId, double localTime, Vector2 value){
            Log("server Raise user move channel");
            _userMoveChannel.RaiseEvent(PlayerManager.Singleton.GetUserIdByClientId(clientId), localTime, value);
        }

        public void OnLook(InputValue inputValue){
            _lookInputValue = inputValue.Get<Vector2>();

        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetLookChannelServerRpc(ulong clientId, double localTime, Vector2 value){
            Log("server Raise user look channel");
            _userLookChannel.RaiseEvent(PlayerManager.Singleton.GetUserIdByClientId(clientId), localTime, value);
        }

        public void OnJump(InputValue inputValue){
            Log("Jump input");
            _onJumpChannel.RaiseEvent();

            RaiseNetJumpChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetJumpChannelServerRpc(ulong clientId, double localTime){
            Log("server Raise user jump channel");
            _userJumpChannel.RaiseEvent(PlayerManager.Singleton.GetUserIdByClientId(clientId), localTime);
        }

        public void OnFire(InputValue inputValue){
            Log("Fire input");
            _onFireChannel.RaiseEvent();
            
            RaiseNetFireChannelServerRpc(_networkManager.LocalClientId, _networkTimeSystem.LocalTime);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RaiseNetFireChannelServerRpc(ulong clientId, double localTime){
            Log("server Raise user fire channel");
            _userFireChannel.RaiseEvent(PlayerManager.Singleton.GetUserIdByClientId(clientId), localTime);
        }
    }
    
}
