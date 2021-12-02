using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

using Ultility.Event;
using Ultility.Scene;

public class SceneLoader : MonoBehaviour
{
    [Header("Listening Channel")]
    [SerializeField] SceneEventChannelSO _requestLoadSceneChannel;
    [SerializeField] SceneEventChannelSO _requestNetworkLoadSceneChannel;

    private void OnEnable() {
        _requestLoadSceneChannel.OnEventRaised += OnRequestLoadScene;
        _requestNetworkLoadSceneChannel.OnEventRaised += OnRequestNetworkLoadScene;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        _requestLoadSceneChannel.OnEventRaised -= OnRequestLoadScene;
        _requestNetworkLoadSceneChannel.OnEventRaised -= OnRequestNetworkLoadScene;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnRequestLoadScene(SceneSO sceneSO)
    {
        // load new scene
        SceneManager.LoadSceneAsync(sceneSO.SceneName, LoadSceneMode.Additive);


        // // unload previous active scene
        // AsyncOperation oldAO = null;
        // if(SceneManager.GetActiveScene().name != "MetaScene"){
        //     oldAO = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        // }

        // Action loadNewScene = () => {
        //     AsyncOperation newAO = SceneManager.LoadSceneAsync(sceneSO.SceneName, LoadSceneMode.Additive);

        //     // set loaded scene to active while complete
        //     newAO.completed += (obj) => {
        //         SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneSO.SceneName));
        //         Debug.Log($"Set active scene to {sceneSO.SceneName}");
        //     };
        // };

        // if(oldAO != null){
        //     // load new scene when done unload
        //     oldAO.completed += (ao) => {
        //         loadNewScene.Invoke();
        //     };
        // }
        // else{
        //     loadNewScene.Invoke();
        // }

    }

    private void OnRequestNetworkLoadScene(SceneSO sceneSO)
    {
        if(NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer != true){
            Debug.LogError("[SceneLoader] cant' network load scene if networkManager is null or this is not server");
        }


        // network load new scene
        NetworkManager.Singleton.SceneManager.LoadScene(sceneSO.SceneName, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
    {
        // unload previous active scene
        string oldSceneName = SceneManager.GetActiveScene().name;

        if(oldSceneName != "MetaScene"){
            SceneManager.UnloadSceneAsync(oldSceneName);
        }

        // set new scene active
        SceneManager.SetActiveScene(newScene);
    }

    // private void OnNetworkLoadComplete(string sceneName)
    // {
    //     // unload previous active scene
    //     AsyncOperation oldAO = null;
    //     if(SceneManager.GetActiveScene().name != "MetaScene"){
    //         oldAO = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    //     }

    //     SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    //     Debug.Log($"Set active scene to {sceneName}");
    // }

}
