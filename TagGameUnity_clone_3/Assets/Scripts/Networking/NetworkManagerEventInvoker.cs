using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

using Ultility.Event;

namespace Tag.Networking{

    /// <summary>
    /// This Class will invoke SO event of type network connection
    /// </summary>
    public class NetworkManagerEventInvoker : MonoBehaviour
    {
        [Header("BroadCast Channels")]
        [SerializeField] UlongEventChannelSO OnClientConnectedCallbackChannel;
        [SerializeField] UlongEventChannelSO OnClientDisconnectCallbackChannel;
        [SerializeField] VoidEventChannelSO OnServerStartedChannel;

        private void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }

        private void OnDisable() {
            if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;       
            }
        }

        private void OnServerStarted()
        {
            OnServerStartedChannel.RaiseEvent();
        }

        private void OnClientDisconnectCallback(ulong obj)
        {
            OnClientDisconnectCallbackChannel.RaiseEvent(obj);
        }

        private void OnClientConnectedCallback(ulong obj)
        {
            OnClientConnectedCallbackChannel.RaiseEvent(obj);
        }
    }

}

