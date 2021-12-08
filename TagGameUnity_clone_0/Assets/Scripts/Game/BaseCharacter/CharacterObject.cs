using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.Event;
using System;

namespace Tag.Game.Character{
    
    /// <summary>
    /// Managing behaviour for a character, just like NetworkObject's desing pattern
    /// </summary>
    public class CharacterObject : NetworkBehaviour
    {
        public NetworkVariable<ulong> OwnerUserId;
        
        [Header("Broadcast Channel")]
        [SerializeField] GameObjectEventChannelSO _characterGiveToUserChannel;
        [SerializeField] GameObjectEventChannelSO _characterSpawnChannel;
        

        [Header("Referecence")]
        public Transform PlayerVCamTarget;
        public void GiveToUser(ulong userId){
            // set user id
            OwnerUserId.Value = userId;
        }

        private void OnEnable() {
            OwnerUserId.OnValueChanged += OnOwnerUserIdChange;

        }

        private void OnDisable() {
            OwnerUserId.OnValueChanged -= OnOwnerUserIdChange;

        }
    
        private void Start() {
            // only run on server
            if(NetworkManager.Singleton.IsServer){
                // spawn this character for all client
                GetComponent<NetworkObject>().Spawn();
            }

        }

        private void OnOwnerUserIdChange(ulong previousValue, ulong newValue)
        {
            _characterGiveToUserChannel.RaiseEvent(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            // broadcast character spawn event
            _characterSpawnChannel.RaiseEvent(gameObject);
        }

    }
    
}

