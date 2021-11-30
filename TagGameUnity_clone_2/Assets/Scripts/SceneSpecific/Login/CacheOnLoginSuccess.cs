using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tag.UserStuff;
using System;

namespace Tag.Login{

    public class CacheOnLoginSuccess : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] CachedUserCredentialsSO cachedUserCredentials;
        LoginManager _loginManager;

        private void Awake() {
            _loginManager = GetComponent<LoginManager>();
        }

        private void Start() {
            _loginManager.LoginSuccessEvent += OnLoginSuccess;
        }

        private void OnDestroy() {
            if(_loginManager == null) return;
            _loginManager.LoginSuccessEvent -= OnLoginSuccess;
        }

        private void OnLoginSuccess(string username, string password)
        {
            cachedUserCredentials.Cache(username, password);
        }
    }

}
