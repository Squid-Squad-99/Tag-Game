using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Ultility.Scene;
using Ultility.Event;
using Tag.UserStuff;
using Tag.Backend;

namespace Tag.ClientBooting{

    /// <summary>
    /// handle logic of booting client app
    /// The goal of this class is to determine which scene to load and load it
    /// </summary>
    public class ClientBooter : MonoBehaviour
    {

        [Header("Next Scene Canditate")]
        [SerializeField] SceneSO WelcomeScene;
        [SerializeField] SceneSO UserHomeScene;
        [SerializeField] SceneSO LoginScene;

        [Header("Reference")]
        [SerializeField] CachedUserCredentialsSO _cachedUserCredentials;

        [Header("BroadcastChannel")]
        [SerializeField] SceneEventChannelSO _requestLoadSceneChannel;

        private async void Start() {
            // decide which scene load next
            SceneSO nextScene = await NextSceneDecideLogic();

            // load next scene
            _requestLoadSceneChannel.RaiseEvent(nextScene); 
        }

        private async Task<SceneSO> NextSceneDecideLogic(){
            SceneSO nextScene = null;
            if(_cachedUserCredentials.HaveCached){
                if(await WebSDK.Login(_cachedUserCredentials.UserName, _cachedUserCredentials.Password)){
                    // have cache credential and is valid
                    nextScene = UserHomeScene;
                }
                else{
                    // have cache credential but invalid
                    nextScene = LoginScene;
                }
            }
            else{
                // dont have cache credential
                nextScene = WelcomeScene;
            }

            return nextScene;
        }

    }

}
