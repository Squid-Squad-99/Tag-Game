using System.Collections;
using System.Collections.Generic;
using RigiArcher.MeshSocket;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

using Tag.Game.Character;

public interface IAttacker
{
    public int Power{get;}
}

public class SwordManager : NetworkBehaviour, IAttacker
{
    [SerializeField]
    private int _power;
    public int Power => _power;
    [Header("Reference")]
    [SerializeField] GameObject _swordGameObject;

    public enum NetActionId{
        Equip,
        AnimEquip,
        UnEquip,
        AnimUnEquip,
        AnimDoneUse,
        Use,
    }

    // data
    private NetworkVariable<float> cooldownDelta = new NetworkVariable<float>(0f);

    // reference
    private Collider _swordCollider;
    private MeshSocketManager _meshSocketManager;
    private CharacterObject _characterObject;
    private Animator _animator;
    private int _animParamEquipSword;
    private int _animParamUseSword;


    private void Start() {
        // reference
        _meshSocketManager = GetComponent<MeshSocketManager>();
        _animator = GetComponent<Animator>();
        _swordCollider = _swordGameObject.GetComponent<Collider>();
        _characterObject = GetComponent<CharacterObject>();

        // anim param
        _animParamEquipSword = Animator.StringToHash("EquipSword");
        _animParamUseSword = Animator.StringToHash("UseSword");

        _meshSocketManager.Attach(_swordGameObject.transform, MeshSocketManager.SocketIdEnum.Back);
    }

    private void Update() {
        if(!IsServer) return;

        if(cooldownDelta.Value > 0) {
            cooldownDelta.Value -= Time.deltaTime;
        }
    }

    public void EquipSword(){
        // animate equip
        _animator.SetBool(_animParamEquipSword, true);
        if(IsServer) NetActionClientRpc(NetActionId.Equip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void OnAnimationEquipSword(){
        _meshSocketManager.Attach(_swordGameObject.transform, MeshSocketManager.SocketIdEnum.RightHand);
        if(IsServer) NetActionClientRpc(NetActionId.AnimEquip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void OnAnimationUnEquipSword(){
        _meshSocketManager.Attach(_swordGameObject.transform, MeshSocketManager.SocketIdEnum.Back);
        if(IsServer) NetActionClientRpc(NetActionId.AnimUnEquip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void OnAnimationStartUseSword(){
        _swordCollider.enabled = true;
    }

    public void OnAnimationDoneUseSword(){
        _animator.SetBool(_animParamUseSword, false);
        _swordCollider.enabled = false;
        if(IsServer) NetActionClientRpc(NetActionId.AnimDoneUse, NetworkManager.Singleton.ServerTime.Time);
    }

    public void UnEquipSword(){
        // animate Unequip
        _animator.SetBool(_animParamEquipSword, false);
        if(IsServer) NetActionClientRpc(NetActionId.UnEquip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void UseSword(){
        // animate use sword
        _animator.SetBool(_animParamUseSword, true);
        if(IsServer) NetActionClientRpc(NetActionId.Use, NetworkManager.Singleton.ServerTime.Time);
    }

    private void OnTriggerEnter(Collider other) {
        // only can hit different character type
        CharacterObject attackC = GetComponent<CharacterObject>(), victimC = other.GetComponent<CharacterObject>();

        if(victimC != null && attackC.CharacterType != victimC.CharacterType){
            Debug.Log("sword hit");
            if(HitManager.Singleton != null) HitManager.Singleton.HitHandle(attackC, victimC);
            // one swipe can only hit one character 
            _swordCollider.enabled = false;
        }

    }

    [ClientRpc]
    private async void NetActionClientRpc(NetActionId netAction, double serverTime){
        if(_characterObject.OwnedByLocalUser == true) return;
        double waitTime = serverTime - NetworkManager.ServerTime.Time;
        if(waitTime > 0) await Task.Delay((int)(waitTime * 1000));

        switch (netAction)
        {
            case NetActionId.Equip:
                EquipSword();
                break;
            case NetActionId.AnimEquip:
                OnAnimationEquipSword();
                break;
            case NetActionId.UnEquip:
                UnEquipSword();
                break;
            case NetActionId.AnimUnEquip:
                OnAnimationUnEquipSword();
                break;
            case NetActionId.AnimDoneUse:
                OnAnimationDoneUseSword();
                break;
            case NetActionId.Use:
                UseSword();
                break;
        }
        
    }


}
