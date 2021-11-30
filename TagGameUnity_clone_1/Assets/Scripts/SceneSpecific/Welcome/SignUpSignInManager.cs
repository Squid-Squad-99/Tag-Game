using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

using Tag.AppEnvironment;

using Ultility.Event;
using Ultility.Scene;

/// <summary>
/// Main job
/// 1. change to login scene when sign in button press
/// 2. redirect web sign up when sign up button press -> change to login scene
/// </summary>
public class SignUpSignInManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Button _signUpButton; 
    [SerializeField] Button _signInButton; 
    [SerializeField] AppEnvironmentSO _appEnvironmentSO;
    [SerializeField] SceneSO _loginScene;
    
    [Header("Broadcast Channel")]
    [SerializeField] SceneEventChannelSO _requestLoadSceneChannel;
    

    private void OnEnable() {
        _signInButton.onClick.AddListener(OnSignInButtonClick);
        _signUpButton.onClick.AddListener(OnSignUpButtonClick);
    }

    private void OnDisable() {
        _signInButton.onClick.RemoveListener(OnSignInButtonClick);
        _signUpButton.onClick.RemoveListener(OnSignUpButtonClick);
    }

    private void OnSignUpButtonClick()
    {
        // open sign up web page
        Process.Start(_appEnvironmentSO.SignUpURL);

        //change scene
        _requestLoadSceneChannel.RaiseEvent(_loginScene);
    }

    private void OnSignInButtonClick()
    {
        // Change Scene
        _requestLoadSceneChannel.RaiseEvent(_loginScene);
    }
}
