using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;

using Tag.Game.Character;

public class ClientSyncSmoothTransform : NetworkBehaviour
{
    // reference
    private CharacterObject _characterObject;
    private Rigidbody _rigidbody;

    // data
    [SerializeField] List<TransformStamp> _transformBuffer = new List<TransformStamp>();

    private void Awake() {
        _characterObject = GetComponentInParent<CharacterObject>();
        _rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(_characterObject != null);
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient && _characterObject.OwnedByLocalUser){
            // hook tick
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTickClient;
        }
    }

    private void Update() {
        if(IsClient && _characterObject.OwnedByLocalUser == false && _transformBuffer.Count > 0){
            // play interprelate transfrom in buffer
            double currentServerTime = NetworkManager.Singleton.ServerTime.Time;
            PlaySmoothTransform(currentServerTime);
        }
    }

    private void PlaySmoothTransform(double currentServerTime)
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        // handle buffer useless case
        if(_transformBuffer.Count == 0){
            // no buffer -> do nothing
        }
        else if(currentServerTime < _transformBuffer[0].Timestamp){
            // no entry for current server time -> just set to closer
            position = _transformBuffer[0].Position;
            rotation = _transformBuffer[0].Rotation;
        }
        else if(_transformBuffer[_transformBuffer.Count - 1].Timestamp < currentServerTime){
            // current ahead buffer timestamp
            position = _transformBuffer[_transformBuffer.Count - 1].Position;
            rotation = _transformBuffer[_transformBuffer.Count - 1].Rotation;
        }
        else{
            // normal case -> interprelate and discard old buffer
            TransformStamp lower = null, upper = null;
            bool finded = false;
            // find lower and upper bound
            for(int i = 0; i < _transformBuffer.Count - 1; i++){
                if(_transformBuffer[i].Timestamp <= currentServerTime && currentServerTime < _transformBuffer[i+1].Timestamp){
                    finded = true;
                    lower = _transformBuffer[i]; 
                    upper = _transformBuffer[i+1];
                    // remove old entries
                    int oldEntryCnt = i;
                    _transformBuffer.RemoveRange(0, oldEntryCnt);
                    break;
                }
            }
            Debug.Assert(finded);

            // get interprelate transform value
            float t = (float)((currentServerTime - lower.Timestamp) / (upper.Timestamp - lower.Timestamp));
            position = Vector3.Lerp(lower.Position, upper.Position, t);
            rotation = Quaternion.Slerp(lower.Rotation, upper.Rotation, t);
        }

        // set transform
        transform.position = position;
        transform.rotation = rotation;
        // _rigidbody.MovePosition(position);
        // _rigidbody
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient && _characterObject.OwnedByLocalUser){
            NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTickClient;
        }
    }

    private void OnTickClient()
    {
        // give server current transform value
        GiveTransformServerRpc(transform.position, transform.rotation, NetworkManager.LocalTime.Time);
    }

    [ServerRpc(RequireOwnership = false)]
    private async void GiveTransformServerRpc(Vector3 position, Quaternion rotation, double localTime){
        double waitTime = localTime - NetworkManager.ServerTime.Time;
        if(waitTime > 0){
            // wait to sync
            await Task.Delay((int)(waitTime * 1000));
        }
        
        // set transfrom
        transform.position = position;
        transform.rotation = rotation;

        // broadcast to client
        GetNewTransformStampClientRpc(position, rotation, NetworkManager.Singleton.ServerTime.Time);
    }

    [ClientRpc]
    private void GetNewTransformStampClientRpc(Vector3 position, Quaternion rotation, double serverTime){
        // local user control this transform
        if(_characterObject.OwnedByLocalUser) return;
        // if get earlier stamp, discard it
        if(_transformBuffer.Count > 0 && serverTime < _transformBuffer[_transformBuffer.Count - 1].Timestamp) return;
        // buffer transform for other user
        _transformBuffer.Add(new TransformStamp(position, rotation, serverTime));
    }

    [Serializable]
    public class TransformStamp{
        public Vector3 Position;
        public Quaternion Rotation;
        public double Timestamp;
        public TransformStamp(Vector3 position, Quaternion rotation, double timestamp){
            Position = position; Rotation = rotation; Timestamp = timestamp; 
        } 
    }
}

