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
        private CommonView _commonView;
        private StateMachine<GameCore> _stateMachine;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        private void Initialize()
        {
            _playFabLoginManager = Owner.playFabLoginManager;
            _titleView = Owner.titleView;
            _commonView = Owner.commonView;
            _stateMachine = Owner._stateMachine;
            _playFabLoginManager.Initialize();
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
<<<<<<< Updated upstream
            var player = PlayFabSettings.staticPlayer;
            if (player.IsClientLoggedIn())
=======
            _commonView.loadingObj.SetActive(false);
            var iconIndex = _userDataManager.GetIconIndex();
            _titleView.iconImage.sprite = _iconDataManager.GetIconSprite(iconIndex);

            if (string.IsNullOrEmpty(PlayerPrefs.GetString(GameCommonData.UserKey)))
>>>>>>> Stashed changes
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