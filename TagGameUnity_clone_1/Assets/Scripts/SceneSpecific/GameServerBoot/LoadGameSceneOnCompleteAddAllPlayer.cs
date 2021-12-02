using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ultility.Event;

public class LoadGameSceneOnCompleteAddAllPlayer : MonoBehaviour
{
    [Header("Listening Channel")]
    [SerializeField] VoidEventChannelSO _onCompleteAddAllPlayer;

    [Header("Broadcasting event")]
    [SerializeField] VoidEventChannelSO _requestLoadGameScene;

    private void OnEnable() {
        _onCompleteAddAllPlayer.OnEventRaised += _requestLoadGameScene.RaiseEvent;
    }

    private void OnDisable() {
        _onCompleteAddAllPlayer.OnEventRaised -= _requestLoadGameScene.RaiseEvent;
    }
}
