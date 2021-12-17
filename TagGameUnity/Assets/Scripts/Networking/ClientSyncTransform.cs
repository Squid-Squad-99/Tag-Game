using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;

public class ClientSyncTransform : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if(IsClient){
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTickClient;
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient){
            NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTickClient;
        }
    }

    private void OnTickClient()
    {
        // give server current transform value
        GiveTransformServerRpc(transform.position, transform.rotation, NetworkManager.LocalTime.Time);
    }

    [ServerRpc(RequireOwnership = false)]
    private async void GiveTransformServerRpc(Vector3 position, Quaternion rotation, double localTime){
        double waitTime = localTime - NetworkManager.ServerTime.Time;
        if(waitTime > 0){
            // wait to sync
            await Task.Delay((int)(waitTime * 1000));
        }
        
        // set transfrom
        if(transform.position != position) transform.position = position;
        if(transform.rotation != rotation) transform.rotation = rotation;
    }
}
