using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

namespace Tag.Game.Managers{

    public class NetworkTimeBufferManager : NetworkBehaviour
    {
        public double DefaultLocalBufferms;  
        public double DefaultServerBufferms;  

        private NetworkTimeSystem _networkTimeSystem;

        public override void OnNetworkSpawn() {
            // cache network time system
            _networkTimeSystem = NetworkManager.Singleton.NetworkTimeSystem;

            // set default value
            _networkTimeSystem.LocalBufferSec = DefaultLocalBufferms * (0.001);
            _networkTimeSystem.ServerBufferSec = DefaultServerBufferms * (0.001);
        }
    }
    
}
