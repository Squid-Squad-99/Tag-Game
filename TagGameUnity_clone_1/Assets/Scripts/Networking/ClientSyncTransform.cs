using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;

using Tag.Game.Character;

public class ClientSyncTransform : NetworkBehaviour
{
    // reference
    private CharacterObject _characterObject;
    
    private void Awake() {
        _characterObject = GetComponentInParent<CharacterObject>();
        Debug.Assert(_characterObject != null);
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient && _characterObject.OwnedByLocalUser){
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTickClient;
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient && _characterObject.OwnedByLocalUser){
            NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTickClient;
        }
    }

    private void OnTickClient()
    {
        // give server current transform value
        GiveTransformServerRpc(transform.position, transform.rotation.eulerAngles, NetworkManager.LocalTime.Time);
    }

    [ServerRpc(RequireOwnership = false)]
    private async void GiveTransformServerRpc(Vector3 position, Vector3 rotation, double localTime){
        double waitTime = localTime - NetworkManager.ServerTime.Time;
        if(waitTime > 0){
            // wait to sync
            await Task.Delay((int)(waitTime * 1000));
        }
        
        // set transfrom
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
