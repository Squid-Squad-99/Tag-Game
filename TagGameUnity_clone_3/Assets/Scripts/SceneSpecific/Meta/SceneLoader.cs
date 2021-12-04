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
        Debug.Log($"Scene {newScene.name} loaded");

        // unload previous active scene
        string oldSceneName = SceneManager.GetActiveScene().name;

        if(oldSceneName.Substring(0, 4) != "Meta"){
            SceneManager.UnloadSceneAsync(oldSceneName);
        }

        // set new scene active
        SceneManager.SetActiveScene(newScene);
    }

}
