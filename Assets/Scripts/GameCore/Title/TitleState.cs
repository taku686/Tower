using Cysharp.Threading.Tasks;
using Data;
using DefaultNamespace;
using Manager.DataManager;
using PlayFab;
using UnityEngine;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class TitleState : State
    {
        private PlayFabLoginManager _playFabLoginManager;
        private TitleView _titleView;
        private CommonView _commonView;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;
        private IconDataManager _iconDataManager;
        private bool _isNameNullOrEmpty;

        protected override void OnEnter(State prevState)
        {
            Initialize().Forget();
        }

        protected override void OnUpdate()
        {
            if (_isNameNullOrEmpty)
            {
                _isNameNullOrEmpty = false;
                _stateMachine.Dispatch((int)Event.NameChange);
            }
        }

        private async UniTask Initialize()
        {
            _playFabLoginManager = Owner.playFabLoginManager;
            _titleView = Owner.titleView;
            _commonView = Owner.commonView;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
            _iconDataManager = Owner.iconDataManager;
            _isNameNullOrEmpty = false;
            Owner.blockFactory.ResetBlockParent();
            if (Camera.main != null) Camera.main.transform.localPosition = new Vector3(0, 0, -10);
            await Login();
            InitializeButton();
            SetUpUiContents();
            Owner.SwitchUiView((int)Event.Title);
        }

        private void InitializeButton()
        {
            _titleView.startButton.onClick.RemoveAllListeners();
            _titleView.nameChangeButton.onClick.RemoveAllListeners();
            _titleView.settingButton.onClick.RemoveAllListeners();
            _titleView.startButton.onClick.AddListener(OnClickStart);
            _titleView.nameChangeButton.onClick.AddListener(OnClickNameChange);
            _titleView.settingButton.onClick.AddListener(OnClickSetting);
        }

        private void SetUpUiContents()
        {
            _commonView.loadingObj.SetActive(false);
            var iconIndex = _userDataManager.GetIconIndex();
            _titleView.iconImage.sprite = _iconDataManager.GetIconSprite(iconIndex);
            if (string.IsNullOrEmpty(_userDataManager.GetUserName()))
            {
                //   _stateMachine.Dispatch((int)Event.NameChange);
                return;
            }

            _titleView.nameText.text = _userDataManager.GetUserName();
        }

        private void OnClickStart()
        {
            SoundManager.Instance.DecideSe();
            _stateMachine.Dispatch((int)Event.BattleModeSelect);
        }

        private void OnClickNameChange()
        {
            SoundManager.Instance.DecideSe();
            _stateMachine.Dispatch((int)Event.NameChange);
        }

        private void OnClickSetting()
        {
            _stateMachine.Dispatch((int)Event.Setting);
        }

        private async UniTask<bool> Login()
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                return true;
            }

            var result = await _playFabLoginManager.Login();
            if (!result)
            {
                Debug.LogError("ログイン失敗");
                return false;
            }

            var userName = await _userDataManager.GetUserNameAsync();
            if (string.IsNullOrEmpty(userName))
            {
                _isNameNullOrEmpty = true;
                return false;
            }

            _userDataManager.SetUserName(userName);
            Debug.Log("ログイン成功");
            return true;
        }
    }
}