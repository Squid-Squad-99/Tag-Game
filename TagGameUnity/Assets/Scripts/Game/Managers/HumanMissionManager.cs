using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.Event;

public class HumanMissionManager : NetworkBehaviour
{
    
    #region Singleton
    static public HumanMissionManager Singleton;
    private void Awake() {
        HumanMissionManager.Singleton = this;
    }
    #endregion

    // if human collect enough skeleton box then it can destroy whole city, the ghost need to do it best to stop it
    // data
    [SerializeField] List<PickUp> _skullBoxes = new List<PickUp>();
    public NetworkVariable<int> Need2CollectCnt = new NetworkVariable<int>();
    public NetworkVariable<int> CurrentCollectCnt = new NetworkVariable<int>(); 

    public override void OnNetworkSpawn(){
        if(!IsServer) return;

        // add need to collect count
        PickUpManager.Singleton.RegisterEvent += (pickup) => {
            if(pickup.PickUpId == PickUpManager.PickUpIdEnum.SkullBox){
                Need2CollectCnt.Value++;
            }
        };

        // add current collect count
        PickUpManager.Singleton.PickUpEvent += (pickUpId, picker) => {
            if(pickUpId == PickUpManager.PickUpIdEnum.SkullBox){
                AddCurrentCollectCnt();
            }
        };
    }

    private void AddCurrentCollectCnt(){
        CurrentCollectCnt.Value++;
        // check have game end condition
        if(CurrentCollectCnt.Value >= Need2CollectCnt.Value){
            GameManager.Singleton.GameEnd(GameManager.GameEndReasonEnum.CollectEnoughSkullBox);
        }
    }

}
