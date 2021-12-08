using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

using Ultility.Event;
using Ultility.ArgParser;

namespace Tag.ServerBooting{

    public class StartGameServerOnRequest : MonoBehaviour
    {
        [Header("Listenting Channel")]
        [SerializeField] VoidEventChannelSO _requestStartGameServer;

        [Header("Reference")]
        [SerializeField] ArgParserSO _serverArgParser;
        [SerializeField] UNetTransport _uNetTransport;

        private void OnEnable() {
            _requestStartGameServer.OnEventRaised += StartGameServer;
        }
        
        private void OnDisable() {
            _requestStartGameServer.OnEventRaised -= StartGameServer;
        }

        public void StartGameServer() {

            // set transport by arg
            _uNetTransport = GameObject.Find("NetworkManager").GetComponent<UNetTransport>();
            int port =Int32.Parse(_serverArgParser.GetArg("port"));
            _uNetTransport.ServerListenPort = port;

            // start server
            NetworkManager.Singleton.StartServer();
        }
    }

}
