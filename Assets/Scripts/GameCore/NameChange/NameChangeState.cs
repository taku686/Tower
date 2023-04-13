using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class NameChangeState : State
    {
        private NameChangeView _nameChangeView;

        private StateMachine<GameCore> _stateMachine;

        protected override void OnEnter(State prevState)
        {
            Initialize();
            InitializeButton();
            Owner.SwitchUiView((int)Event.NameChange);
        }

        private void Initialize()
        {
            _nameChangeView = Owner.nameChangeView;
        }

        private void InitializeButton()
        {
            _nameChangeView.closeButton.onClick.RemoveAllListeners();
            _nameChangeView.closeButton.onClick.AddListener(OnClickCloseView);
        }

        private void OnClickCloseView()
        {
            _stateMachine.Dispatch((int)Event.Title);
        }
    }
}