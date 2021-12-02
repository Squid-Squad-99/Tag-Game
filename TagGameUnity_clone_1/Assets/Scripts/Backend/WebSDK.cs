using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tag.Backend{

    public class WebSDK
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

    }


}
