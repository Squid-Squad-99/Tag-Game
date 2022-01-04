using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using System;

public class CutSceneManager : NetworkBehaviour
{
    #region Singleton
    static public CutSceneManager Singleton = null;

    private void Awake() {
        CutSceneManager.Singleton = this;
    }
         
    #endregion

    public enum CutSceneId{
        BeforeStartGame,
    } 

    public event Action<CutSceneId> CuteSceneStartEvent;
    public event Action<CutSceneId> CuteSceneEndEvent;

    public async void PlayCutSceneAsync(CutSceneId cutSceneId){
        if(IsServer) PlayCutSceneClientRpc(cutSceneId, NetworkManager.Singleton.ServerTime.Time);

        CuteSceneStartEvent?.Invoke(cutSceneId);

        switch (cutSceneId)
        {
            case CutSceneId.BeforeStartGame:
                Debug.Log("[CutSceneManger] Play before start game cut scene");
                GameUIManager.Singleton.PlayStartGameCutScene();
                await Task.Delay(3700);
                Debug.Log("[CutSceneManger] Before start game cut scene end");
                break;
            default:
                break;
        }

        CuteSceneEndEvent?.Invoke(cutSceneId);

    }

    [ClientRpc]
    private async void PlayCutSceneClientRpc(CutSceneId cutSceneId, double serverTime){
        double waitTime = serverTime - NetworkManager.Singleton.ServerTime.Time;
        Debug.Log("Client Play cut scene");
        if(waitTime > 0){
            await Task.Delay( (int)( (waitTime) *1000) );
        }

        PlayCutSceneAsync(cutSceneId);
    }
}
