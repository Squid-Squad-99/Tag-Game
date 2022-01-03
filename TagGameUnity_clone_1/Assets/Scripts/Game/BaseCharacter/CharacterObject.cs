using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.Event;
using System;
using Tag.Game.Managers;

namespace Tag.Game.Character{
    
    /// <summary>
    /// Managing behaviour for a character, just like NetworkObject's desing pattern
    /// </summary>
    public class CharacterObject : NetworkBehaviour
    {
        public NetworkVariable<ulong> OwnerUserId = new NetworkVariable<ulong>();
        public NetworkList<char> OwnerUsername = new NetworkList<char>(); 
        public NetworkVariable<int> OwnerUserRank = new NetworkVariable<int>();
        public CharacterTypeEnum CharacterType;
        public bool OwnedByLocalUser;
        public enum CharacterTypeEnum{
            None,
            Human,
            Ghost,
        }
        
        [Header("Broadcast Channel")]
        [SerializeField] GameObjectEventChannelSO _characterGiveToUserChannel;
        [SerializeField] GameObjectEventChannelSO _characterSpawnChannel;
        

        [Header("Referecence")]
        public Transform PlayerVCamTarget;
        public void GiveToUser(ulong userId, PlayerConfig playerConfig){
            // set user id
            OwnerUserId.Value = userId;

            // set player config
            OwnerUsername.Clear();
            for(int i = 0; i < playerConfig.Username.Length; i++) OwnerUsername.Add(playerConfig.Username[i]);
            OwnerUserRank.Value = playerConfig.Rank;
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
            // set is owner
            OwnedByLocalUser = PlayerManager.Singleton.LocalUserId == OwnerUserId.Value;
            // broadcast character spawn event
            _characterSpawnChannel.RaiseEvent(gameObject);
        }

    }
    
}

