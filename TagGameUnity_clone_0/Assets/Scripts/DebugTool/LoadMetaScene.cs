using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tag.DebugTool{

    public class LoadMetaScene : MonoBehaviour
    {
        public enum HostTypeEnum{
            Server, 
            Client,
        }

        public HostTypeEnum hostType;

        void Awake()
        {
            // if more than 1 scene is load-> meta scene have load...
            if(SceneManager.sceneCount > 1) return;
            
            switch (hostType)
            {
                case HostTypeEnum.Client:
                    SceneManager.LoadScene("MetaClientScene");
                    break;
                default:
                    SceneManager.LoadScene("MetaServerScene");
                    break;
            }
            
        }

    }

}
