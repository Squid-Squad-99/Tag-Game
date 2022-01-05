using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Tag.Backend;
using static Tag.Backend.MatchMakingSDK;

public class test : MonoBehaviour
{
    [Serializable]
    public class MyList{
        public List<ServerTicket> data = new List<ServerTicket>();
    }

    // // Start is called before the first frame update
    // void Start()
    // {
    //     // ServerTicket[] tickes = new ServerTicket[2];
    //     // tickes[0] = new ServerTicket();
    //     // tickes[0].Username = "Eason";
    //     // tickes[0].UserId = 11;
    //     // tickes[1] = new ServerTicket();
    //     // tickes[1].Username = "Henry";
    //     // tickes[1].UserId = 22;

    //     // string playerToJson = JsonHelper.ToJson(tickes, true);
    //     // Debug.Log(playerToJson);
    //     // return;

    //     string jsonServerList = "{\"ServerTicketList\": [{\"UserId\": 6, \"Username\": \"henry\", \"Rank\": 100, \"GameMode\": 0, \"CharacterType\": 1, \"ConnectionAuthId\": \"a04c4b4b-7d76-3479-84b0-646b305d32b4\"}, {\"UserId\": 2, \"Username\": \"eason\", \"Rank\": 100, \"GameMode\": 0, \"CharacterType\": 0, \"ConnectionAuthId\": \"4bb9c1a0-ded7-3f16-93fb-bd2cbac9a815\"}]}";
    //     string jsonServerList2 = "{\"Items\": [{\"UserId\": 6, \"Username\": \"henry\", \"Rank\": 100, \"GameMode\": 0, \"CharacterType\": 1, \"ConnectionAuthId\": \"a04c4b4b-7d76-3479-84b0-646b305d32b4\"}, {\"UserId\": 2, \"Username\": \"eason\", \"Rank\": 100, \"GameMode\": 0, \"CharacterType\": 0, \"ConnectionAuthId\": \"4bb9c1a0-ded7-3f16-93fb-bd2cbac9a815\"}]}";

    //     ServerTicket[] serverTickets = JsonHelper.FromJson<ServerTicket>(jsonServerList2);
    //     Debug.Log(jsonServerList2);
    //     Debug.Log(serverTickets[0].Username);
    //     Debug.Log(serverTickets[1].Username);
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}