using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Ultility.Event;

public class ButtonRaiseEvent : MonoBehaviour
{
    [Header("BroadcastChannel")]
    [SerializeField] VoidEventChannelSO _onButtonClickChannel;

    private Button _button;


    private void Awake() {
        _button = GetComponent<Button>();
        if(_button == null){
            Debug.LogWarning("[ButtonRaiseEvent] can't find button component");
        }
    }

    private void OnEnable() {
        _button.onClick.AddListener(_onButtonClickChannel.RaiseEvent);
    }

    private void OnDisable() {
        _button.onClick.RemoveListener(_onButtonClickChannel.RaiseEvent);
    }

}
