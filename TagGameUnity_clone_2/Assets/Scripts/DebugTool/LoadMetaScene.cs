using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tag.DebugTool{

    public class LoadMetaScene : MonoBehaviour
    {

        void Awake()
        {
            // if more than 1 scene is load-> meta scene have load...
            if(SceneManager.sceneCount > 1) return;
            
            SceneManager.LoadScene("MetaScene");
            
        }

    }

}
