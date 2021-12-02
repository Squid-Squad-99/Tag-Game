using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

using Ultility.ArgParser;
using System;

namespace Tag.Networking{

    public class CommandLineStartNetworking : NetworkBehaviour
    {
        [SerializeField] ArgParserSO argParser;
        [SerializeField] UNetTransport uNetTransport;

        private IEnumerator Start() {
            // wait 1 frame for every have run start
            yield return null;
            string network = argParser.GetArg("network");

            switch (network)
            {
                case "client":
                    uNetTransport.ConnectAddress = argParser.GetArg("ip");
                    uNetTransport.ConnectPort = Int32.Parse(argParser.GetArg("port"));
                    Debug.Log($"Connect address: {uNetTransport.ConnectAddress}");
                    Debug.Log($"Connect port   : {uNetTransport.ConnectPort}");
                    NetworkManager.StartClient();
                    break;
                case "server":
                    uNetTransport.ServerListenPort = Int32.Parse(argParser.GetArg("port"));;
                    Debug.Log($"Listening port: {uNetTransport.ServerListenPort}");
                    NetworkManager.StartServer();
                    break;
                default:
                    Debug.LogError($"network arg didn't match client or server, get {network}");
                    break;
            }

        

        }
    }

}
