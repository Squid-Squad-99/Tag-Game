using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.Event;
using Tag.Game.Character;
using System;
using System.Threading.Tasks;

public class PickUp : NetworkBehaviour
{
    [Header("Pick Up Setting")]
    public PickUpManager.PickUpIdEnum PickUpId;
    public List<CharacterObject.CharacterTypeEnum> WhoCanPickUp;
    public GameObject PickUpVFX;

    public event Action<CharacterObject> PickUpEvent;

    protected IEnumerator Start() {
        yield return null;
        if(PickUpManager.Singleton != null) PickUpManager.Singleton.RegisterPickUp(this); 
    }


    private void OnTriggerEnter(Collider other) {
        CharacterObject characterObject = other.GetComponent<CharacterObject>();
        Debug.Assert(characterObject);

        // check can this character pick up
        bool canPickup = WhoCanPickUp.Exists(x => x == characterObject.CharacterType);

        if(!canPickup) return;

        // pick up happend
        OnPickUp();
        PickUpEvent?.Invoke(characterObject);
    }

    private void OnPickUp(){
        if(IsServer){
            if(PickUpVFX != null) Instantiate(PickUpVFX, transform.position, transform.rotation);
            gameObject.SetActive(false);

            OnPickUpClientRpc(NetworkManager.Singleton.ServerTime.Time);
        }   
        else{

        }
    }

    [ClientRpc]
    private async void OnPickUpClientRpc(double serverTime){
        double waitTime = serverTime - NetworkManager.Singleton.ServerTime.Time;
        if(waitTime > 0) await Task.Delay( (int)(waitTime * 1000) );

        if(PickUpVFX != null) Instantiate(PickUpVFX, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}
