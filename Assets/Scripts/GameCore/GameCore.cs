using System.Collections.Generic;
using DefaultNamespace;
using Manager.DataManager;
using Photon;
using UnityEngine;

public partial class GameCore : MonoBehaviour
{
    private StateMachine<GameCore> _stateMachine;

    [SerializeField] private PlayFabLoginManager playFabLoginManager;
    [SerializeField] private PlayFabTitleDataManager playFabTitleDataManager;
    [SerializeField] private BlockDataManager blockDataManager;
    [SerializeField] private PhotonManager photonManager;
    [SerializeField] private TitleView titleView;
    [SerializeField] private BattleModeSelectView battleModeSelectView;
    [SerializeField] private BattleReadyView battleReadyView;
    [SerializeField] private BattleView battleView;
    [SerializeField] private BattleResultView battleResultView;
    [SerializeField] private BlockFactory blockFactory;
    [SerializeField] private GameOverLine gameOverLine;
    [SerializeField] private List<GameObject> uiObjects = new();
    private bool _isOnLine;

    private enum Event
    {
        Title,
        BattleModeSelect,
        BattleReady,
        Battle,
        BattleResult
    }

    private void Start()
    {
        InitializeState();
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void InitializeState()
    {
        _stateMachine = new StateMachine<GameCore>(this);
        _stateMachine.Start<TitleState>();
        _stateMachine.AddTransition<TitleState, BattleModeSelectState>((int)Event.BattleModeSelect);
        _stateMachine.AddTransition<BattleReadyState, BattleModeSelectState>((int)Event.BattleModeSelect);
        _stateMachine.AddTransition<BattleModeSelectState, BattleReadyState>((int)Event.BattleReady);
        _stateMachine.AddTransition<BattleReadyState, BattleState>((int)Event.Battle);
        _stateMachine.AddTransition<BattleState, BattleResultState>((int)Event.BattleResult);
        _stateMachine.AddTransition<BattleResultState, TitleState>((int)Event.Title);
    }

    private void SwitchUiView(int index)
    {
        foreach (var obj in uiObjects)
        {
            obj.SetActive(false);
        }

        uiObjects[index].SetActive(true);
    }
}