using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkAnimation : NetworkBehaviour
{
    Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void SetFloat(int id, float value){
        if(IsClient) return;
        Debug.Assert(IsServer);
        _animator.SetFloat(id, value);
        SetFloatClientRpc(id, value);
    }

    public void SetBool(int id, bool value){
        if(IsClient) return;
        Debug.Assert(IsServer);
        _animator.SetBool(id, value);
        SetBoolClientRpc(id, value);
    }

    public void SetInteger(int id, int value){
        if(IsClient) return;
        Debug.Assert(IsServer);
        _animator.SetInteger(id, value);
        SetIntegerClientRpc(id, value);
    }

    [ClientRpc]
    private void SetFloatClientRpc(int id, float value){
        _animator.SetFloat(id, value);
    }

    [ClientRpc]
    private void SetBoolClientRpc(int id, bool value){
        _animator.SetBool(id, value);
    }

    [ClientRpc]
    private void SetIntegerClientRpc(int id, int value){
        _animator.SetInteger(id, value);
    }

}
