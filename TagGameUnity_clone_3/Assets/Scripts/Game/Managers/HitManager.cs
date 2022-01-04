using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

using Tag.Game.Character;
using System.Threading.Tasks;

public class HitManager : NetworkBehaviour
{
    #region Singleton
    static public HitManager Singleton;
    private void Awake() {
        HitManager.Singleton = this;
    }
    #endregion


    [Header("Setting")]
    [SerializeField] float _cameraShakeIntensity;
    [SerializeField] float _cameraShakeTime;
    [Header("state")]
    public int LocalCharacterDealDamage = 0;
    [Header("reference")]
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    public void HitHandle(CharacterObject attackter, CharacterObject victim){
        if(IsServer){
            // deal damage
            int power = attackter.GetComponent<IAttacker>().Power;
            victim.GetComponent<IDamagable>().TakeDamage(power);

            // visual effect
        }

        // attacker is local user
        if(IsClient && attackter == CharacterManager.Singleton.LocalCharacter){
            // cal total damage
            LocalCharacterDealDamage += attackter.GetComponent<IAttacker>().Power;
        }

        if(IsClient && victim == CharacterManager.Singleton.LocalCharacter){
            // shake camera
            ShakeCamera();
        }
    }

    private async void ShakeCamera(){
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = _cameraShakeIntensity;

        await Task.Delay((int)( 1000 * _cameraShakeTime ));
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;

    }
}
