using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

using Ultility.ArgParser;
using Ultility.Event;
using Ultility.Scene;

using Tag.Backend;
using UnityEditor;

namespace Tag.ClientBooting{
    /// <summary>
    /// command arg give port, gameMode
    /// main job
    /// 1. start server at port
    /// 2. get game scene's SceneSO by arg
    /// 2. network load game scene 
    /// </summary>
    public class GameServerBooter : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] ArgParserSO _serverArgParser;
        [SerializeField] UNetTransport _uNetTransport;

        [Header("Game Scene Canidate")]
        [SerializeField] SceneSO _grabBallScene;

        [Header("Broadcast Channel")]
        [SerializeField] SceneEventChannelSO _requestNetworkLoadSceneChannel;

        private void Start() {

            // set transport by arg
            int port =Int32.Parse(_serverArgParser.GetArg("port"));
            _uNetTransport.ServerListenPort = port;

            // start server
            NetworkManager.Singleton.StartServer();
        }

        public void LoadGameScene(){
                // network load game scene 
                _requestNetworkLoadSceneChannel.RaiseEvent(GetSceneByArg());
        }

        private SceneSO GetSceneByArg(){
            BackendSDK.GameModeEnum gameMode = (BackendSDK.GameModeEnum)Int32.Parse(_serverArgParser.GetArg("gameMode"));

            switch (gameMode)
            {
                case BackendSDK.GameModeEnum.GrabBall:
                    return _grabBallScene;
                default:
                    Debug.LogError("Can't find game scene to load by arg, default to GrabBallScene");
                    return _grabBallScene;
            }
            
        }
    }



}

