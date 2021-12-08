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
        [SerializeField] StringEventChannelSO OnAllClientCompleteLoadSceneChannel;

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
            NetworkManager.Singleton.SceneManager.DisableValidationWarnings(true);
            NetworkManager.Singleton.SceneManager.VerifySceneBeforeLoading += OnVerifySceneBeforeLoading;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }

        private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if(NetworkManager.Singleton.IsServer == false) return;
            OnAllClientCompleteLoadSceneChannel.RaiseEvent(sceneName);
        }

        private bool OnVerifySceneBeforeLoading(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode){
            // Dont load metascene, client will have already load...
            if(sceneName == "MetaScene"){
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
                HookSceneStuffChannel();
            }
            OnClientConnectedCallbackChannel.RaiseEvent(obj);
        }
    }

}

