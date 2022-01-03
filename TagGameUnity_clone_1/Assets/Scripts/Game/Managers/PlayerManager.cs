using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.Event;
using Ultility.ArgParser;
using Tag.Backend;
using System;
using System.Threading.Tasks;

namespace Tag.Game.Managers{

    public class PlayerManager : NetworkBehaviour
    {
        static public PlayerManager Singleton;
        private void Awake() {
            Singleton = this;
        }

        public Dictionary<ulong, PlayerConfig> UserIdToPlayerConfigDict;
        public List<ulong> Players;
        public ulong LocalUserId;


        [Header("BroadcastChannel")]
        [SerializeField] VoidEventChannelSO _onCompleteAddAllPlayers;
        [SerializeField] VoidEventChannelSO _onPlayerManagerInited;

        [Header("Listening Channel")]
        [SerializeField] UlongEventChannelSO _onClientConnectedChannel;

        [Header("Reference")]
        [SerializeField] ArgParserSO _argParser;

        private Dictionary<string, MatchMakingSDK.Ticket> _authIdToTicketDict; 
        private Dictionary<ulong, ulong> _clientIdToUserIdDict;

        public ulong GetUserIdByClientId(ulong clientId){
            return _clientIdToUserIdDict[clientId];
        }

        public override void OnNetworkSpawn()
        {
            if(IsServer) return;

            // get local user id from game server
            AskLocalUserIdServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        void AskLocalUserIdServerRpc(ulong clientId){

            // only send to the calling client
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]{clientId}
                }
            };

            // set client's local user id
            SetLocalUserIdClientRpc(GetUserIdByClientId(clientId), clientRpcParams);

        }

        [ClientRpc]
        void SetLocalUserIdClientRpc(ulong userId, ClientRpcParams clientRpcParams){
            LocalUserId = userId;
        }

        private void OnEnable() {
            _onClientConnectedChannel.OnEventRaised += OnClientConnect;
        }

        private void OnDisable() {
            _onClientConnectedChannel.OnEventRaised -= OnClientConnect;
        }

        private void Start() {
            InitPlayerManagerInServer();
        }

        private void OnClientConnect(ulong clientId)
        {
            if(IsClient) return;
            CheckAllPlayerAdded();
        }

        private void CheckAllPlayerAdded(){
            if(Players.Count == _authIdToTicketDict.Count){
                // raise event when all player are added
                Debug.Log("[PlayerManager] All player are added");
                _onCompleteAddAllPlayers.RaiseEvent();
            }
        }

        private async void InitPlayerManagerInServer(){
                // only run in server
                if(_argParser.GetArg("gameServerId")  == "-1") return;
                
                Debug.Log("Initialize PlayerManager");

                // init variable
                UserIdToPlayerConfigDict = new Dictionary<ulong, PlayerConfig>();
                Players = new List<ulong>();
                _clientIdToUserIdDict = new Dictionary<ulong, ulong>();

                // add player by auth when client connect
                NetworkManager.Singleton.ConnectionApprovalCallback += AddPlayerByAuthId;

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
            AddPlayer(ticket.UserId, new PlayerConfig(clientId, ticket));
        }
    }

    public struct PlayerConfig{
        public ulong ClientId; 
        public string Username;
        public int Rank;
        public PlayerConfig(ulong clientId, MatchMakingSDK.Ticket ticket){
            ClientId = clientId;
            Username = ticket.Username;
            Rank = ticket.Rank;
        }
    }

}
