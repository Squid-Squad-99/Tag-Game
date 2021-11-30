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

        static public async Task<Account> GetUserAccount(){
            // TODO
            await Task.Delay(1000);
            Account account = new Account();
            account.Email = "root@gmail.com";
            account.Age = 20;
            return account;
        }

        static public async Task<VirtualAsset> GetUserVirtualAsset(){
            // TODO
            await Task.Delay(1000);
            VirtualAsset virtualAsset = new VirtualAsset();
            virtualAsset.Level = 10;
            virtualAsset.CoinCnt = 500;
            return virtualAsset;
        }

        static public async Task<GameStats> GetUserGameStats(){
            // TODO
            await Task.Delay(1000);
            return new GameStats();
        }

        public struct Account{
            public string Email;
            public int Age;
        }

        public struct GameStats{
            public int Rank;
            public int VictoryCnt;
            public int HighestRank;
        }

        public struct VirtualAsset{
            public int Level;
            public int CoinCnt;

        }

        /// <summary>
        /// Find a match by asking server,
        /// this is suppose to block for a long time
        /// </summary>
        /// <returns>match info</returns>
        static public async Task<Match> FindMatch(GameModeEnum playMode){
            // TODO

            // 1. connect to web server tcp socket

            // 2. send ticket to server

            // 3. await server give back match info

            // 4. return match info

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
