using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tag.UserStuff{

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

namespace Tag.ClientBooting{

    [CustomEditor(typeof(GameServerBooter))]
    public class YourScriptEditor:Editor{
        public override void OnInspectorGUI () {
            DrawDefaultInspector();
            if(GUILayout.Button("Load Game Scene")){
                ((GameServerBooter)target).LoadGameScene();
            }
        }
    }
}