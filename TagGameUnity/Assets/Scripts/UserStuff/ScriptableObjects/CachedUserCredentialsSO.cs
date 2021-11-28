using System;
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

        public bool HaveCached {get => cachedUserCredentials.HaveCached;}
        public string UserName {get => cachedUserCredentials.UserName;}
        public string Password {get => cachedUserCredentials.Password;}

        [Header("Attributes")]
        [SerializeField] CachedUserCredentials cachedUserCredentials;

        [Header("Reference")]
        [SerializeField] AppEnvironmentSO appEnvironmentSO;

        public void Save(){
            cachedUserCredentials.Save(appEnvironmentSO.ClientCredentialsPath);
        }

        public void Load(){
            cachedUserCredentials.Load(appEnvironmentSO.ClientCredentialsPath);
        }

    }

    [Serializable]
    public struct CachedUserCredentials{
        public bool HaveCached {get => _haveCached;}
        public string UserName {get=> _userName;}
        public string Password {get => _password;}

        [SerializeField] bool _haveCached;
        [SerializeField] string _userName;
        [SerializeField] string _password;

        /// <summary>
        /// Load credentials from disk
        /// </summary>
        public void Load(string path){
            var diskCredential = BinarySerialization.ReadFromBinaryFile<CachedUserCredentials>(path);
            _haveCached = diskCredential.HaveCached;
            _userName = diskCredential.UserName;
            _password = diskCredential.Password;
        }

        /// <summary>
        /// save credentials to disk
        /// </summary>
        public void Save(string path){
            BinarySerialization.WriteToBinaryFile<CachedUserCredentials>(path, this);
        }
    }

    [CustomEditor(typeof(CachedUserCredentialsSO))]
    // ^ This is the script we are making a custom editor for.
    public class YourScriptEditor: Editor {

        public override void OnInspectorGUI () {
            DrawDefaultInspector();

            if(GUILayout.Button("Save")) {
                Debug.Log("Save credential from CachedUserCredentialsSO inspector");
                ((CachedUserCredentialsSO)target).Save();
            }

            if(GUILayout.Button("Load")) {
                Debug.Log("Load credential from CachedUserCredentialsSO inspector");
                ((CachedUserCredentialsSO)target).Load();
            }
        }
    }

}