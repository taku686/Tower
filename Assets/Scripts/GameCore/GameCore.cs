using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public partial class GameCore : MonoBehaviour
{
    private StateMachine<GameCore> _stateMachine;

    [SerializeField] private PlayFabLoginManager playFabLoginManager;
    [SerializeField] private TitleView titleView;
    [SerializeField] private BattleModeSelectView battleModeSelectView;
    [SerializeField] private BattleReadyView battleReadyView;
    [SerializeField] private BattleView battleView;
    [SerializeField] private BattleResultView battleResultView;
<<<<<<< Updated upstream
=======
    [SerializeField] private NameChangeView nameChangeView;
    [SerializeField] private SettingView settingView;
    [SerializeField] private SingleBattleResultView singleBattleResultView;
    [SerializeField] private CommonView commonView;
    [SerializeField] private BlockFactory blockFactory;
    [SerializeField] private GameOverLine gameOverLine;
>>>>>>> Stashed changes
    [SerializeField] private List<GameObject> uiObjects = new();

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

<<<<<<< Updated upstream
=======
    private void Update()
    {
        _stateMachine.Update();
    }

    private void Initialize()
    {
        SoundManager.Instance.BgmPlay();
        commonView.loadingObj.SetActive(true);
        advertisementObj.SetActive(false);
        photonManager.Initialize(userDataManager);
        userDataManager.Initialize(playFabUserDataManager);
        playFabTitleDataManager.Initialize(blockDataManager, stageDataManager, iconDataManager, ngWordDataManager);
        playFabLoginManager.Initialize(playFabTitleDataManager, userDataManager);
        titleView.Initialize();
    }

>>>>>>> Stashed changes
    private void InitializeState()
    {
        _stateMachine = new StateMachine<GameCore>(this);
        _stateMachine.Start<TitleState>();
        _stateMachine.AddTransition<TitleState, BattleModeSelectState>((int)Event.BattleModeSelect);
        _stateMachine.AddTransition<BattleModeSelectState, BattleReadyState>((int)Event.BattleReady);
        _stateMachine.AddTransition<BattleReadyState, BattleState>((int)Event.Battle);
        _stateMachine.AddTransition<BattleState, BattleReadyState>((int)Event.BattleResult);
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