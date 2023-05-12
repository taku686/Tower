using Cysharp.Threading.Tasks;
using Data;
using DefaultNamespace;
using PlayFab;
using UnityEditor;
using UnityEngine;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class TitleState : State
    {
        private PlayFabLoginManager _playFabLoginManager;
        private TitleView _titleView;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;
        private bool _isNameNullOrEmpty;

        protected override void OnEnter(State prevState)
        {
            Initialize().Forget();
        }

        protected override void OnUpdate()
        {
            if (_isNameNullOrEmpty)
            {
                _stateMachine.Dispatch((int)Event.NameChange);
            }
        }

        private async UniTask Initialize()
        {
            _playFabLoginManager = Owner.playFabLoginManager;
            _titleView = Owner.titleView;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
            _isNameNullOrEmpty = false;
            Owner.blockFactory.ResetBlockParent();
            Owner.gameOverLine.Setup();
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
            _titleView.startButton.onClick.AddListener(OnClickStart);
            _titleView.nameChangeButton.onClick.AddListener(OnClickNameChange);
        }

        private void SetUpUiContents()
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString(GameCommonData.UserKey)))
            {
                _titleView.nameText.text = "";
                return;
            }

            _titleView.nameText.text = PlayerPrefs.GetString(GameCommonData.UserKey);
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

        private async UniTask<bool> Login()
        {
            var result = await _playFabLoginManager.Login();
            if (!result)
            {
                Debug.Log("ログイン失敗");
                return false;
            }

            var userName = await _userDataManager.GetUserName();
            Debug.Log(userName);
            if (string.IsNullOrEmpty(userName))
            {
                _isNameNullOrEmpty = true;
                return false;
            }

            Debug.Log("ログイン成功");
            return true;
        }
    }
}