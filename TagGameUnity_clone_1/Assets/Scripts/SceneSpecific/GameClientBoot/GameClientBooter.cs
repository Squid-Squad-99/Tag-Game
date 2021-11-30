using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

using Tag.GameConfig;

/// <summary>
/// Boot up the game scene
/// main job
/// 1. connect to game server by matchInfoSO
/// </summary>
public class GameClientBooter : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] MatchInfoSO matchInfo;
    [SerializeField] UNetTransport uNetTransport;

    private void Start() {
        // config transport by match info
        uNetTransport.ConnectAddress = matchInfo.GameServerIP;
        uNetTransport.ConnectPort = Int32.Parse(matchInfo.GameServerPort);


        // connect to server (start network client)
        NetworkManager.Singleton.StartClient();
        
        // set scene sync mode for client
        Debug.Log(NetworkManager.Singleton.SceneManager);
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);

    }
}
