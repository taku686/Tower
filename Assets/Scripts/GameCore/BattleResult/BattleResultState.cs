using System.Threading;
using UnityEngine;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleResultState : State
    {
        private GameOverLine _gameOverLine;
        private CancellationTokenSource _cancellationTokenSource;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _gameOverLine = Owner.gameOverLine;
            var message = _gameOverLine.GameEnd.Value ? "勝利" : "敗北";
            Debug.Log(message);
        }
    }
}