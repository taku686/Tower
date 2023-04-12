using Cysharp.Threading.Tasks;
using DefaultNamespace;
using PlayFab;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class TitleState : State
    {
        private PlayFabLoginManager _playFabLoginManager;
        private TitleView _titleView;
        private StateMachine<GameCore> _stateMachine;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        private void Initialize()
        {
            _playFabLoginManager = Owner.playFabLoginManager;
            _titleView = Owner.titleView;
            _stateMachine = Owner._stateMachine;
            var titleDataManager = Owner.playFabTitleDataManager;
            var blockDataManager = Owner.blockDataManager;
            _playFabLoginManager.Initialize(titleDataManager, blockDataManager);
            InitializeButton();
            Owner.SwitchUiView((int)Event.Title);
        }

        private void InitializeButton()
        {
            _titleView.startButton.onClick.RemoveAllListeners();
            _titleView.startButton.onClick.AddListener(() => UniTask.Void(async () => { await OnClickStart(); }));
        }

        private async UniTask OnClickStart()
        {
            var player = PlayFabSettings.staticPlayer;
            if (player.IsClientLoggedIn())
            {
                return;
            }

            var result = await Login();
            if (!result)
            {
                return;
            }

            _stateMachine.Dispatch((int)Event.BattleModeSelect);
        }

        private async UniTask<bool> Login()
        {
            var result = await _playFabLoginManager.Login();
            if (!result)
            {
                return false;
            }

            return true;
        }
    }
}