using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Ultility.Event;
using Ultility.Scene;

using Tag.Backend;
using Tag.GameConfig;

/// <summary>
/// main job
/// 1. find match by asking server
///   When Find Match
    /// 1. Set MatchInfoSO to the matchInfo MatchMakginServer given
    /// 2. change to game scene
/// </summary>
public class MatchMakingManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] MatchInfoSO _matchInfo;
    [SerializeField] SceneSO _gameClientBootScene;

    [Header("BroadcastChannel")]
    [SerializeField] VoidEventChannelSO _haveFindMatchChannel;
    [SerializeField] SceneEventChannelSO _requestLoadSceneChannel;

    public static MatchMakingManager Singleton;
    private void Awake() {
        Singleton = this;
    }


    public async void FindMatch(){
        // TODO: changable game mode
        MatchMakingSDK.GameModeEnum gameMode = MatchMakingSDK.GameModeEnum.GrabBall;

        // await server to find match
        MatchMakingSDK.MatchInfo match = await MatchMakingSDK.FindMatch(gameMode, UserHomeUIManager.Singleton.ChoosedCharacter);

        // set match info so
        // TODO currently use editor to set
        _matchInfo.Set(match.success, match.GameServerIP, match.GameServerPort, match.ConnectionAuthId);

        if(match.success == true){
            // delegate find match handler
            _haveFindMatchChannel.RaiseEvent();
            
            // load game scene 
            _requestLoadSceneChannel.RaiseEvent(_gameClientBootScene);
        }
    }
}
