using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using System;

using Ultility.Event;
using Tag.Game.Character;

namespace Tag.Networking
{

    public class SyncTransform : NetworkBehaviour
    {
        public enum ModeEnum
        {
            Sync,
            PredictionCorrection
        }

        public ModeEnum Mode;
        [Header("Network Buffer")]
        [SerializeField] int _networkBufferMaxSize;
        [SerializeField] int _pastEntriesCnt;
        [SerializeField] int _futerEntriesCnt;

        [Header("Local Buffer")]
        [SerializeField] int _localBufferSize;

        [Header("Sync Setting")]
        [SerializeField] float _positionSmoothTime;
        [SerializeField] float _rotationSmoothTime;


        [Header("Prediction Correction Setting")]
        [SerializeField] float _correctPositionThreadHold;
        [SerializeField] float _correctRotationThreadHold;

        [Header("Debug")]
        [SerializeField] StringEventChannelSO _requestDisplay;
        public Vector3 CorrectPosition, CorrectRotation;
        public Vector3 PredictPosition, PredictRotation;
        public Vector3 FixDeltaPosition, FixDeltaRotation;

        // general data
        private NetworkList<TransformStamp> _networkTransformStampBuffer;
        private NetworkVariable <TransformStamp> _newestTransformStamp;

        // sync data
        private Vector3 _currentRotateVelocity;
        private Vector3 _currentPositionVelocity;

        // prediction corretion data
        private List<TransformStamp> _localTransfomrStampBuffer = new List<TransformStamp>();
        private Vector3 _targetPosition;
        private Vector3 _targetRotation;
        private Vector3 _lastFixDeltaPosition;
        private Vector3 _lastFixDeltaRotation;

        // reference 
        Rigidbody _rigidbody;


        #region Network Buffer Method
        
        private void NetworkTransformStampBufferAddServer(TransformStamp entry){
            // remove oldest when buffer full
            if(_networkTransformStampBuffer.Count >= _networkBufferMaxSize){
                _networkTransformStampBuffer.RemoveAt(0);
            }
            _networkTransformStampBuffer.Add(entry);
            _newestTransformStamp.Value = entry;
        }

        private void SetNetworkBufferPastFuterEntryiesCnt(double currentTime){
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

        private Tuple<TransformStamp?, TransformStamp?> NetworkBUfferGetLowUpBound(double currentTime){
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

        #region Local Buffer Method

        private void LocalBufferAdd(TransformStamp entry){
            _localTransfomrStampBuffer.Add(entry);
            _localBufferSize = _localTransfomrStampBuffer.Count;
        }
        
        #endregion

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
        }

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
                switch (Mode)
                {
                    case ModeEnum.Sync:
                        // update and sync to server transform value
                        SyncTransformClient();
                        break;
                    case ModeEnum.PredictionCorrection:
                        break;
                    default:
                        break;
                }
                
            }
        }


        #region Client

        private void SyncTransformClient()
        {
            // find target transform
            Vector3 targetPosition = Vector3.zero, targetRotation = Vector3.zero;

            // get lower & upper bound entry
            double currentServerTime = NetworkManager.NetworkTickSystem.ServerTime.Time;
            var (lower, upper) = NetworkBUfferGetLowUpBound(currentServerTime);

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
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentPositionVelocity, _positionSmoothTime);
            transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(
                transform.rotation.eulerAngles,
                targetRotation,
                ref _currentRotateVelocity,
                _rotationSmoothTime
            ));

        }

        /// <summary>
        /// Correct transform (run in update)
        /// </summary>
        private void CorrectingTransfromClient(){
            _requestDisplay.RaiseEvent($"off pos: {_lastFixDeltaPosition.magnitude}");
            if(_lastFixDeltaPosition.magnitude >= _correctPositionThreadHold){
                Debug.Log("fix position");
                Vector3 position = Vector3.Lerp(transform.position, _targetPosition, _positionSmoothTime);
                // apply to transfrom
                transform.position = position;
                // apply delta to buffer
                for(int i = 0; i < _localTransfomrStampBuffer.Count; i++){
                    _localTransfomrStampBuffer[i] = new TransformStamp(
                        _localTransfomrStampBuffer[i].Stamp,
                        Vector3.Lerp(_localTransfomrStampBuffer[i].Position, position, _positionSmoothTime),
                        _localTransfomrStampBuffer[i].Rotation 
                    );
                }
            }

            _requestDisplay.RaiseEvent($"off angle: {Mathf.Abs(Quaternion.Angle(Quaternion.Euler(_lastFixDeltaRotation), Quaternion.identity))}");
            if(Mathf.Abs(Quaternion.Angle(Quaternion.Euler(_lastFixDeltaRotation), Quaternion.identity)) >= _correctRotationThreadHold){
                Debug.Log("fix rotation");
                Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_targetRotation), _rotationSmoothTime);
                // apply to transform
                transform.rotation = rotation;
                // apply delta to buffer
                for(int i = 0; i < _localTransfomrStampBuffer.Count; i++){
                    _localTransfomrStampBuffer[i] = new TransformStamp(
                        _localTransfomrStampBuffer[i].Stamp,
                        _localTransfomrStampBuffer[i].Position,
                        Quaternion.Slerp(Quaternion.Euler(_localTransfomrStampBuffer[i].Rotation), rotation, _positionSmoothTime).eulerAngles 
                    );
                }
            }
        }
        
        /// <summary>
        /// Get server newest transform stamp, and do correction
        /// </summary>
        /// <param name="newTransformStamp"></param>
        private void GetPredictionCorrectionTargetValueClient(TransformStamp newTransformStamp)
        {
            if(_localTransfomrStampBuffer.Count <= 10){
                // wait for buffer to full
                return;
            }

            // find local buffer entry of bound of new transform stamp
            TransformStamp? lower = null, upper = null;
            int lowerIndex = 1, upperIndex = 1;
            for(int i = 1; i < _localTransfomrStampBuffer.Count; i++){
                if(newTransformStamp.Stamp <= _localTransfomrStampBuffer[i].Stamp){
                    lowerIndex = i - 1;
                    upperIndex = i;
                    lower = _localTransfomrStampBuffer[lowerIndex];
                    upper = _localTransfomrStampBuffer[upperIndex];
                    break;
                }
            }
            if(!lower.HasValue || !upper.HasValue){
                Debug.LogWarning("[SyncTransfrom(prediction correction)] local buffer dont have entry for server's incoming transfrom stamp");
                return;
            }

            // remove old entry of local buffer
            if(_localTransfomrStampBuffer.Count > 0 && lowerIndex > 1) _localTransfomrStampBuffer.RemoveRange(0, lowerIndex - 1);

            // get correct position & rotation
            Vector3 correctPosition, correctRotation;
            correctPosition = newTransformStamp.Position;
            correctRotation = newTransformStamp.Rotation;
            CorrectPosition = correctPosition;
            CorrectRotation = correctRotation;

            // find find prediction off vector of position & rotation
            Vector3 predictPosition, predictRotation;
            float t = (float)((newTransformStamp.Stamp - lower.Value.Stamp) / (upper.Value.Stamp - lower.Value.Stamp));
            
            predictPosition = Vector3.Lerp(lower.Value.Position, upper.Value.Position, t);
            predictRotation = Vector3.Lerp(lower.Value.Rotation, upper.Value.Rotation, t);
            PredictPosition = predictPosition;
            PredictRotation = predictRotation;

            // get fix delta position & rotation
            Vector3 fixDeltaPosition, fixDeltaRotation;
            fixDeltaPosition = (correctPosition == predictPosition)? Vector3.zero: (correctPosition - predictPosition);
            fixDeltaRotation = (correctRotation == predictRotation)? Vector3.zero: ((Quaternion.Euler(correctRotation) * Quaternion.Inverse(Quaternion.Euler(predictRotation))).eulerAngles);
            FixDeltaPosition = fixDeltaPosition;
            FixDeltaRotation = fixDeltaRotation; 


            // Get target position & rotation
            _targetPosition = transform.position + FixDeltaPosition;
            _targetRotation = transform.rotation.eulerAngles + fixDeltaRotation;
            _lastFixDeltaPosition = fixDeltaPosition;
            _lastFixDeltaRotation = fixDeltaRotation;

            CorrectingTransfromClient();
        }


        private void OnTickClient()
        {   
            // Debug
            SetNetworkBufferPastFuterEntryiesCnt(NetworkManager.NetworkTickSystem.ServerTime.Time);
            // buffer local transform if is in prediction correctio mode
            if(Mode == ModeEnum.PredictionCorrection){
                LocalBufferAdd(new TransformStamp(
                    NetworkManager.Singleton.NetworkTickSystem.LocalTime.Time,
                    transform.position,
                    transform.rotation.eulerAngles
                ));
            }
        }

        
        private void OnGetNewTransformStampClient(TransformStamp previousValue, TransformStamp newValue)
        {
            // when prediction correction
            if(Mode == ModeEnum.PredictionCorrection){
                GetPredictionCorrectionTargetValueClient(newValue);
            }
        }


        #endregion


        #region Server
        private void OnTickServer()
        {
            // add current transform to buffer with stamp
            NetworkTransformStampBufferAddServer(new TransformStamp(
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
