using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleResultState : State
    {
        private GameOverLine _gameOverLine;
        private CancellationTokenSource _cancellationTokenSource;
        private BattleResultView _battleResultView;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;
        private AdMobManager _adMobManager;
        private static readonly Color WinColor = Color.red;
        private static readonly Color LoseColor = Color.blue;
        private const string WinText = "Win";
        private const string LoseText = "Lose";

        protected override void OnEnter(State prevState)
        {
            Initialize().Forget();
        }

        protected override void OnExit(State nextState)
        {
            Owner.advertisementObj.SetActive(false);
            _adMobManager.HideBanner();
            Owner._overlapBlockCount = 0;
            Owner._isMyTurn = false;
        }

        private async UniTaskVoid Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _gameOverLine = Owner.gameOverLine;
            _battleResultView = Owner.battleResultView;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
            _adMobManager = Owner.adMobManager;
            Destroy(Owner._stageObj);
            InitializeButton();
            await SetResultData();
            SetUpUiContent();
            Owner.SwitchUiView((int)Event.BattleResult);
            Owner.advertisementObj.SetActive(true);
            _adMobManager.ShowBanner();
        }

        private void InitializeButton()
        {
            _battleResultView.backButton.onClick.RemoveAllListeners();
            _battleResultView.retryButton.onClick.RemoveAllListeners();
            _battleResultView.backButton.onClick.AddListener(OnClickBack);
            _battleResultView.retryButton.onClick.AddListener(OnClickRetry);
        }

        private void SetUpUiContent()
        {
            _battleResultView.resultText.color = Owner._isMyTurn ? WinColor : LoseColor;
            _battleResultView.resultText.text = Owner._isMyTurn ? WinText : LoseText;
            _battleResultView.rateText.text = _userDataManager.GetRate().ToString();
            var addRate = _userDataManager.CalculateAddRate(Owner._isMyTurn, Owner._overlapBlockCount);
            _battleResultView.addRateText.text = Owner._isMyTurn ? "(+" + addRate + ")" : "(" + addRate + ")";
            _battleResultView.winLoseText.text = _userDataManager.GetWinCount() + "勝" +
                                                 _userDataManager.GetLoseCount() + "敗";
            _battleResultView.currentContinuityWinCountText.text =
                "(現在" + _userDataManager.GetCurrentContinuityWinCount() + "連勝中）";
            _battleResultView.maxContinuityWinCountText.text = _userDataManager.GetMaxContinuityWinCount() + "連勝";
        }

        private async UniTask SetResultData()
        {
            var isWin = Owner._isMyTurn;
            if (isWin)
            {
                _userDataManager.SetWinCount();
                _userDataManager.SetCurrentContinuityWinCount(true);
                _userDataManager.SetMaxContinuityWinCount();
                _userDataManager.SetRate(true, Owner._overlapBlockCount);
            }
            else
            {
                _userDataManager.SetLoseCount();
                _userDataManager.SetCurrentContinuityWinCount(false);
                _userDataManager.SetRate(false, Owner._overlapBlockCount);
            }

            await _userDataManager.UpdateUserData();
        }

        private void OnClickBack()
        {
            SoundManager.Instance.DecideSe();
            _stateMachine.Dispatch((int)Event.Title);
        }

        private void OnClickRetry()
        {
            SoundManager.Instance.DecideSe();
            Owner._isOnLine = true;
            _stateMachine.Dispatch((int)Event.BattleReady);
        }
    }
}