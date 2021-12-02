using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tag.ServerBooting{

    [CustomEditor(typeof(StartGameServerOnRequest))]
    public class StartGameServerOnRequestEditor : Editor
    {
        public override void OnInspectorGUI () {
            DrawDefaultInspector();
            if(GUILayout.Button("Start Server")) {
                ((StartGameServerOnRequest)target).StartGameServer();
            }
        }
    }

}
