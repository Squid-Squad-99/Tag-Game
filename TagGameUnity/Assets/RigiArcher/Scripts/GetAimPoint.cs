using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using Tag.Game.Character;

namespace RigiArcher{

    public class GetAimPoint : NetworkBehaviour
    {
        [Header("Setting")]
        [SerializeField] LayerMask _aimLayerMask = new LayerMask();
        [SerializeField] Transform _debugTransform;

        [Header("Data")]
        [SerializeField] Vector3 _aimPoint;
        public Vector3 AimPoint => _aimPoint;

        private CharacterObject _characterObject;

        private void Awake() {
            _characterObject = GetComponent<CharacterObject>();
        }

        private void Update() {
            _debugTransform.position = _aimPoint;
            if(IsServer || _characterObject.OwnedByLocalUser == false) return;

            // get client aim point
            Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPos);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, float.MaxValue,_aimLayerMask)){
                _aimPoint = hit.point;
            }

            SetAimPointServerRpc(_aimPoint, NetworkManager.Singleton.LocalTime.Time);
        }     

        [ServerRpc(RequireOwnership = false)]
        private async void SetAimPointServerRpc(Vector3 aimPoint, double localTime){
            double waitTime = localTime - NetworkManager.ServerTime.Time;
            if(waitTime > 0){
                // wait to sync
                await Task.Delay((int)(waitTime * 1000));
            }

            _aimPoint = aimPoint;

            SetAimPointClientRpc(aimPoint, NetworkManager.ServerTime.Time);
        }

        [ClientRpc]
        private async void SetAimPointClientRpc(Vector3 aimPoint, double serverTime){
            if(_characterObject.OwnedByLocalUser == true) return;
            double waitTime = serverTime - NetworkManager.Singleton.ServerTime.Time;
            if(waitTime > 0){
                // wait to sync
                await Task.Delay((int)(waitTime * 1000));
            }

            _aimPoint = aimPoint;
        }
    }

}
