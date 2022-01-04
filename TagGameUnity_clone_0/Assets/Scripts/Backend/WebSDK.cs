using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tag.Backend{

    public class WebSDK
    {
        static string accessStr;
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Login with username and password, return whether success login
        /// </summary>
        static public async Task<bool> Login(string username, string password){
            // TODO
            
            await client.GetAsync("https://taggame.dodofk.xyz/auth/jwt/create/");

            return true;
        }

        static public async Task<Account> GetUserAccount(){
            // TODO
            await Task.Delay(1000);
            Account account = new Account();
            account.UserId = 1111;
            account.Username = "Eason0203";
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
            public ulong UserId;
            public string Username;
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

    }


}
