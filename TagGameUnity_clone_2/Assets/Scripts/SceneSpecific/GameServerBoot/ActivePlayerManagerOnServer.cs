using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Tag.GameServer;

public class ActivePlayerManagerOnServer : MonoBehaviour
{
    [SerializeField] PlayerManagerSO _playerManager;

    static private bool _haveInit = false;

    private void Start() {
        if(_haveInit || NetworkManager.Singleton.IsClient) return;
        _haveInit = true;
        _playerManager.InitPlayerManagerInServer();
    }
}
