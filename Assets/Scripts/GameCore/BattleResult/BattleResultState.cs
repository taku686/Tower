using System.Threading;
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
        private static readonly Color WinColor = Color.red;
        private static readonly Color LoseColor = Color.green;
        private const string WinText = "Win";
        private const string LoseText = "Lose";

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        protected override void OnExit(State nextState)
        {
            Owner._isMyTurn = false;
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _gameOverLine = Owner.gameOverLine;
            _battleResultView = Owner.battleResultView;
            _stateMachine = Owner._stateMachine;
            InitializeButton();
            SetUpUiContent();
            Owner.SwitchUiView((int)Event.BattleResult);
        }

        private void InitializeButton()
        {
            _battleResultView.backButton.onClick.RemoveAllListeners();
            _battleResultView.backButton.onClick.AddListener(OnClickBack);
        }

        private void SetUpUiContent()
        {
            _battleResultView.resultImage.color = !Owner._isMyTurn ? WinColor : LoseColor;
            _battleResultView.resultText.text = !Owner._isMyTurn ? WinText : LoseText;
        }

        private void OnClickBack()
        {
            _stateMachine.Dispatch((int)Event.Title);
        }
    }
}