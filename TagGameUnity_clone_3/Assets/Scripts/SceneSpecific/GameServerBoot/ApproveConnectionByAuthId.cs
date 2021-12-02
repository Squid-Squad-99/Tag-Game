using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using Ultility.ArgParser;

using Tag.Backend;
using System;

namespace Tag.ServerBooting{

    /// <summary>
    /// Approve a connection by examing the connection data is valid auth id
    /// </summary>
    public class ApproveConnectionByAuthId : MonoBehaviour
    {
            [Header("Reference")]
            [SerializeField] ArgParserSO _argParser;
            
            private Dictionary<string, MatchMakingSDK.Ticket> _authIdToTicketDict; 

            private void Start() {
                NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalTest;
            }

        private void ApprovalTest(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            // get this connection's auth id
            string authId = System.Text.Encoding.Default.GetString(connectionData);

            // check have _authIdToTicketDict
            if(_authIdToTicketDict == null) {
                Debug.LogError("[ApproveConnectionByAuthId] _authIdToTicketDict not set yet");
            }

            bool approve = _authIdToTicketDict.ContainsKey(authId);
            bool createPlayerObeject = false;
            uint? prefabHash = null;
            Vector3? positionToSpawnAt = null;
            Quaternion? rotationToSpawnWith = null;

            // call back to let networkManager approve or disapprove connection
            callback(createPlayerObeject, prefabHash, approve, positionToSpawnAt, rotationToSpawnWith);
        }

        private async void OnEnable() {
                _authIdToTicketDict = await MatchMakingSDK.GetAuthIdToTicketDict(_argParser.GetArg("gameServerId"));
                
            }
    }

} 
