using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;
using Tag.Game.Character;

public interface IDamagable
{
    public void TakeDamage(int power);
    public event Action DieEvent;
}

public class CharacterGameState : NetworkBehaviour, IDamagable
{
    [Header("Setting")]
    public int InitialHealth;
    public float GhostFrozenTime;

    [Header("State")]

    // state
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);

    public bool IsDied;
    public NetworkVariable<float> GhostFrozenTimeDelta;

    public event Action DieEvent;
    public event Action RebornEvent;

    public void TakeDamage(int power)
    {
        if(IsServer){
            Health.Value -= power;

        }
    }

    private void Start() {
        // set health when game start
        if(GameManager.Singleton != null){
            GameManager.Singleton.StartGameEvent += () =>{
                if(IsServer){
                    Health.Value = InitialHealth;
                }

                // invoke die event when health 0
                Health.OnValueChanged += (p,n) => {
                    if(IsDied == false && n <= 0){
                        IsDied = true; 
                        DieEvent?.Invoke();
                    }
                };
            };
        }

        // reborn ghost after died
        DieEvent += () => {
            if(IsServer && GetComponent<CharacterObject>().CharacterType == CharacterObject.CharacterTypeEnum.Ghost){
                StartCoroutine(CountDownReborn());
            }
        };

    }

    private IEnumerator CountDownReborn(){
        GhostFrozenTimeDelta.Value = GhostFrozenTime;
        while(GhostFrozenTimeDelta.Value > 0) {
            GhostFrozenTimeDelta.Value -= Time.deltaTime;
            yield return null;
        }

        IsDied = false;
        RebornEvent?.Invoke();
        InvokeRebornClientRpc();
    }

    [ClientRpc]
    private void InvokeRebornClientRpc(){
        IsDied = false;
        RebornEvent?.Invoke();
    }
}
