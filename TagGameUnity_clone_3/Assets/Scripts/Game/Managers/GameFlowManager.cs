using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Unity.Netcode;

using Ultility.Event;
using Ultility.Scene;
using System;
using System.Threading.Tasks;

using Tag.Game.Managers;
using UnityEngine.UI;

public class GameFlowManager : NetworkBehaviour
{
    #region Singleton
    static public GameFlowManager Singleton;
    private void Awake() {
        GameFlowManager.Singleton = this;
    }
    #endregion

    public enum StateId{
        SetupGame,
        BeforeStartGame, // countdown, startgame cut scene
        InGame,
        GameOver,
    }

    public event Action BeforeStarGameEvent;
    
    [Header("Listening Channel")]
    [SerializeField] StringEventChannelSO _AllClientLoadSceneEvent;

    // state
    [SerializeField] StateId _gameFlowState;

    [Header("Broad Channel")]
    [SerializeField] SceneEventChannelSO _requestLoadSceneEvent;

    [Header("Reference")]
    [SerializeField] Button _continueButton;
    [SerializeField] SceneSO _userHomeScene;

    private void Start() {
            //only run at server
            if(NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer == false) return;

            // Spawn player's character on all client load game scene
            UnityAction<string> SpawnPlayersCharacter = null;

            SpawnPlayersCharacter = async (sceneGame) => {
                // check is game scene
                if(sceneGame != SceneManager.GetActiveScene().name) return;

                // wait a sec to let all client set active scene
                await Task.Delay(500);

                // init game state 
                SetGameState(StateId.SetupGame);

                _AllClientLoadSceneEvent.OnEventRaised -= SpawnPlayersCharacter;
            };

            _AllClientLoadSceneEvent.OnEventRaised += SpawnPlayersCharacter;

            // hook button
            _continueButton.onClick.AddListener(OnContinueButtonClick);
    }

    private void SetGameState(StateId gameState){
        _gameFlowState = gameState;
        switch (gameState)
        {
            case StateId.SetupGame:
                OnSetUpGame();
                break;
            case StateId.BeforeStartGame:
                OnBeforeStartGame();
                InvokeBeforeStartGameEvent();
                break;
            case StateId.InGame:
                OnInGame();
                break;
            default:
                break;
        }
        
    }

    private void OnGameOver(GameManager.GameEndReasonEnum reason)
    {
        Debug.Assert(IsServer);
        Log("To Game over state");

        // show game result
        GameUIManager.Singleton.ShowGameResult(reason);

    }

    public void OnContinueButtonClick(){
        // clean up managers
        

        // change to home scene
        _requestLoadSceneEvent.RaiseEvent(_userHomeScene);

    }

    private void GameOverCleanUp()
    {
        Debug.Assert(IsClient);
        
        Destroy(NetworkManager.Singleton.gameObject);
        Destroy(PlayerManager.Singleton.gameObject);

        Log("Clean up Managers");
    }


    /// <summary>
    /// set up (all character ready)
    /// </summary>
    private void OnSetUpGame()
    {
        CharacterManager.Singleton.AllCharacterReadiedEvent += () => SetGameState(StateId.BeforeStartGame);

        // spawn characters 
        GetComponent<CharacterSpawnManager>().SpawnCharacters();

    }

    private void OnBeforeStartGame()
    {
        Log("Before star game State");

        // play start game cut scene
        CutSceneManager.Singleton.PlayCutSceneAsync(CutSceneManager.CutSceneId.BeforeStartGame);
        
        // Set to play state on cut scene over
        Action<CutSceneManager.CutSceneId> OnCutSceneEnd = null;
        OnCutSceneEnd = (cutSceneId) => {
            if(cutSceneId != CutSceneManager.CutSceneId.BeforeStartGame) return;
            SetGameState(StateId.InGame);
            CutSceneManager.Singleton.CuteSceneEndEvent -= OnCutSceneEnd;
        };
        CutSceneManager.Singleton.CuteSceneEndEvent += OnCutSceneEnd;
    }

    private void OnInGame()
    {
        // start game
        GameManager.Singleton.StartGame();

        GameManager.Singleton.GameEndEvent += (reason) => {SetGameState(StateId.GameOver); OnGameOver(reason);};
    }

    private void InvokeBeforeStartGameEvent(){
        BeforeStarGameEvent?.Invoke();
        BeforeStartGameClientRpc();

    }

    [ClientRpc]
    private void BeforeStartGameClientRpc(){
        BeforeStarGameEvent?.Invoke();
    }

    private void Log(string s){
        Debug.Log($"[GameFlowManager] {s}");
    }
}
