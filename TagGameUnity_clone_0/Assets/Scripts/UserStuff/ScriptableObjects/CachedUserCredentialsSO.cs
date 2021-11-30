using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Ultility.Serialization;

using Tag.AppEnvironment;

namespace Tag.UserStuff{

    /// <summary>
    /// Contain usercredentials if have cached
    /// </summary>
    [CreateAssetMenu(menuName = "UserStuff/User Credentials")]
    public class CachedUserCredentialsSO : ScriptableObject
    {
        [TextArea] public string Description;

        [Header("Attributes")]
        [SerializeField] bool _haveCached;
        public bool HaveCached {get => _haveCached;}
        public string UserName {get => cachedUserCredentials._userName;}
        public string Password {get => cachedUserCredentials._password;}

        [SerializeField] CachedUserCredentials cachedUserCredentials;

        [Header("Reference")]
        [SerializeField] AppEnvironmentSO appEnvironmentSO;

        private void OnEnable() {
            if(File.Exists(appEnvironmentSO.ClientCredentialsPath)){
                // load user credential when app start
                Load();
                _haveCached = true;
            }
            else{
                _haveCached = false;
            }
        }

        public void Cache(string username, string password){
            cachedUserCredentials._userName = username;
            cachedUserCredentials._password = password;
            Save();
        }

        public void Save(){
            BinarySerialization.WriteToBinaryFile<CachedUserCredentials>(appEnvironmentSO.ClientCredentialsPath, cachedUserCredentials);
            _haveCached = true;
        }

        public void Load(){
            cachedUserCredentials = BinarySerialization.ReadFromBinaryFile<CachedUserCredentials>(appEnvironmentSO.ClientCredentialsPath);
            _haveCached = true;
        }

        public void Delete(){
            File.Delete(appEnvironmentSO.ClientCredentialsPath);
            _haveCached = false;
        }
    }

    [Serializable]
    internal struct CachedUserCredentials{
        public string _userName;
        public string _password;
    }



}