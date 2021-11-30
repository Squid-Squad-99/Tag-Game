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

    [CustomEditor(typeof(CachedUserCredentialsSO))]
    // ^ This is the script we are making a custom editor for.
    public class YourScriptEditor: Editor {

        public override void OnInspectorGUI () {
            DrawDefaultInspector();

            if(GUILayout.Button("Save")) {
                Debug.Log("Save credential to disk");
                ((CachedUserCredentialsSO)target).Save();
            }

            if(GUILayout.Button("Load")) {
                Debug.Log("Load credential from disk");
                ((CachedUserCredentialsSO)target).Load();
            }

            if(GUILayout.Button("Delete")) {
                Debug.Log("Delete credential in disk");
                ((CachedUserCredentialsSO)target).Delete();
            }
        }
    }

}