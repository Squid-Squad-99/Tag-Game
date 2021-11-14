using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ultility.Event;
using System;
using TMPro;

namespace Tag.Debugging{

    public class DebugDisplayer : MonoBehaviour
    {

        [Header("Reference")]
        [SerializeField] TextMeshProUGUI _displayText;

        [Header("Listening Channel")]
        [SerializeField] StringEventChannelSO _requestDebugDisplayChannel;

        private List<string> _displayTextList = new List<string>();
        private List<string> _displayChannelNameList = new List<string>();

        private void OnEnable() {
            _requestDebugDisplayChannel.OnEventRaised += HandleDisplayRequest;
            _displayText.text = "";
        }

        private void OnDisable() {
            _requestDebugDisplayChannel.OnEventRaised -= HandleDisplayRequest;
            _displayText.text = "";
        }

        private void HandleDisplayRequest(string arg)
        {
            string channelName = arg.Substring(0, arg.IndexOf(':'));

            //have this channel
            bool haveThis = false;
            for(int i = 0; i < _displayChannelNameList.Count; i++){
                if(_displayChannelNameList[i] == channelName){
                    _displayTextList[i] = arg;
                    haveThis = true;
                    break;
                }
            }

            // when not have this channel
            if(haveThis == false){
                _displayChannelNameList.Add(channelName);
                _displayTextList.Add(arg);
            }

            //display
            string displayText = "";
            foreach(string s in _displayTextList){
                displayText += $"{s}\n";
            }

            _displayText.text = displayText;
        }
    }

}
