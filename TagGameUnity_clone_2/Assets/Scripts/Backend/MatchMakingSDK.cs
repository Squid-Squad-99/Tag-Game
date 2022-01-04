using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Tag.Backend{
    public class MatchMakingSDK
    {
        
        public enum GameModeEnum{
            GrabBall = 0,
        }

        public enum CharacterTypeEnum{
            Human = 0,
            Ghost = 1,
        }
                
        public struct MatchInfo{
            public bool success;
            public string GameServerIP;
            public string GameServerPort;
            public string ConnectionAuthId;
        }

        public struct Ticket{
            public ulong UserId;
            public string Username;
            public int Rank;
            public GameModeEnum GameMode;
            public CharacterTypeEnum CharacterType;

            public Ticket(ulong userId, string username, int rank, GameModeEnum gameMode, CharacterTypeEnum characterType){
                UserId = userId;
                Username = username;
                Rank = rank;
                GameMode = gameMode;
                CharacterType = characterType;
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
        static public async Task<MatchInfo> FindMatch(GameModeEnum playMode, CharacterTypeEnum characterType){
            // TODO

            // 1. connect to web server tcp socket
            string hostname = "taggame.dodofk.xyz";
            int serverPort = 8888;
            TcpClient client = new TcpClient(hostname, serverPort);
            NetworkStream stream = client.GetStream();

            // 2. send ticket to server
            WebSDK.Account account = await WebSDK.GetUserAccount();
            Ticket ticket = new Ticket(account.UserId, account.Username, 100, GameModeEnum.GrabBall, characterType);
            string jsonTicket = JsonUtility.ToJson(ticket);
            Debug.Log($"ticket: {jsonTicket}");
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(jsonTicket);
            stream.Write(data, 0, data.Length);

            // 3. await server give back match info
            data = new Byte[1024];
            int byteCnt = stream.Read(data, 0, data.Length);
            string jsonMatchInfo = System.Text.Encoding.ASCII.GetString(data, 0, byteCnt);
            Debug.Log($"match info: {jsonMatchInfo}");

            // 4. return match info
            MatchInfo match = JsonUtility.FromJson<MatchInfo>(jsonMatchInfo);
            // MatchInfo match = new MatchInfo();
            // match.success = true;
            // match.GameServerIP = "127.0.0.1";
            // match.GameServerPort = "7777";
            // match.ConnectionAuthId = "1010";

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
            dic.Add("0001", new Ticket(1111,"Player1", 942, GameModeEnum.GrabBall, CharacterTypeEnum.Human)); 
            dic.Add("0002", new Ticket(2222, "Player2", 455, GameModeEnum.GrabBall, CharacterTypeEnum.Ghost)); 
            return dic;
        }


        #endregion
    }

}
