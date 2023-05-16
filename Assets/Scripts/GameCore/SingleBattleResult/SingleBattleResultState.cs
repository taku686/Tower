﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class SingleBattleResultState : State
    {
        private GameOverLine _gameOverLine;
        private CancellationTokenSource _cancellationTokenSource;
        private SingleBattleResultView _singleBattleResultView;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;

        protected override void OnEnter(State prevState)
        {
            Initialize().Forget();
        }

        protected override void OnExit(State nextState)
        {
            Owner._isMyTurn = false;
            Owner._overlapBlockCount = 0;
        }

        private async UniTaskVoid Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _gameOverLine = Owner.gameOverLine;
            _singleBattleResultView = Owner.singleBattleResultView;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
            Destroy(Owner._stageObj);
            InitializeButton();
            await SetResultData();
            SetUpUiContent();
            Owner.SwitchUiView((int)Event.SingleBattleResult);
        }

        private void InitializeButton()
        {
            _singleBattleResultView.backButton.onClick.RemoveAllListeners();
            _singleBattleResultView.backButton.onClick.AddListener(OnClickBack);
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
    }
}