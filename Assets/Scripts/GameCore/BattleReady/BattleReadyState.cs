using Data;
using Manager.DataManager;
using Photon;
using Photon.Pun;
using UnityEngine;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleReadyState : State
    {
        private BattleReadyView _battleReadyView;
        private PhotonManager _photonManager;
        private StateMachine<GameCore> _stateMachine;
        private StageDataManager _stageDataManager;
        private BlockParent _blockParent;
        private Transform _stageParent;
        private Transform _blockGeneratePoint;
        private bool _isProcessing;
        private readonly Vector3 _initPos = new(0, 0.82f, 0);
        private readonly Vector3 _blockInitPos = new(0, 3.2f, 0);

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        protected override void OnExit(State nextState)
        {
            _isProcessing = false;
        }

        protected override void OnUpdate()
        {
            if (!Owner._isOnLine && !_isProcessing)
            {
                _isProcessing = true;
                PhotonNetwork.OfflineMode = true;
                Debug.Log("ステージ作成");
                GenerateStage();
                _stateMachine.Dispatch((int)Event.BattleSingle);
            }

            if (Owner._isOnLine && !_isProcessing)
            {
                _isProcessing = true;
                _photonManager.OnStartConnectNetwork();
            }

            TransitionBattleState();
        }

        private void Initialize()
        {
            _battleReadyView = Owner.battleReadyView;
            _stateMachine = Owner._stateMachine;
            _photonManager = Owner.photonManager;
            _stageDataManager = Owner.stageDataManager;
            _stageParent = Owner.stageParent;
            _blockParent = Owner.blockParent;
            _blockGeneratePoint = Owner.blockGeneratePoint;
            if (Camera.main != null)
            {
                Camera.main.transform.localPosition = new Vector3(0, 0, -10);
            }

            _blockParent.transform.position = _initPos;
            _blockGeneratePoint.position = _blockInitPos;
            InitializeButton();
            Owner.SwitchUiView((int)Event.BattleReady);
        }

        private void InitializeButton()
        {
            _battleReadyView.backButton.onClick.RemoveAllListeners();
            _battleReadyView.backButton.onClick.AddListener(OnClickBack);
        }

        private void TransitionBattleState()
        {
            if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                _stateMachine.Dispatch((int)Event.Battle);
            }
        }

        private void GenerateStage()
        {
            if (!Owner._isOnLine)
            {
                var stageObj = _stageDataManager.GetRandomStageData().StageObj;
                Owner._stageObj = Instantiate(stageObj, _stageParent);
                Owner._stageObj.transform.localPosition = Vector3.zero;
            }
        }

        private void OnClickBack()
        {
            SoundManager.Instance.CancelSe();
            _photonManager.OnLeftRoom();
            _stateMachine.Dispatch((int)Event.BattleModeSelect);
        }
    }
}