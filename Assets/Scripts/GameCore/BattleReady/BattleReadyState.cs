using Photon;
using Photon.Pun;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleReadyState : State
    {
        private BattleReadyView _battleReadyView;
        private PhotonManager _photonManager;
        private StateMachine<GameCore> _stateMachine;
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
                _stateMachine.Dispatch((int)Event.Battle);
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

        private void OnClickBack()
        {
            SoundManager.Instance.CancelSe();
            _photonManager.OnLeftRoom();
            _stateMachine.Dispatch((int)Event.BattleModeSelect);
        }
    }
}