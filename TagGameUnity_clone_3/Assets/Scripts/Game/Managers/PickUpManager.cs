using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Tag.Game.Character;
using System.Threading.Tasks;

public class PickUpManager : NetworkBehaviour
{
    #region Singleton
    static public PickUpManager Singleton;
    private void Awake() {
        PickUpManager.Singleton = this;
    }
    #endregion

    public enum PickUpIdEnum{
        SkullBox,
    }

    public event Action<PickUpIdEnum, CharacterObject> PickUpEvent;
    public event Action<PickUp> RegisterEvent;

    // data
    private List<PickUp> _pickUps = new List<PickUp>(); 


    public void RegisterPickUp(PickUp pickUp){
        _pickUps.Add(pickUp);
        pickUp.PickUpEvent += (picker) => {
            PickUpEvent?.Invoke(pickUp.PickUpId, picker);
        };
        RegisterEvent?.Invoke(pickUp);
    }
}
