using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

using Ultility.Event;
using Tag.Networking;

namespace Tag.Game.Character{

    /// <summary>
    ///  disable or enable character logic to run,
    /// if is a client and not own it, dont run
    /// server always run
    /// if is own by local user, run for client prediction
    /// </summary>
    public class CharacterLogicSwitch : NetworkBehaviour
    {
        [SerializeField] List<MonoBehaviour> LogicComponents;

        [Header("Listening Channel")]
        [SerializeField] GameObjectEventChannelSO _characterSpawnEvent;
        public bool EnableLogic = true;

        private void OnEnable() {
            UnityAction<GameObject> LogicSwitch = null;
            LogicSwitch = (character) => {
                //is this character
                if(character == gameObject){
                    // is client and not own this character
                    if(IsClient && GetComponent<CharacterObject>().OwnedByLocalUser == false){
                        TurnOffLogic();
                        if(GetComponent<SyncTransform>()) GetComponent<SyncTransform>().Mode = SyncTransform.ModeEnum.Sync;
                    }
                    else{
                        if(GetComponent<SyncTransform>()) GetComponent<SyncTransform>().Mode = SyncTransform.ModeEnum.PredictionCorrection;
                    }
                    // onHook event
                    _characterSpawnEvent.OnEventRaised -= LogicSwitch;
                }
            };

            // hook character spawn event
            _characterSpawnEvent.OnEventRaised += LogicSwitch;
        }

        private void TurnOffLogic(){
            Debug.Log($"IS owen {GetComponent<CharacterObject>().OwnedByLocalUser}");
            if(GetComponent<CharacterObject>().OwnedByLocalUser == true) return;

            // turn of component
            foreach(MonoBehaviour mono in LogicComponents){
                mono.enabled = false;
            }
            // if have rigibody turn to kinematic
            if(TryGetComponent<Rigidbody>(out Rigidbody rigibody)) rigibody.isKinematic = true;
            // if(TryGetComponent<Collider>(out Collider collider)) collider.enabled = false;

            EnableLogic = false;
        }

    }
}
