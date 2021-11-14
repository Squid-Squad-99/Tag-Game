using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

using Ultility.Event;

namespace Tag.Debugging{

    public class ConnectionCheck : NetworkBehaviour
    {
        public enum LogLevelEnum{
            None,
            Log,
        }
        [Tooltip("Set None to not display log message\n Set Log to display log message")]
        public LogLevelEnum LogLevel = LogLevelEnum.Log;
        [Header("Listening Channels")]
        [SerializeField] UlongEventChannelSO OnClientConnectedCallbackChannel;
        [SerializeField] UlongEventChannelSO OnClientDisconnectCallbackChannel;
        [SerializeField] VoidEventChannelSO OnServerStartedChannel;

        private void Log(string s){
            if(LogLevel == LogLevelEnum.Log)
            {
                Debug.Log(s);
            }
        }

        private void OnEnable() {
            OnClientConnectedCallbackChannel.OnEventRaised += OnClientConnectedCallback;
            OnClientDisconnectCallbackChannel.OnEventRaised += OnClientDisconnectCallback;
            OnServerStartedChannel.OnEventRaised += OnServerStarted;
        }

        private void OnDisable() {
            OnClientConnectedCallbackChannel.OnEventRaised -= OnClientConnectedCallback;
            OnClientDisconnectCallbackChannel.OnEventRaised -= OnClientDisconnectCallback;
            OnServerStartedChannel.OnEventRaised -= OnServerStarted;
        }

        private void OnClientDisconnectCallback(ulong obj)
        {
            if(IsServer == true){
                Log($"Server disconnect client {obj}");
            }
            if(IsClient == true){
                Log($"Local client {obj} disconnect to server");
            }
        }

        private void OnClientConnectedCallback(ulong obj)
        {
            if(IsServer == true){
                Log($"Server connect client {obj}");
            }
            if(IsClient == true && NetworkManager.Singleton.LocalClientId == obj){
                Log($"Local client {obj} connect to server");
            }
        }

        private void OnServerStarted()
        {
            Log("Server started");
        }
    }

}
