using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.AppEnvironment {

    /// <summary>
    /// just like the .env file, this SO contain the config of the app
    /// </summary>
    [CreateAssetMenu(menuName = "AppEnvironment/AppEnvironmentSO")]
    public class AppEnvironmentSO : ScriptableObject
    {
        [TextArea] public string Description;

        [Header("Attributes")]
        [Tooltip("The relative path to the file that storing client credentials. [relate to Application.persistentDataPath]")]
        [SerializeField] List<string> _clientCredentialsPath;

        [Tooltip("Sign up account URL")]
        [SerializeField] string _signUpURL;

        /// <summary>
        /// Sign up account URL
        /// </summary>
        public string SignUpURL => _signUpURL;

        /// <summary>
        /// Full path to the file that storing client credentials.
        /// </summary>
        public string ClientCredentialsPath{
            get{
                var cp = new List<string>(_clientCredentialsPath);
                cp.Insert(0, Application.persistentDataPath);
                string[] path =  cp.ToArray();
                return Path.Combine(path);
            }
        }
    }

}
