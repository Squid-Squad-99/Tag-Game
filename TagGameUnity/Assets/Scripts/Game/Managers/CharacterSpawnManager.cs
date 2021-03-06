using System.Collections;
using System.Collections.Generic;
using Tag.Game.Character;
using Tag.Game.Managers;
using Ultility.Event;
using UnityEngine;

public class CharacterSpawnManager : MonoBehaviour
{
        [Header("Character")]
        [SerializeField] GameObject _HumancharacterPrefab;
        [SerializeField] GameObject _GhostcharacterPrefab;

        [Header("Reference")]
        [SerializeField] Transform _humanPositions; 
        [SerializeField] Transform _ghostPositions; 

    public void SpawnCharacters(){

        var humanPosIter = _humanPositions.GetEnumerator();
        var ghostPosIter = _ghostPositions.GetEnumerator();

        foreach (ulong userId in PlayerManager.Singleton.Players)
        {
            // get this user player config
            PlayerConfig playerConfig = PlayerManager.Singleton.UserIdToPlayerConfigDict[userId];

            //TODO
            // use playerConfig to what character to spawn and set some attribute of the character 


            // spawn character
            GameObject logic;
            if(playerConfig.CharacterType == Tag.Backend.MatchMakingSDK.CharacterTypeEnum.Ghost){
                ghostPosIter.MoveNext();
                Transform spawnPoint = (Transform)ghostPosIter.Current;
                logic = Instantiate(_GhostcharacterPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else{
                humanPosIter.MoveNext();
                Transform spawnPoint = (Transform)humanPosIter.Current;
                logic = Instantiate(_HumancharacterPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            // give to this user
            logic.GetComponent<CharacterObject>().GiveToUser(userId, playerConfig);
        }            
}
}
