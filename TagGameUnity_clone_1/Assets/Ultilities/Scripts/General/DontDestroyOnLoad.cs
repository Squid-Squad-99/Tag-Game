using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultility.General{

    /// <summary>
    /// Let this gameobject dont destroy on loading scene
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }

}

