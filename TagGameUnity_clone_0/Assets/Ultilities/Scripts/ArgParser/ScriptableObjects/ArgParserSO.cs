using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ultility.ArgParser{

    [CreateAssetMenu(menuName = "ArgParser/ArgParserSO")]
    public class ArgParserSO : ScriptableObject
    {
        [TextArea] public string description;

        
        [Tooltip("List of arg add to program")]
        public List<string> AddArgList;
        [Tooltip("If in editor, use this to be args")]
        public List<string> EditorModeArgs;

        private Dictionary<string, string> _argDict;
        public Dictionary<string, string> ArgDict {
            get{
                Init();
                return _argDict;
            }
        }
        public string GetArg(string key){
            // check if key is in add arg
            if(ArgDict.ContainsKey(key) == false){
                Debug.LogError($"[ArgParserSO] arg {key} isn't in AddArg");
            }

            return ArgDict[key];
        }

        public bool Contain(string key){
            return ArgDict.ContainsKey(key);
        }

        private void Init()
        {
            if(_argDict != null) return;
            List<string> arguments = new List<string>(Environment.GetCommandLineArgs());
            arguments.RemoveAt(0);
            if(Application.isEditor) arguments = EditorModeArgs;
            _argDict = new Dictionary<string, string>();
            for(int i = 0; i < AddArgList.Count; i++){
                _argDict.Add(AddArgList[i], arguments[i]);
            }
        }

    }

}
