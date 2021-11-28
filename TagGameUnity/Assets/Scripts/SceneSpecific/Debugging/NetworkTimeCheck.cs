using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

using Ultility.Event;

namespace Tag.Debugging{

    /// <summary>
    /// Show information about networking time(pin, local time, server time)
    /// </summary>
    public class NetworkTimeCheck : NetworkBehaviour
    {
        [Header("BroadCast Channel")]
        [SerializeField] StringEventChannelSO RequestDebugDisplayChannel;

        public override void OnNetworkSpawn(){
            NetworkManager.NetworkTickSystem.Tick += OnTick;
            
            if(IsClient) StartCoroutine(PinTest(0.2f));
        }
        public override void OnNetworkDespawn(){
            NetworkManager.NetworkTickSystem.Tick -= OnTick;
        }


        private void OnTick()
        {
            var localTime = NetworkManager.NetworkTimeSystem.LocalTime;
            var serveTime = NetworkManager.NetworkTimeSystem.ServerTime;
            var localBufferSec = NetworkManager.NetworkTimeSystem.LocalBufferSec;
            var serverBufferSec = NetworkManager.NetworkTimeSystem.ServerBufferSec;
            RequestDebugDisplayChannel.RaiseEvent($"localTime: {Math.Round(localTime, 4)} sec");
            RequestDebugDisplayChannel.RaiseEvent($"serveTime: {Math.Round(serveTime, 4)} sec");
            RequestDebugDisplayChannel.RaiseEvent($"Local server time diff: {(int)((localTime - serveTime)*1000)} ms");
            RequestDebugDisplayChannel.RaiseEvent($"localBufferSec: {Math.Round(localBufferSec, 4)*1000} ms");
            RequestDebugDisplayChannel.RaiseEvent($"serverBufferSec: {Math.Round(serverBufferSec, 4)*1000} ms");
        }


        private IEnumerator PinTest(float timeInterval)
        {
            while(true){
                float startTime = Time.time;
                PinServer_ServerRpc(startTime, NetworkManager.LocalTime.TimeAsFloat);
                yield return new WaitForSeconds(timeInterval);
            }
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Unreliable)]
        private void PinServer_ServerRpc(float startTime, float clientLocalTime, ServerRpcParams serverRpcParams = default){
            float serverProccessTime = Time.time;
            ulong senderId = serverRpcParams.Receive.SenderClientId;
            ClientRpcParams clientRpcParams = new ClientRpcParams{
                Send = new ClientRpcSendParams{
                    TargetClientIds = new ulong[]{senderId}
                }
            };
            RequestDebugDisplayChannel.RaiseEvent($"clientLocalTime - serverTime: {Mathf.Round(1000*(clientLocalTime - NetworkManager.ServerTime.TimeAsFloat))} ms");
            serverProccessTime = Time.time - serverProccessTime;
            PinClientBack_ClientRpc(startTime + serverProccessTime, NetworkManager.ServerTime.TimeAsFloat, clientRpcParams);
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void PinClientBack_ClientRpc(float startTime, float serverServerTime, ClientRpcParams clientRpcParams){
            float pin = Time.time - startTime;
            RequestDebugDisplayChannel.RaiseEvent($"Pin: {(int)(pin*1000)} ms");
            RequestDebugDisplayChannel.RaiseEvent($"serverServerTime - serverTime: {Mathf.Round(1000*(serverServerTime - NetworkManager.ServerTime.TimeAsFloat))} ms");
        }
            
    }

}
