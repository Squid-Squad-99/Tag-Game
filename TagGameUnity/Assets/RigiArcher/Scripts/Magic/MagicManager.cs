using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using RigiArcher;
using RigiArcher.MeshSocket;
using System.Threading.Tasks;

using Tag.Game.Character;

namespace RigiArcher.Magic{

    /// <summary>
    /// we use left hand to cast a magic, ext. rope
    /// provide Equip Method to equip magic to character
    /// 1. basic casting animation
    /// </summary>
    public class MagicManager : NetworkBehaviour
    {
        public enum MagicIdEnum{
            None,
            Rope,
        }

        public enum NetActionId{
            CastMagic,
            CancelMagic,
        }

        [Header("Setting")]
        [SerializeField] MagicIdEnum _InitEquippedMagicId;
        [SerializeField] float _AnimLayerSmoothTime;
        public float MaxMana;
        public float ManaRegenerateSpeed;
        
        [Header("Data")]
        [SerializeField] MagicIdEnum _currentEquippedMagicId;
        public MagicBase CurrentEquipedMagic => _magicMap[_currentEquippedMagicId];

        // data
        private Dictionary<MagicIdEnum, MagicBase> _magicMap = new Dictionary<MagicIdEnum, MagicBase>();
        public NetworkVariable<float> CurrentMana;

        // reference
        private MeshSocketManager _meshSocketManager;
        private CharacterObject _characterObject;
        private RigiArcherObject _rigiArcherObject;
        private CharacterGameState _characterGameState;
        private Animator _animator;
        private int _leftArmAnimLayerIndex;
        private int _animParamCastMagicId;



        private void Awake() {
            // reference
            _meshSocketManager = GetComponent<MeshSocketManager>();
            _animator = GetComponent<Animator>();
            _rigiArcherObject = GetComponent<RigiArcherObject>();
            _characterObject = GetComponent<CharacterObject>();
            _characterGameState = GetComponent<CharacterGameState>();
            _leftArmAnimLayerIndex = _animator.GetLayerIndex("LeftArm");
            _animParamCastMagicId = Animator.StringToHash("CastMagic");

            // populate magic map
            MagicBase[] magics = GetComponentsInChildren<MagicBase>(true);
            foreach(MagicBase magic in magics){
                _magicMap.Add(magic.MagicId, magic);
            }

        }

        private void Start() {
            // set to init equipped maigic
            Equip(_InitEquippedMagicId);

            if(IsServer){
                GameManager.Singleton.StartGameEvent += () => StartCoroutine("ManaCountDown");
                GameManager.Singleton.GameEndEvent += (reason) => StopCoroutine("ManaCountDown");
                
                _characterGameState.DieEvent += () => StopCoroutine("ManaCountDown");
                _characterGameState.RebornEvent += () => StartCoroutine("ManaCountDown"); 
            }
        }

        private IEnumerator ManaCountDown(){
            CurrentMana.Value = 0;
            while(true){
                yield return null;
                if(CurrentMana.Value < MaxMana){
                    CurrentMana.Value += Time.deltaTime * ManaRegenerateSpeed;
                }
            }
        }

        public override void OnNetworkSpawn(){
            // init data
            if(IsServer){
                CurrentMana.Value = 0;
            }
        }
        
        public void Equip(MagicIdEnum magicId){
            // check is None
            if(magicId == MagicIdEnum.None) Debug.LogWarning("can't equip none type magic");

            // set to current magic
            _currentEquippedMagicId = magicId;

            //equip magic
            CurrentEquipedMagic.Equip(_rigiArcherObject);

            // hold to left hand
            _meshSocketManager.Attach(CurrentEquipedMagic.transform, MeshSocketManager.SocketIdEnum.LeftHand);
        }

        public void CastMagic(){
            // check have equip 
            if(_currentEquippedMagicId == MagicIdEnum.None) Debug.LogWarning("[Magic Manger] no magic equip");

            if(CanCastMagic())
            {
                CastMaigcNoneCheck();
                if(IsServer) NetActionClientRpc(NetActionId.CastMagic, NetworkManager.Singleton.ServerTime.Time);
            }
            else
            {
                Debug.LogError("[MagicManager] can't cast magic, (not cool down yet");
            }

        }  

        private void CastMaigcNoneCheck(){
            // hook magic's events
            CurrentEquipedMagic.CastMagicFinish.AddListener(OnCastMagicFinish);
            CurrentEquipedMagic.SpellFinish.AddListener(OnSpellMagicFinish);
            // play cast magic animation for character
            PlayCastMagicAnimation();
            // cast magic
            CurrentEquipedMagic.CastMagic();
            // mana use
            if(IsServer) CurrentMana.Value -= CurrentEquipedMagic.ManaCost;
        }

        public bool CanCastMagic(){
            return CurrentEquipedMagic.CanCastMagic() && CurrentMana.Value >= CurrentEquipedMagic.ManaCost;
        }

        public void     CancelMagic(){
            CurrentEquipedMagic.CancelMagic();
            if(IsServer) NetActionClientRpc(NetActionId.CancelMagic, NetworkManager.Singleton.ServerTime.Time);
        } 

        [ClientRpc]
        private async void NetActionClientRpc(NetActionId netAction, double serverTime){
            if(_characterObject.OwnedByLocalUser == true) return;
            double waitTime = serverTime - NetworkManager.ServerTime.Time;
            if(waitTime > 0) await Task.Delay((int)(waitTime * 1000));

            switch (netAction)
            {
                case NetActionId.CastMagic:
                    CastMaigcNoneCheck();
                    break;
                case NetActionId.CancelMagic:
                    CancelMagic();
                    break;
            }
            
        }

        private void PlayCastMagicAnimation(){
            // play cast magic animation
            _animator.SetBool(_animParamCastMagicId, true);
            // enable anim left hand layer active
            SetLeftHandLayerActive(true);
        }

        private void OnCastMagicFinish(){
            // set anim param 
            _animator.SetBool(_animParamCastMagicId, false);
        }

        private void OnSpellMagicFinish()
        {
            // set anim
            SetLeftHandLayerActive(false);
        }

        private bool _haveSetLayerRoutine = false;
        private void SetLeftHandLayerActive(bool enable)
        {
            if (_haveSetLayerRoutine)
            {
                StopCoroutine("SetLeftHandLayerActiveRoutine");
            }

            _haveSetLayerRoutine = true;
            StartCoroutine("SetLeftHandLayerActiveRoutine", enable);
        }

        private float _layerChangeVel, _targetValue;
        private IEnumerator SetLeftHandLayerActiveRoutine(bool enable){
            float currentLayerWeight = _animator.GetLayerWeight(_leftArmAnimLayerIndex);
            float _targetValue = enable ? 1 : 0;
            while(true){
                float weight = Mathf.SmoothDamp(currentLayerWeight, _targetValue, ref _layerChangeVel, _AnimLayerSmoothTime);
                _animator.SetLayerWeight(_leftArmAnimLayerIndex, weight);
                currentLayerWeight = weight;
                if(Mathf.Abs(_targetValue - weight) <= 0.01) break;
                yield return null;
            }
            _haveSetLayerRoutine = false;
        }
    }

}
