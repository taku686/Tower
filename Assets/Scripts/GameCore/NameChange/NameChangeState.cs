using Cysharp.Threading.Tasks;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class NameChangeState : State
    {
        private NameChangeView _nameChangeView;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;

        protected override void OnEnter(State prevState)
        {
            Initialize();
            InitializeButton();
            Owner.SwitchUiView((int)Event.NameChange);
        }

        private void Initialize()
        {
            _nameChangeView = Owner.nameChangeView;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
        }

        private void InitializeButton()
        {
            _nameChangeView.closeButton.onClick.RemoveAllListeners();
            _nameChangeView.closeButton.onClick.AddListener(() =>
                UniTask.Void(async () => { await OnClickCloseView(); }));
        }

        private async UniTask OnClickCloseView()
        {
            SoundManager.Instance.DecideSe();
            var name = _nameChangeView.inputField.text;
            var result = await _userDataManager.SetUserName(name);
            if (!result)
            {
                return;
            }

            _stateMachine.Dispatch((int)Event.Title);
        }
    }
}