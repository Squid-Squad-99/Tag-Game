using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFlowManager : MonoBehaviour
{
    [SerializeField] GameObject SignIn;

    private void Update() {
        // sign in pop up
        if(SignIn.activeSelf == false && (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))){
            SignIn.SetActive(true);
        }
    }

    public void CloseSignIn(){
        SignIn.SetActive(false);
    }

}
