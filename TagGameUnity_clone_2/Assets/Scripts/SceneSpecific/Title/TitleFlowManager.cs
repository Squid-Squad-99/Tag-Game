using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFlowManager : MonoBehaviour
{

    public static TitleFlowManager Singleton;
    private void Awake() {
        Singleton = this;
    }

    [SerializeField] GameObject SignIn;
    [SerializeField] GameObject SignUp;

    private void Update() {
        // sign in pop up
        if(SignIn.activeSelf == false && (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))){
            OpenSignIn();
        }
    }

    public void OpenSignIn(){
        SignIn.SetActive(true);
        CloseSingUp();
    }

    public void CloseSignIn(){
        SignIn.SetActive(false);
    }

    public void OpenSignUp(){
        SignUp.SetActive(true);
        CloseSignIn();
    }

    public void CloseSingUp(){
        SignUp.SetActive(false);
    }

}
