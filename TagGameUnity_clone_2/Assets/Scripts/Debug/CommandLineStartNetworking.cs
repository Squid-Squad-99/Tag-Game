using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Ultility.ArgParser;

namespace Tag.Networking{

    public class CommandLineStartNetworking : NetworkBehaviour
    {
        [SerializeField] ArgParserSO argParser;

        private IEnumerator Start() {
            if(Application.isEditor == false){
                // wait 1 frame for every have run start
                yield return null;
                string network = argParser.GetArg("network");

                switch (network)
                {
                    case "client":
                        NetworkManager.StartClient();
                        break;
                    case "server":
                        NetworkManager.StartServer();
                        break;
                    default:
                        Debug.LogError($"network arg didn't match client or server, get {network}");
                        break;
                }

            }
            

        }
    }

}
