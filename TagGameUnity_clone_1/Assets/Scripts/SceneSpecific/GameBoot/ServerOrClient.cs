using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ultility.ArgParser;

public class ServerOrClient : MonoBehaviour
{
    [SerializeField] GameObject ClientGameBooter;
    [SerializeField] GameObject ServerGameBooter;
    [SerializeField] ArgParserSO argParser;

    public enum NetTypeEnum{
        Server,
        Client
    }

    private void Awake() {
        NetTypeEnum netType;
        if(argParser.GetArg("gameServerId") != "-1")
        {
            netType = NetTypeEnum.Server;
        }
        else{
            netType = NetTypeEnum.Client;
        }
        switch (netType)
        {
            case NetTypeEnum.Server:
            Instantiate(ServerGameBooter);
                break;
            case NetTypeEnum.Client:
            Instantiate(ClientGameBooter);
                break;
            default:
                break;
        }
        
    }

}
