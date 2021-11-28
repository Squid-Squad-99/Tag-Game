using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ultility.Scene;
using Ultility.Event;

public class Booter : MonoBehaviour
{
    [Space(10)]
    [Tooltip("The scene that will be first load")]
    [SerializeField] SceneSO BootingSceneSO;

    [Header("BroadcastChannel")]
    [SerializeField] LoadSceneEventChannelSO _requestLoadSceneChannel;

    private void Start() {
        _requestLoadSceneChannel.RaiseEvent(BootingSceneSO, false);
    }
}
