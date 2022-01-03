using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Tag.Game.Character;

public class HitManager : NetworkBehaviour
{
    #region Singleton
    static public HitManager Singleton;
    private void Awake() {
        HitManager.Singleton = this;
    }
    #endregion

    public void HitHandle(CharacterObject attackter, CharacterObject victim){
        if(IsServer){
            // deal damage
            int power = attackter.GetComponent<IAttacker>().Power;
            victim.GetComponent<IDamagable>().TakeDamage(power);

            // visual effect
        }
    }
}
