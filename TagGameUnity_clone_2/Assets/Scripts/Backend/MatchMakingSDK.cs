using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Tag.Backend{
    public class MatchMakingSDK
    {
        
        public enum GameModeEnum{
            GrabBall = 0,
        }
                
        public struct MatchInfo{
            public bool success;
            public string GameServerIP;
            public string GameServerPort;
            public string ConnectionAuthId;
        }

        public struct Ticket{
            public ulong UserId;
            public GameModeEnum GameMode;

            public Ticket(ulong userId, GameModeEnum gameMode){
                UserId = userId;
                GameMode = gameMode;
            }
            
            // TODO
            // other attribute example: level, choosed character
        }
        
        #region ClientAppMethods
    
        /// <summary>
        /// Find a match by asking match making server,
        /// this is suppose to block for a long time
        /// </summary>
        /// <returns>match info</returns>
        static public async Task<MatchInfo> FindMatch(GameModeEnum playMode){
            // TODO

            // 1. connect to web server tcp socket

            // 2. send ticket to server

            // 3. await server give back match info

            // 4. return match info

            await Task.Delay(2000);

            MatchInfo match = new MatchInfo();
            match.success = true;
            match.GameServerIP = "127.0.0.1";
            match.GameServerPort = "7777";
            match.ConnectionAuthId = "1010";

            return match;
        }
             
        #endregion

        #region GameServerMethods

        /// <summary>
        /// Get the authId -> ticket dictionary for specify game server
        /// </summary>
        static public async Task<Dictionary<string, Ticket>> GetAuthIdToTicketDict(string gameServerId){
            await Task.Delay(1000);
            var dic = new Dictionary<string, Ticket>();
            dic.Add("1010", new Ticket(1234, GameModeEnum.GrabBall)); 
            // dic.Add("1", new Ticket("1", GameModeEnum.GrabBall)); 
            return dic;
        }


        #endregion
    }

}
