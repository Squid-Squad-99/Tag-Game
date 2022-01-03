using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour
{

    #region Singleton
    static public GameManager Singleton;
    private void Awake() {
        GameManager.Singleton = this;
    }
    #endregion

    [Header("Game Setting")]
    public int GameDuration;
    
    // state
    public NetworkVariable<int> TimeLeft;

    public event Action StartGameEvent;
    public event Action<GameEndReasonEnum> GameEndEvent;

    public enum GameEndReasonEnum{
        TimeOut,
        CollectEnoughSkullBox,
    }

    private void Start() {
        if(IsServer){
            TimeLeft.Value = GameDuration;
        }
    }

    public void StartGame(){
        Debug.Assert(IsServer);

        Log("Start Game");

        // start timer
        StartCoroutine("StartTimer");

        // event
        InvokeStartGameEvent();
    }

    private IEnumerator StartTimer(){
        TimeLeft.Value = GameDuration;
        while(TimeLeft.Value > 0){
            yield return new WaitForSeconds(1);
            TimeLeft.Value--;
        }
        
        // time up!
        GameEnd(GameEndReasonEnum.TimeOut);
    }

    public void GameEnd(GameEndReasonEnum gameEndReason){
        // stop timer
        StopCoroutine("StartTimer");

        // event
        InvokeGameEndEvent(gameEndReason);
    }

    private void InvokeStartGameEvent(){
        Debug.Assert(IsServer);
        Log("Game Start");
        StartGameEvent?.Invoke();
        StartGameClientRpc();
    }

    private void InvokeGameEndEvent(GameEndReasonEnum gameEndReason){
        Debug.Assert(IsServer);
        Log($"Game End {gameEndReason}");
        GameEndEvent?.Invoke(gameEndReason);
        GameEndClientRpc(gameEndReason);
    }

    [ClientRpc]
    private void StartGameClientRpc(){
        Log("Game Start");
        StartGameEvent?.Invoke();        
    }

    [ClientRpc] 
    private void GameEndClientRpc(GameEndReasonEnum gameEndReason){
        Log($"Game End {gameEndReason}");
        GameEndEvent?.Invoke(gameEndReason);        
    }

    private void Log(string s){
        Debug.Log($"[GameManager] {s}");
    }
}
