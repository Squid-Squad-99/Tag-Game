using System;
 using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

using Ultility.Event;
using Ultility.Event.Network;

using RigiArcher.CharacterInput;

namespace Tag.Game.Character{

    /// <summary>
    /// raise user input event of specify user
    /// </summary>
    public class UserSpecifyInputBroadcaster : MonoBehaviour, ICharacterInputBroadcaster
    {
        [Header("listening Channel")]
        [SerializeField] NetworkVoidEventChannelSO UserFireChannel;
        [SerializeField] NetworkVoidEventChannelSO UserJumpChannel;
        [SerializeField] NetworkVector2EventChannelSO UserLookChannel;
        [SerializeField] NetworkVector2EventChannelSO UserMoveChannel;
        [SerializeField] VoidEventChannelSO LocalFireChannel;
        [SerializeField] VoidEventChannelSO LocalJumpChannel;
        [SerializeField] Vector2EventChannelSO LocalLookChannel;
        [SerializeField] Vector2EventChannelSO LocalMoveChannel;

        [Header("Broadcasting Event")]
        [SerializeField] UnityEvent _inputFireEvent;
        [SerializeField] UnityEvent _inputJumpEvent;
        [SerializeField] UnityEvent<Vector2> _inputLookEvent;
        [SerializeField] UnityEvent<Vector2> _inputMoveEvent;

        [Header("Debug")]
        [SerializeField] StringEventChannelSO RequestDebugDisplayChannel;

        private CharacterObject _characterObject;

        public UnityEvent InputJumpEvent => _inputJumpEvent;

        public UnityEvent<Vector2> InputLookEvent => _inputLookEvent;

        public UnityEvent<Vector2> InputMoveEvent => _inputMoveEvent;

        private void OnEnable() {
            // cache character Object
            _characterObject = GetComponent<CharacterObject>();

            if(NetworkManager.Singleton.IsServer){
                // listen user input channels
                UserFireChannel.OnEventRaised += OnUserFire;
                UserJumpChannel.OnEventRaised += OnUserJump;
                UserLookChannel.OnEventRaised += OnUserLook;
                UserMoveChannel.OnEventRaised += OnUserMove;
            }
            else{
                // listen user input channels
                LocalFireChannel.OnEventRaised += OnLocalFire;
                LocalJumpChannel.OnEventRaised += OnLocalJump;
                LocalLookChannel.OnEventRaised += OnLocalLook;
                LocalMoveChannel.OnEventRaised += OnLocalMove;
            }
        }

        private void OnDisable() {
            if(NetworkManager.Singleton.IsServer){
                UserFireChannel.OnEventRaised -= OnUserFire;
                UserJumpChannel.OnEventRaised -= OnUserJump;
                UserLookChannel.OnEventRaised -= OnUserLook;
                UserMoveChannel.OnEventRaised -= OnUserMove;
            }
            else{
                LocalFireChannel.OnEventRaised -= OnLocalFire;
                LocalJumpChannel.OnEventRaised -= OnLocalJump;
                LocalLookChannel.OnEventRaised -= OnLocalLook;
                LocalMoveChannel.OnEventRaised -= OnLocalMove;
            }
        }


        #region LocalInput_Client

        private void OnLocalFire()
        {
            if(IsThisUserInput(_characterObject.OwnerUserId.Value)){
                _inputFireEvent.Invoke();
            }
        }

        private void OnLocalJump()
        {
            if(IsThisUserInput(_characterObject.OwnerUserId.Value)){
                InputJumpEvent.Invoke();
            }
        }

        private void OnLocalLook(Vector2 inputValue)
        {
            if(IsThisUserInput(_characterObject.OwnerUserId.Value)){
                RequestDebugDisplayChannel.RaiseEvent($"Pos: {transform.position}");
                Debug.Log("user Look");
                InputLookEvent.Invoke(inputValue);
            }
        }

        private void OnLocalMove(Vector2 inputValue)
        {
            if(IsThisUserInput(_characterObject.OwnerUserId.Value)){
                InputMoveEvent.Invoke(inputValue);
            }
        }


        #endregion

        #region UserInput_Server


        private async void OnUserFire(ulong userId, double clientLocalTime)
        {
            if(IsThisUserInput(userId)){
                // wait to sync client time
                await SyncClientTime(clientLocalTime);
                // invoke event
                _inputFireEvent.Invoke();
            }
        }

        private async void OnUserJump(ulong userId, double clientLocalTime)
        {
            if(IsThisUserInput(userId)){
                // wait to sync client time
                await SyncClientTime(clientLocalTime);
                // invoke event
                InputJumpEvent.Invoke();
            }
        }
        
        private async void OnUserLook(ulong userId, double clientLocalTime, Vector2 inputValue)
        {
            if(IsThisUserInput(userId)){
                RequestDebugDisplayChannel.RaiseEvent($"Pos: {transform.position}");
                Debug.Log("user Look");
                // wait to sync client time
                await SyncClientTime(clientLocalTime);
                // invoke event
                InputLookEvent.Invoke(inputValue);
            }
        }

        private async void OnUserMove(ulong userId, double clientLocalTime, Vector2 inputValue)
        {
            if(IsThisUserInput(userId)){
                // wait to sync client time
                await SyncClientTime(clientLocalTime);
                // invoke event
                InputMoveEvent.Invoke(inputValue);
            }
        }

        private async Task SyncClientTime(double clientLocalTime){
            double serverTime = NetworkManager.Singleton.NetworkTimeSystem.ServerTime;
            double waitTime = clientLocalTime - serverTime;
            if(waitTime < 0){
                // TODO: handle wait negative time
                Debug.Log("Client's time is too slow");
            }
            else{
                await Task.Delay((int)(waitTime * 1000));
            }
        }
#endregion

        private bool IsThisUserInput(ulong userId){
            if(userId == _characterObject.OwnerUserId.Value){
                return true;
            }

            return false;
        }
    }

}
