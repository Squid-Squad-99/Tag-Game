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
            int serverPort = 9999;
            Debug.Log("Start Connect");
            TcpClient client = new TcpClient(hostname, serverPort);
            NetworkStream stream = client.GetStream();
            Debug.Log("Connect success");

            // 2. send ticket to server
            WebSDK.Account account = UserHomeUIManager.Singleton.Account;
            Ticket ticket = new Ticket(account.UserId, account.Username, 100, GameModeEnum.GrabBall, characterType);
            string jsonTicket = JsonUtility.ToJson(ticket);
            Debug.Log($"ticket: {jsonTicket}");
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(jsonTicket);
            await stream.WriteAsync(data, 0, data.Length);

            // 3. await server give back match info
            data = new Byte[1024];
            int byteCnt = await stream.ReadAsync(data, 0, data.Length);
            string jsonMatchInfo = System.Text.Encoding.ASCII.GetString(data, 0, byteCnt);
            Debug.Log($"match info: {jsonMatchInfo}");

            // 4. return match info
            MatchInfo match = JsonUtility.FromJson<MatchInfo>(jsonMatchInfo);
            // MatchInfo match = new MatchInfo();
            // match.success = true;
            // match.GameServerIP = "127.0.0.1";
            // match.GameServerPort = "7777";
            // match.ConnectionAuthId = "1010";

            stream.Close();
            client.Close();

            await Task.Delay(1400);

            return match;
        }
             
        #endregion

        #region GameServerMethods

        [Serializable]
        public class ServerTicket{
            public ulong UserId;
            public string Username;
            public int Rank;
            public GameModeEnum GameMode;
            public CharacterTypeEnum CharacterType;
            public string ConnectionAuthId;
        }

        static public Dictionary<string, Ticket> Auth2Ticket;
        static public bool Connected = false;
        static public async Task<Dictionary<string, Ticket>> GetAuthIdToTicketDict(string gameServerId){
            // await Task.Delay(1000);
            // var dic = new Dictionary<string, Ticket>();
            // dic.Add("0001", new Ticket(1111,"Player1", 942, GameModeEnum.GrabBall, CharacterTypeEnum.Human)); 
            // dic.Add("0002", new Ticket(2222, "Player2", 455, GameModeEnum.GrabBall, CharacterTypeEnum.Ghost)); 
            // return dic;

            while(Connected == true){
                await Task.Delay(1000);
            }

            if(Auth2Ticket != null) return Auth2Ticket;

            // 1. connect to web server tcp socket
            Connected = true;
            string hostname = "taggame.dodofk.xyz";
            int serverPort = 9999;
            Debug.Log("Start Connect");
            TcpClient client = new TcpClient(hostname, serverPort);
            NetworkStream stream = client.GetStream();
            Debug.Log("Connect success");

            // 2. told I am server
            string imServer = "server";
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(imServer);
            await stream.WriteAsync(data, 0, data.Length);

            // 3. wait get server ticket list
            data = new Byte[1024 * 10];
            int byteCnt = await stream.ReadAsync(data, 0, data.Length);
            string jsonServerList = System.Text.Encoding.ASCII.GetString(data, 0, byteCnt);
            // Debug.Log($"Server List: {jsonServerList}");

            ServerTicket[] serverTickets = JsonHelper.FromJson<ServerTicket>(jsonServerList);

            // to dic
            Auth2Ticket = new Dictionary<string, Ticket>();
            foreach(var t in serverTickets){
                Auth2Ticket.Add(t.ConnectionAuthId, new Ticket(t.UserId, t.Username, 231, t.GameMode, t.CharacterType));
                // Debug.Log($"Ticket: {t.ConnectionAuthId} [{t.UserId}, {t.Username}, {t.GameMode}, {t.CharacterType}]");
            }

            
            stream.Close();
            client.Close();
            Connected = false;

            return Auth2Ticket;
        }

        #endregion
    }

}

//{"ServerTicketList": [{"UserId": 6, "Username": "henry", "Rank": 100, "GameMode": 0, "CharacterType": 1, "ConnectionAuthId": "a04c4b4b-7d76-3479-84b0-646b305d32b4"}, {"UserId": 2, "Username": "eason", "Rank": 100, "GameMode": 0, "CharacterType": 0, "ConnectionAuthId": "4bb9c1a0-ded7-3f16-93fb-bd2cbac9a815"}]}
