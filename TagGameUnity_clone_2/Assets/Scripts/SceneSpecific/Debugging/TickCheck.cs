using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

using Ultility.Event;

public class TickCheck : NetworkBehaviour
{

    [Header("BroadCast Channel")]
    [SerializeField] StringEventChannelSO RequestDebugDisplayChannel;


    private int callPerSec = 0;
    private int updatePerSec = 0;

    public override void OnNetworkSpawn()
    {
        NetworkManager.NetworkTickSystem.Tick += OnTick;
        if(IsServer) StartCoroutine(CalcallPersec());
    }

    private IEnumerator CalcallPersec()
    {
        while(true){
            yield return new WaitForSeconds(1);
            RequestDebugDisplayChannel.RaiseEvent($"Rpc per sec: {callPerSec}");
            RequestDebugDisplayChannel.RaiseEvent($"update per sec: {updatePerSec}");
            callPerSec = 0;
            updatePerSec = 0;
        }
    }

    private void OnTick()
    {
    }

    private void Update() {
        if(IsClient) ServerCall_ServerRpc();
        updatePerSec++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerCall_ServerRpc(){
        //todo: cal how may rpc server get per sec
        callPerSec++;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -=OnTick;   
    }
}
