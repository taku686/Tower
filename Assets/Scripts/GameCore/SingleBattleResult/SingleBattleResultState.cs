using System.Threading;
using Cysharp.Threading.Tasks;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class SingleBattleResultState : State
    {
        private SingleBattleResultView _singleBattleResultView;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;
        private AdMobManager _adMobManager;

        protected override void OnEnter(State prevState)
        {
            Initialize().Forget();
        }

        protected override void OnExit(State nextState)
        {
            Owner.advertisementObj.SetActive(false);
            _adMobManager.HideBanner();
            Owner._overlapBlockCount = 0;
        }

        private async UniTaskVoid Initialize()
        {
            _singleBattleResultView = Owner.singleBattleResultView;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
            _adMobManager = Owner.adMobManager;
            InitializeButton();
            await SetResultData();
            SetUpUiContent();
            Owner.SwitchUiView((int)Event.SingleBattleResult);
            Owner.advertisementObj.SetActive(true);
            _adMobManager.ShowBanner();
        }

        private void InitializeButton()
        {
            _singleBattleResultView.backButton.onClick.RemoveAllListeners();
            _singleBattleResultView.retryButton.onClick.RemoveAllListeners();
            _singleBattleResultView.backButton.onClick.AddListener(OnClickBack);
            _singleBattleResultView.retryButton.onClick.AddListener(OnClickRetry);
        }

        private void SetUpUiContent()
        {
            _singleBattleResultView.winLoseText.text = Owner._overlapBlockCount + "個";
            _singleBattleResultView.maxContinuityWinCountText.text = _userDataManager.GetBlockCount() + "個";
        }

        private async UniTask SetResultData()
        {
            _userDataManager.SetBlockCount(Owner._overlapBlockCount);
            await _userDataManager.UpdateUserData();
        }

        private void OnClickBack()
        {
            SoundManager.Instance.DecideSe();
            _stateMachine.Dispatch((int)Event.Title);
        }

        private void OnClickRetry()
        {
            SoundManager.Instance.GameStartSe();
            Owner._isOnLine = false;
            _stateMachine.Dispatch((int)Event.BattleReady);
        }
    }
}