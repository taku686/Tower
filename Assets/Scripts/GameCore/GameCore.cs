using System;
using UnityEngine;

public partial class GameCore : MonoBehaviour
{
    private StateMachine<GameCore> _stateMachine;

    private enum Event
    {
        Title,
        BattleReady,
        Battle,
        BattleResult
    }

    private void Start()
    {
        InitializeState();
    }

    private void InitializeState()
    {
        _stateMachine.Start<TitleState>();
        _stateMachine.AddTransition<TitleState, BattleReadyState>((int)Event.BattleReady);
        _stateMachine.AddTransition<BattleReadyState, BattleState>((int)Event.Battle);
        _stateMachine.AddTransition<BattleState, BattleReadyState>((int)Event.BattleResult);
        _stateMachine.AddTransition<BattleResultState, TitleState>((int)Event.Title);
    }
}