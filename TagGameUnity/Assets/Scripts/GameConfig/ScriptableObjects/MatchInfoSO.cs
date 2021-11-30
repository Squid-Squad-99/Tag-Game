using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.GameConfig{

    [CreateAssetMenu(menuName = "Game Config/Match Info")]
    public class MatchInfoSO : ScriptableObject
    {
        public bool Success;
        public string GameServerIP;
        public string GameServerPort;
        public string ConnectionAuthId;

        public void Set(bool success, string IP, string Port, string authId){
            Success = success;
            GameServerIP = IP;
            GameServerPort = Port;
            ConnectionAuthId = authId;
        }
    }

}
