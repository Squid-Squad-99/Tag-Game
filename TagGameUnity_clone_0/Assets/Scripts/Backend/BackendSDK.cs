using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tag.Backend{

    public class BackendSDK
    {
        /// <summary>
        /// Login with username and password, return whether success login
        /// </summary>
        static public async Task<bool> Login(string username, string password){
            // TODO
            
            await Task.Delay(1000);

            return true;
        }

        /// <summary>
        /// Find a match by asking server,
        /// this is suppose to block for a long time
        /// </summary>
        /// <returns>match info</returns>
        static public async Task<Match> FindMatch(GameModeEnum playMode){
            // TODO

            await Task.Delay(2000);

            Match match = new Match();
            match.success = true;
            match.GameServerIP = "127.0.0.1";
            match.GameServerPort = "7777";
            match.ConnectionAuthId = "";

            return match;
        }
        public struct Match{
            public bool success;
            public string GameServerIP;
            public string GameServerPort;
            public string ConnectionAuthId;
        }

        public enum GameModeEnum{
            GrabBall = 0,
        }
    }


}
