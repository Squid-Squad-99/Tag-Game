using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tag.Backend{

    public class WebSDK : MonoBehaviour
    {
        static string accessStr;
        private static readonly HttpClient client = new HttpClient();

        [Serializable] 
        public struct loginResponed{
            public string refresh;
            public string access;
        }

        /// <summary>
        /// Login with username and password, return whether success login
        /// </summary>
        static public async Task<bool> Login(string username, string password){
            // TODO
            string payload = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync("https://taggame.dodofk.xyz/auth/jwt/create/", c);

            if(result.StatusCode != System.Net.HttpStatusCode.OK){
                Debug.Log("Login Fail");
                return false;
            }

            string body = await result.Content.ReadAsStringAsync();
            var loginRes = JsonUtility.FromJson<loginResponed>(body);
            
            accessStr = loginRes.access;

            Debug.Log("Login Success");
            return true;
        }

        static public async Task<bool> SignUp(string username, string password){
            string payload = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync("https://taggame.dodofk.xyz/auth/users/", c);

            if(result.StatusCode != System.Net.HttpStatusCode.Created){
                Debug.Log(await result.Content.ReadAsStringAsync());
                Debug.Log("SignUp Fail");
                return false;
            }

            Debug.Log(await result.Content.ReadAsStringAsync());
            Debug.Log("SignUp Sccess");
            return true;
        }

        public struct GetUserAccountResponed{
            public ulong pk;
            public string username;
        }

        static public async Task<Account> GetUserAccount(){
            // TODO
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://taggame.dodofk.xyz/auth/users/me/"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessStr);
                HttpResponseMessage result = await client.SendAsync(requestMessage);
                if(result.StatusCode != System.Net.HttpStatusCode.OK){
                    Debug.Log("Get user data fail");
                    return new Account();
                }
                string body = await result.Content.ReadAsStringAsync();
                Debug.Log(body);
                var accountRes = JsonUtility.FromJson<GetUserAccountResponed>(body);
                Account account = new Account();
                account.UserId = accountRes.pk;
                account.Username = accountRes.username;
                return account;

            }
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
