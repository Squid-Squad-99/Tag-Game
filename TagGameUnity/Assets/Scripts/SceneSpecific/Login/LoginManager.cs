using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using Ultility.Event;
using Ultility.Scene;

using Tag.Backend;

namespace Tag.Login{

    /// <summary>
    /// main class for handle login logic
    /// main job
    /// 1. main logic of login, (button press -> check valid -> change to User Home Scene if valid)
    /// 2. Raise event between main logic for other subclass to handle
    /// </summary>
    public class LoginManager : MonoBehaviour
    {

        // Note: normally will use SO event, but this is used in same gameobject, hence this will be enough
        [HideInInspector] public event Action<string, string> loginButtonPressEvent; 
        [HideInInspector] public event Action<string, string> LoginSuccessEvent; 
        [HideInInspector] public event Action<string, string> LoginFailEvent; 

        [Header("Reference")]
        [SerializeField] TMP_InputField _usernameInputField;
        [SerializeField] TMP_InputField _passwordInputField;
        [SerializeField] Button _loginButton;
        [SerializeField] SceneSO _userHomeScene;

        [Header("Broadcast Event")]
        [SerializeField] SceneEventChannelSO _requestLoadSceneChannel;

        private void OnEnable() {
            _loginButton.onClick.AddListener(OnLoginButtonPress);
        }

        private void OnDisable() {
            _loginButton.onClick.RemoveListener(OnLoginButtonPress);
        }

        private async void OnLoginButtonPress()
        {
            string username = _usernameInputField.text;
            string password = _passwordInputField.text;

            // invoke press event, delegate handle to other subclass
            loginButtonPressEvent?.Invoke(username, password);

            // validate username & password
            bool isValidInput = await WebSDK.Login(username, password);

            // invoke handler of valid/invalid input event
            if(isValidInput){
                // raise event of valid credential
                LoginSuccessEvent?.Invoke(username, password);

                // load user home scene
                _requestLoadSceneChannel.RaiseEvent(_userHomeScene);
            }
            else{
                // raise event of invalid credential
                LoginFailEvent?.Invoke(username, password);
            }

        }
    }

}
