using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ultility.Event;
using Ultility.Scene;
using Ultility.ArgParser;

using Tag.Backend;

namespace Tag.ServerBooting{

    /// <summary>
    /// main job
    /// 1. load game scene when be request
    /// </summary>
    public class LoadGameSceneOnRequest : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] ArgParserSO _serverArgParser;
        
        [Header("Game Scene Canidate")]
        [SerializeField] SceneSO _grabBallScene;

        [Header("Broadcast Channel")]
        [SerializeField] SceneEventChannelSO _requestNetworkLoadSceneChannel;
        
        [Header("Listening Channel")]
        [SerializeField] VoidEventChannelSO _requestLoadGameScene;
        
        private void OnEnable() {
            _requestLoadGameScene.OnEventRaised += OnRequestLoadGameScene;
        }

        private void OnDisable() {
            _requestLoadGameScene.OnEventRaised -= OnRequestLoadGameScene;
        }

        private void OnRequestLoadGameScene()
        {
            LoadGameScene();
        }
        public void LoadGameScene(){
            // network load game scene 
            _requestNetworkLoadSceneChannel.RaiseEvent(GetSceneByArg());
        }

        private SceneSO GetSceneByArg(){
            MatchMakingSDK.GameModeEnum gameMode = (MatchMakingSDK.GameModeEnum)Int32.Parse(_serverArgParser.GetArg("gameMode"));

            switch (gameMode)
            {
                case MatchMakingSDK.GameModeEnum.GrabBall:
                    return _grabBallScene;
                default:
                    Debug.LogError("Can't find game scene to load by arg, default to GrabBallScene");
                    return _grabBallScene;
            }
            
        }
    }

}