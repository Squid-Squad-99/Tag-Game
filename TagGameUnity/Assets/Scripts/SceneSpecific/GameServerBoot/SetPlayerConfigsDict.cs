using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.ArgParser;
using Ultility.Event;

using Tag.Backend;

namespace Tag.ServerBooting{
    /// <summary>
    /// main job
    /// 1. Set up User Id -> Player config dictionary
    /// </summary>
    public class SetPlayerConfigsDict : MonoBehaviour
    {

        [Header("Reference")]
        [SerializeField] ArgParserSO _argParser;
        
        private Dictionary<string, MatchMakingSDK.Ticket> _authIdToTicketDict; 

        private void Start() {
            NetworkManager.Singleton.ConnectionApprovalCallback += AddClientToPlayerConfigDict;
        }

        private async void OnEnable() {
            _authIdToTicketDict = await MatchMakingSDK.GetAuthIdToTicketDict(_argParser.GetArg("gameServerId"));
            
        }

        private void AddClientToPlayerConfigDict(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            // get this connection's auth id
            string authId = System.Text.Encoding.Default.GetString(connectionData);

            // check is dict set up yet and have this auth id
            if(_authIdToTicketDict == null || _authIdToTicketDict.ContainsKey(authId) == false){
                Debug.LogError("_authIdToTicketDict is null or not have this connection auth id");
            }

            // get ticket coresponed to auth id
            MatchMakingSDK.Ticket ticket = _authIdToTicketDict[authId];

            
        }



    }



}

