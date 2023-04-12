using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleModeSelectState : State
    {
        private StateMachine<GameCore> _stateMachine;
        private BattleModeSelectView _battleModeSelectView;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        private void Initialize()
        {
            _stateMachine = Owner._stateMachine;
            _battleModeSelectView = Owner.battleModeSelectView;
            InitializeButton();
            Owner.SwitchUiView((int)Event.BattleModeSelect);
        }

        private void InitializeButton()
        {
            _battleModeSelectView.singleModeButton.onClick.RemoveAllListeners();
            _battleModeSelectView.multiModeButton.onClick.RemoveAllListeners();
            _battleModeSelectView.singleModeButton.onClick.AddListener(OnClickSingleMode);
            _battleModeSelectView.multiModeButton.onClick.AddListener(OnClickMultiMode);
        }

        private void OnClickSingleMode()
        {
            Owner._isOnLine = false;
            _stateMachine.Dispatch((int)Event.BattleReady);
        }

        private void OnClickMultiMode()
        {
            Owner._isOnLine = true;
            _stateMachine.Dispatch((int)Event.BattleReady);
        }
    }
}