using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;

using Ultility.Event;

namespace Tag.Networking{

    /// <summary>
    /// This Class will invoke SO event of type network connection
    /// </summary>
    public class NetworkManagerEventInvoker : MonoBehaviour
    {
        [Header("Network General Channel")]
        [SerializeField] UlongEventChannelSO OnClientConnectedCallbackChannel;
        [SerializeField] UlongEventChannelSO OnClientDisconnectCallbackChannel;
        [SerializeField] VoidEventChannelSO OnServerStartedChannel;
        [SerializeField] VoidEventChannelSO OnNetworkManagerInstantiate;

        [Header("Scene Stuff Channel")]
        [SerializeField] StringEventChannelSO OnBeforeNetworkLoadSceneChannel;
        [SerializeField] StringEventChannelSO OnLoadCompleteChannel;

        private void Start() {
            OnNetworkManagerInstantiate.RaiseEvent();
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

        private void HookSceneStuffChannel(){
            NetworkManager.Singleton.SceneManager.VerifySceneBeforeLoading += OnVerifySceneBeforeLoading;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
            NetworkManager.Singleton.SceneManager.OnLoad += OnLoad;
        }

        private void OnLoad(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
        {
            // Debug.Log($"{sceneName} on load");
        }

        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if(NetworkManager.Singleton.IsServer && NetworkManager.Singleton.LocalClientId != clientId) return;
            OnLoadCompleteChannel.RaiseEvent(sceneName);
        }

        private bool OnVerifySceneBeforeLoading(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode){
            // if is not game scene, we dont load it
            // TODO: game scene can not only be GabBallGameScene 
            if(sceneName != "GrabBallGameScene" && sceneName != "NetworkSampleScene"){
                return false;
            } 

            OnBeforeNetworkLoadSceneChannel.RaiseEvent(sceneName);
            return true;
        }

        private void OnServerStarted()
        {
            HookSceneStuffChannel();
            OnServerStartedChannel.RaiseEvent();
        }

        private void OnClientDisconnectCallback(ulong obj)
        {
            OnClientDisconnectCallbackChannel.RaiseEvent(obj);
        }

        private void OnClientConnectedCallback(ulong obj)
        {
            if(NetworkManager.Singleton.IsClient){
                Debug.Log("client connect");
                HookSceneStuffChannel();
            }
            OnClientConnectedCallbackChannel.RaiseEvent(obj);
        }
    }

}

