using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using System;

using Ultility.Event;

namespace Tag.Networking
{

    public class SyncTransform : NetworkBehaviour
    {
        public enum ModeEnum
        {
            Sync,
            PredictionCorrection
        }

        [Header("Buffer")]
        [SerializeField] ModeEnum _mode;
        [SerializeField] int _maxSize;
        [SerializeField] int _pastEntriesCnt;
        [SerializeField] int _futerEntriesCnt;

        [Header("Sync Setting")]
        [SerializeField] float _positionSmoothTime;

        [Header("Debug")]
        [SerializeField] StringEventChannelSO _requestDisplay;

        // general data
        private NetworkList<TransformStamp> _networkTransformStampBuffer;
        private NetworkVariable <TransformStamp> _newestTransformStamp;
        // sync data
        private Vector3 _currentVelocity;

        // prediction corretion data


        #region Buffer Method
        
        private void BufferAdd(TransformStamp entry){
            // remove oldest when buffer full
            if(_networkTransformStampBuffer.Count >= _maxSize){
                _networkTransformStampBuffer.RemoveAt(0);
            }
            _networkTransformStampBuffer.Add(entry);
            _newestTransformStamp.Value = entry;
        }

        private void SetBufferPastFuterEntryiesCnt(double currentTime){
            int pastCnt = 0;
            foreach(TransformStamp s in _networkTransformStampBuffer){
                if(s.Stamp < currentTime) pastCnt++;
                else{
                    break;
                }
            }
            _pastEntriesCnt = pastCnt;
            _futerEntriesCnt = _networkTransformStampBuffer.Count - pastCnt;
        }

        private Tuple<TransformStamp?, TransformStamp?> GetLowUpBound(double currentTime){
            TransformStamp? lower = null, upper = null;

            // handle no entry
            if(_networkTransformStampBuffer.Count <= 10){
                return new Tuple<TransformStamp?, TransformStamp?>(lower, upper);
            }

            // all entry is in past
            if(_networkTransformStampBuffer[_networkTransformStampBuffer.Count - 1].Stamp < currentTime){
                lower = _networkTransformStampBuffer[_networkTransformStampBuffer.Count - 1];
                return new Tuple<TransformStamp?, TransformStamp?>(lower, upper);
            }

            // have past and futer entry
            for(int i = 1; i < _networkTransformStampBuffer.Count; i++){
                if(_networkTransformStampBuffer[i-1].Stamp < currentTime && currentTime <= _networkTransformStampBuffer[i].Stamp){
                    lower = _networkTransformStampBuffer[i-1];
                    upper = _networkTransformStampBuffer[i];
                    Debug.Assert(currentTime - lower.Value.Stamp >= 0);
                    Debug.Assert(currentTime - upper.Value.Stamp <= 0);
                    break;
                }
            }
            return new Tuple<TransformStamp?, TransformStamp?>(lower, upper);
        }
             
        #endregion

        public override void OnNetworkSpawn()
        {
            if(IsServer){
                NetworkManager.NetworkTickSystem.Tick += OnTickServer;
            }
            else{
                NetworkManager.NetworkTickSystem.Tick += OnTickClient;
                _newestTransformStamp.OnValueChanged += OnGetNewTransformStampClient;
            }
        }

        public override void OnNetworkDespawn()
        {
            if(IsServer){
                NetworkManager.NetworkTickSystem.Tick -= OnTickServer;
            }
            else{
                NetworkManager.NetworkTickSystem.Tick -= OnTickClient;
                _newestTransformStamp.OnValueChanged -= OnGetNewTransformStampClient;
            }
        }

        private void Update() {
            if(IsServer){

            }
            else{
                switch (_mode)
                {
                    case ModeEnum.Sync:
                        // update and sync to server transform value
                        SetTransformClient();
                        break;
                    case ModeEnum.PredictionCorrection:
                        break;
                    default:
                        break;
                }
                
            }
        }


        #region Client

        private void SetTransformClient()
        {
            // find target transform
            Vector3 targetPosition = Vector3.zero, targetRotation = Vector3.zero;

            // get lower & upper bound entry
            double currentServerTime = NetworkManager.NetworkTickSystem.ServerTime.Time;
            var (lower, upper) = GetLowUpBound(currentServerTime);

            // handle different buffer state for find target transfrom
            if(lower == null && upper == null){
                Debug.LogWarning("[Sync Transform] no buffer transfrom to sync");
            }
            else if(upper == null){
                Debug.LogWarning("[Sync Transform] buffer transform is too old");
                targetPosition = lower.Value.Position;
                targetRotation = lower.Value.Rotation;
            }
            else{
                Debug.Assert(lower != null && upper != null);
                float t = (float)((currentServerTime - lower.Value.Stamp) / (upper.Value.Stamp - lower.Value.Stamp));
                _requestDisplay.RaiseEvent($"t: {t}");
                targetPosition = Vector3.Lerp(lower.Value.Position, upper.Value.Position, t);
                targetRotation = Vector3.Lerp(lower.Value.Rotation, upper.Value.Rotation, t);
            }
            
            // set transform
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, _positionSmoothTime);
            transform.rotation = Quaternion.Euler(targetRotation);

        }

        private void OnTickClient()
        {   
            SetBufferPastFuterEntryiesCnt(NetworkManager.NetworkTickSystem.ServerTime.Time);
        }

        
        private void OnGetNewTransformStampClient(TransformStamp previousValue, TransformStamp newValue)
        {
            
        }
             
        #endregion


        #region Server
        private void OnTickServer()
        {
            // add current transform to buffer with stamp
            BufferAdd(new TransformStamp(
                NetworkManager.NetworkTickSystem.ServerTime.Time,
                transform.position,
                transform.rotation.eulerAngles
            ));
        }
             
        #endregion
    }


    public struct TransformStamp : IHaveStamp, IComparable<TransformStamp>, INetworkSerializable, IEquatable<TransformStamp>
    {
        private double _stamp;
        private Vector3 _position;
        private Vector3 _rotation;
        public double Stamp => _stamp;
        public Vector3 Position => _position;
        public Vector3 Rotation => _rotation;

        public TransformStamp(double stamp, Vector3 position, Vector3 rotation){
            _stamp = stamp; _position = position; _rotation = rotation;
        }

        public int CompareTo(TransformStamp other)
        {
            return Stamp.CompareTo(other.Stamp);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _stamp);
            serializer.SerializeValue(ref _position);
            serializer.SerializeValue(ref _rotation);
        }

        public bool Equals(TransformStamp other)
        {
            return Stamp.Equals(other.Stamp);
        }
    }

}
