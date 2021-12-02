using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tag.ServerBooting{

    [CustomEditor(typeof(LoadGameSceneOnRequest))]
    public class LoadGameSceneOnRequestEditor : Editor
    {
        public override void OnInspectorGUI () {
            DrawDefaultInspector();
            if(GUILayout.Button("Load Game Scene")) {
                ((LoadGameSceneOnRequest)target).LoadGameScene();
            }
        }
    }

}

