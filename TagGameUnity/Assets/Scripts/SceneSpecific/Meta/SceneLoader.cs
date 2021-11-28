using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Ultility.Event;
using Ultility.Scene;

public class SceneLoader : MonoBehaviour
{
    [Header("Listening Channel")]
    [SerializeField] LoadSceneEventChannelSO _requestLoadSceneChannel;

    private void OnEnable() {
        _requestLoadSceneChannel.OnEventRaised += OnRequestLoadScene;
    }

    private void OnDisable() {
        _requestLoadSceneChannel.OnEventRaised -= OnRequestLoadScene;
    }

    private void OnRequestLoadScene(SceneSO sceneSO, bool withLoadingScene)
    {
        // TODO
        // handle wit loading scene

        // load scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneSO.SceneName, LoadSceneMode.Additive);

        // set loaded scene to active while complete
        asyncOperation.completed += (obj) => {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneSO.SceneName));
            Debug.Log($"Set active scene to {sceneSO.SceneName}");
        };
    }
}
