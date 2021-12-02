using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tag.Backend;
using Ultility.ArgParser;

using Unity.Netcode;

using Ultility.Event;


namespace Tag.GameConfig{

    [CreateAssetMenu(menuName = "Game Server/Player Manager")]
    public class PlayerManagerSO : ScriptableObject
    {
        public Dictionary<string, PlayerConfig> UserIdToPlayerConfigDict;
        public List<string> Players;

        [Header("BroadcastChannel")]
        [SerializeField] VoidEventChannelSO _onCompleteAddAllPlayers;

        [Header("Listening Channel")]
        [SerializeField] VoidEventChannelSO _onNetworkManagerInstantiate;
        [SerializeField] UlongEventChannelSO _onClientConnectedChannel;

        [Header("Reference")]
        [SerializeField] ArgParserSO _argParser;

        private Dictionary<string, MatchMakingSDK.Ticket> _authIdToTicketDict; 

        private void OnEnable() {
            // init variable
            UserIdToPlayerConfigDict = new Dictionary<string, PlayerConfig>();
            Players = new List<string>();

            // add player by auth when client connect
            _onNetworkManagerInstantiate.OnEventRaised += () => {
                NetworkManager.Singleton.ConnectionApprovalCallback += AddPlayerByAuthId;
            };

            // check is all player are add
            _onClientConnectedChannel.OnEventRaised += (clientId) => {
                if(Players.Count == _authIdToTicketDict.Count){
                    // raise event when all player are added
                    Debug.Log("[PlayerManager] All player are added");
                    _onCompleteAddAllPlayers.RaiseEvent();
                }
            };

            // get authId -> Ticket dict
            GetAUthIdToTicketDict();
        }

        private async void GetAUthIdToTicketDict(){
            // get authId -> Ticket dict
            _authIdToTicketDict = await MatchMakingSDK.GetAuthIdToTicketDict(_argParser.GetArg("gameServerId"));
        }

        private void AddPlayer(string userId, PlayerConfig playerConfig){
            // add to UserIdToPlayerConfigDict
            if(UserIdToPlayerConfigDict.ContainsKey(userId)) {
                // if already have remove it and add it again
                UserIdToPlayerConfigDict.Remove(userId);
            }
            UserIdToPlayerConfigDict.Add(userId, playerConfig);

            // add to players list
            if(Players.Contains(userId) == false){
                Players.Add(userId);
            }
        }

        private void AddPlayerByAuthId(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            // get this connection's auth id
            string authId = System.Text.Encoding.Default.GetString(connectionData);

            // check is dict set up yet and have this auth id
            if(_authIdToTicketDict == null || _authIdToTicketDict.ContainsKey(authId) == false){
                Debug.LogError("_authIdToTicketDict is null or not have this connection auth id");
            }

            // get ticket coresponed to auth id
            MatchMakingSDK.Ticket ticket = _authIdToTicketDict[authId];

            // add player
            AddPlayer(ticket.UserId, new PlayerConfig(clientId));
        }

    }

    public struct PlayerConfig{
        public ulong ClientId; 
        public PlayerConfig(ulong clientId){
            ClientId = clientId;
        }
    }

}
