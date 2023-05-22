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
        private Transform _stageParent;
        private bool _isProcessing;

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
            if (!Owner._isOnLine)
            {
                PhotonNetwork.OfflineMode = true;
                GenerateStage();
                _stateMachine.Dispatch((int)Event.BattleSingle);
            }

            if (Owner._isOnLine && !_isProcessing)
            {
                _isProcessing = true;
                _photonManager.SetStageGenerateCallBack(GenerateStage);
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