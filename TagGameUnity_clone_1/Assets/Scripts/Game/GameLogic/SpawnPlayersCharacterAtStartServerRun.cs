using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

using Ultility.Event;

using Tag.Game.Managers;
using Tag.Game.Character;
using UnityEngine.SceneManagement;

namespace Tag.Game.GameLogic{

    /// <summary>
    /// (only run in server)
    /// main job
    /// 1. spawn player's character coresponding to player's config for each player
    /// </summary>
    public class SpawnPlayersCharacterAtStartServerRun : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject _characterPrefab;

        [Header("Listening Channel")]
        [SerializeField] StringEventChannelSO _onAllClientLoadSceneChannel;

        [Header("Reference")]
        [SerializeField] Transform _defaultSpawnPoints; 



        void Awake()
        {   
            //only run at server
            if(NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer == false) return;

            // Spawn player's character on all client load game scene
            UnityAction<string> SpawnPlayersCharacter = null;

            SpawnPlayersCharacter = async (sceneGame) => {
                // check is game scene
                if(sceneGame != SceneManager.GetActiveScene().name) return;

                // wait a sec to let all client set active scene
                await Task.Delay(500);

                var SpawnPointsEnumerator = _defaultSpawnPoints.GetEnumerator();

                foreach (ulong userId in PlayerManager.Singleton.Players)
                {
                    // get this user player config
                    PlayerConfig playerConfig = PlayerManager.Singleton.UserIdToPlayerConfigDict[userId];

                    //TODO
                    // use playerConfig to what character to spawn and set some attribute of the character 

                    // get spawn point
                    SpawnPointsEnumerator.MoveNext();
                    Transform spawnPoint = (Transform)SpawnPointsEnumerator.Current;

                    // spawn character
                    GameObject logic = Instantiate(_characterPrefab, spawnPoint.position, _characterPrefab.transform.rotation);

                    // give to this user
                    logic.GetComponent<CharacterObject>().GiveToUser(userId);
                }            

                _onAllClientLoadSceneChannel.OnEventRaised -= SpawnPlayersCharacter;
            };

            _onAllClientLoadSceneChannel.OnEventRaised += SpawnPlayersCharacter;

        }

    }
}

