using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tag.Backend;
using Ultility.ArgParser;

using Unity.Netcode;

using Ultility.Event;


namespace Tag.GameServer{

    /// <summary>
    /// Main class for managing player
    /// only can be use in game server
    /// </summary>
    [CreateAssetMenu(menuName = "Game Server/Player Manager")]
    public class PlayerManagerSO : ScriptableObject
    {
        public Dictionary<ulong, PlayerConfig> UserIdToPlayerConfigDict;
        public List<ulong> Players;

        [Header("BroadcastChannel")]
        [SerializeField] VoidEventChannelSO _onCompleteAddAllPlayers;
        [SerializeField] VoidEventChannelSO _onPlayerManagerInited;

        [Header("Listening Channel")]
        [SerializeField] VoidEventChannelSO _onNetworkManagerInstantiate;
        [SerializeField] UlongEventChannelSO _onClientConnectedChannel;

        [Header("Reference")]
        [SerializeField] ArgParserSO _argParser;

        private Dictionary<string, MatchMakingSDK.Ticket> _authIdToTicketDict; 
        private Dictionary<ulong, ulong> _clientIdToUserIdDict;

        public ulong GetUserIdByClientId(ulong clientId){
            return _clientIdToUserIdDict[clientId];
        }

        public async void InitPlayerManagerInServer(){
                // only run in server
                if(_argParser.Contain("gameServerId")  == false) return;
                
                Debug.Log("Initialize PlayerManager");

                // init variable
                UserIdToPlayerConfigDict = new Dictionary<ulong, PlayerConfig>();
                Players = new List<ulong>();
                _clientIdToUserIdDict = new Dictionary<ulong, ulong>();

                // add player by auth when client connect
                NetworkManager.Singleton.ConnectionApprovalCallback += AddPlayerByAuthId;

                // check is all player are add
                _onClientConnectedChannel.OnEventRaised += (clientId) => {
                    if(Players.Count == _authIdToTicketDict.Count){
                        // raise event when all player are added
                        Debug.Log("[PlayerManager] All player are added");
                        _onCompleteAddAllPlayers.RaiseEvent();
                    }
                };

                // get authId -> Ticket dict
                _authIdToTicketDict = await MatchMakingSDK.GetAuthIdToTicketDict(_argParser.GetArg("gameServerId"));

                // delegate player manager init finish
                _onPlayerManagerInited.RaiseEvent();
        }

        private void AddPlayer(ulong userId, PlayerConfig playerConfig){
            // add to UserIdToPlayerConfigDict
            if(UserIdToPlayerConfigDict.ContainsKey(userId)) {
                // if already have remove it and add it again
                UserIdToPlayerConfigDict.Remove(userId);
                _clientIdToUserIdDict.Remove(playerConfig.ClientId);
            }
            UserIdToPlayerConfigDict.Add(userId, playerConfig);
            _clientIdToUserIdDict.Add(playerConfig.ClientId, userId);

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
