using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ultility.ArgParser{

    [CreateAssetMenu(menuName = "ArgParser/ArgParserSO")]
    public class ArgParserSO : ScriptableObject
    {
        [TextArea] public string description;
        [Serializable]
        public struct AddArg{
            public string name_or_flag;
            [HideInInspector] public bool IsOptional => name_or_flag[0] == '-';

            public string key{
                get{
                    if(IsOptional){
                        // strip '-'
                        return name_or_flag.Substring(name_or_flag.LastIndexOf('-') + 1); 
                    }
                    return name_or_flag;
                }
            }
        }
        
        [Tooltip("List of arg add to program")]
        public List<AddArg> AddArgList;
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

        private void Init()
        {
            List<string> arguments = new List<string>(Environment.GetCommandLineArgs());
            if(Application.isEditor) arguments = EditorModeArgs;
            _argDict = new Dictionary<string, string>();
            //add positional arg to dic
            for(int i = 0; i < AddArgList.Count; i++){
                if(AddArgList[i].IsOptional) break;
                //check command line have suply this positional arg
                if(i >= arguments.Count || arguments[i][0] == '-'){
                    Debug.LogError($"[ArgParserSO] positional arg {AddArgList[i].name_or_flag} is not supplied");
                    return;
                }
                _argDict.Add(AddArgList[i].key, arguments[i]);
            }

            // add optional arg to dic
            for(int i = 0; i < AddArgList.Count; i++){
                if(AddArgList[i].IsOptional == false) continue;
                if(arguments.Contains(AddArgList[i].name_or_flag)){
                    int index = arguments.FindIndex(s => s == AddArgList[i].name_or_flag);
                    string value = "";
                    if(arguments[index+1][0] != '-'){
                        value = arguments[index+1];
                    }
                    _argDict.Add(AddArgList[i].key, value);
                }
            }
        }

    }

}
