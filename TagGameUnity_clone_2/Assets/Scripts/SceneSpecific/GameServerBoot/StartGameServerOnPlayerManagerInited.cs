using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ultility.Event;

namespace Tag.ServerBooting{
    public class StartGameServerOnPlayerManagerInited : MonoBehaviour
    {
        [Header("Listening Channel")]
        [SerializeField] VoidEventChannelSO _onPlayerMangerInitedChannel;

        [Header("Broadcasting event")]
        [SerializeField] VoidEventChannelSO _requestStartServerChannel;

        private void OnEnable() {
            _onPlayerMangerInitedChannel.OnEventRaised += _requestStartServerChannel.RaiseEvent;
        }

        private void OnDisable() {
            _onPlayerMangerInitedChannel.OnEventRaised -= _requestStartServerChannel.RaiseEvent;
        }
    }

}

