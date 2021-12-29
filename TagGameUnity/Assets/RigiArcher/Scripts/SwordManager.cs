using System.Collections;
using System.Collections.Generic;
using RigiArcher.MeshSocket;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

public class SwordManager : NetworkBehaviour
{
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

    // reference
    private Collider _swordCollider;
    private MeshSocketManager _meshSocketManager;
    private Animator _animator;
    private int _animParamEquipSword;
    private int _animParamUseSword;


    private void Start() {
        // reference
        _meshSocketManager = GetComponent<MeshSocketManager>();
        _animator = GetComponent<Animator>();
        _swordCollider = _swordGameObject.GetComponent<Collider>();

        // anim param
        _animParamEquipSword = Animator.StringToHash("EquipSword");
        _animParamUseSword = Animator.StringToHash("UseSword");

        // 
        _meshSocketManager.Attach(_swordGameObject.transform, MeshSocketManager.SocketIdEnum.Back);
    }

    public void EquipSword(){
        // animate equip
        _animator.SetBool(_animParamEquipSword, true);
        NetActionClientRpc(NetActionId.Equip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void OnAnimationEquipSword(){
        _meshSocketManager.Attach(_swordGameObject.transform, MeshSocketManager.SocketIdEnum.RightHand);
        NetActionClientRpc(NetActionId.AnimEquip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void OnAnimationUnEquipSword(){
        _meshSocketManager.Attach(_swordGameObject.transform, MeshSocketManager.SocketIdEnum.Back);
        NetActionClientRpc(NetActionId.AnimUnEquip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void OnAnimationDoneUseSword(){
        _animator.SetBool(_animParamUseSword, false);
        _swordCollider.enabled = false;
        NetActionClientRpc(NetActionId.AnimDoneUse, NetworkManager.Singleton.ServerTime.Time);
    }

    public void UnEquipSword(){
        // animate Unequip
        _animator.SetBool(_animParamEquipSword, false);
        NetActionClientRpc(NetActionId.UnEquip, NetworkManager.Singleton.ServerTime.Time);
    }

    public void UseSword(){
        // animate use sword
        _animator.SetBool(_animParamUseSword, true);
        _swordCollider.enabled = true;
        NetActionClientRpc(NetActionId.Use, NetworkManager.Singleton.ServerTime.Time);
    }

    private void OnTriggerEnter(Collider other) {
        // check didn't hit ourseft
        if(other.gameObject == gameObject) return;
        Debug.Log("sword hit");
    }

    [ClientRpc]
    private async void NetActionClientRpc(NetActionId netAction, double serverTime){
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
