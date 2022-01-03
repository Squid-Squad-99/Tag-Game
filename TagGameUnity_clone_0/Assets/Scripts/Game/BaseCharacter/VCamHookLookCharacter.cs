using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

using Ultility.Event;

using Tag.Game.Managers;

namespace Tag.Game.Character{

    /// <summary>
    /// main job
    /// 1. hook this gameobject vcam to player look target only on client
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class VCamHookLookCharacter : MonoBehaviour
    {
        [Header("Listening Channel")]
        [SerializeField] GameObjectEventChannelSO _characterGiveToUserChannel;

        private void OnEnable() {
            _characterGiveToUserChannel.OnEventRaised += HookToLocalUserCharacter;
        }

        private void OnDisable() {
            _characterGiveToUserChannel.OnEventRaised -= HookToLocalUserCharacter;
        }

        private void HookToLocalUserCharacter(GameObject character)
        {
            if(NetworkManager.Singleton.IsServer) return;
            // check this character visual belong to local user
            CharacterObject characterObject = character.GetComponent<CharacterObject>(); 
            ulong localUserId = PlayerManager.Singleton.LocalUserId;
            if(characterObject.OwnerUserId.Value == localUserId){
                GetComponent<CinemachineVirtualCamera>().Follow = characterObject.PlayerVCamTarget;
            }
        }
    }

}
