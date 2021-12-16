using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
        public bool EnableLogic = true;

        public override void OnNetworkSpawn()
        {
            // turn of logic if is client and not own this
            if(IsClient){
                Debug.Log($"IS owen {GetComponent<CharacterObject>().OwnedByLocalUser}");
                if(GetComponent<CharacterObject>().OwnedByLocalUser == true) return;

                // turn of component
                foreach(MonoBehaviour mono in LogicComponents){
                    mono.enabled = false;
                }
                // if have rigibody turn to kinematic
                if(TryGetComponent<Rigidbody>(out Rigidbody rigibody)) rigibody.isKinematic = true;
                if(TryGetComponent<Collider>(out Collider collider)) collider.enabled = false;

                EnableLogic = false;
            }
        }


    }
}
