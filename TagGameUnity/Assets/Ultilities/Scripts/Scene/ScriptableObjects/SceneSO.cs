using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultility.Scene{

	[CreateAssetMenu(menuName = "Scenes/SceneSO")]
    public class SceneSO : ScriptableObject
    {
        [TextArea] public string description;
        public string SceneName;
    }

}
