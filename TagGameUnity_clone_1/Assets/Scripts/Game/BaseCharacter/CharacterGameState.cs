using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public interface IDamagable
{
    public void TakeDamage(int power);
    public event Action DieEvent;
}

public class CharacterGameState : NetworkBehaviour, IDamagable
{
    [Header("Setting")]
    public int InitialHealth;

    // state
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);

    public bool IsDied;

    public event Action DieEvent;

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

    }
}
