using System.Collections.Generic;
using DefaultNamespace;
using Manager.DataManager;
using Photon;
using UnityEngine;
using UnityEngine.Serialization;

public partial class GameCore : MonoBehaviour
{
    private StateMachine<GameCore> _stateMachine;

    [SerializeField] private PlayFabLoginManager playFabLoginManager;
    [SerializeField] private PlayFabTitleDataManager playFabTitleDataManager;
    [SerializeField] private PlayFabUserDataManager playFabUserDataManager;
    [SerializeField] private BlockDataManager blockDataManager;
    [SerializeField] private StageDataManager stageDataManager;
    [SerializeField] private PhotonManager photonManager;
    [SerializeField] private UserDataManager userDataManager;
    [SerializeField] private AdMobManager adMobManager;
    [SerializeField] private IconDataManager iconDataManager;
    [SerializeField] private NgWordDataManager ngWordDataManager;
    [SerializeField] private TitleView titleView;
    [SerializeField] private BattleModeSelectView battleModeSelectView;
    [SerializeField] private BattleReadyView battleReadyView;
    [SerializeField] private BattleView battleView;
    [SerializeField] private BattleResultView battleResultView;
    [SerializeField] private NameChangeView nameChangeView;
    [SerializeField] private SettingView settingView;
    [SerializeField] private SingleBattleResultView singleBattleResultView;
    [SerializeField] private BlockFactory blockFactory;
    [SerializeField] private GameOverLine gameOverLine;
    [SerializeField] private List<GameObject> uiObjects = new();
    [SerializeField] private GameObject advertisementObj;
    [SerializeField] private Transform stageParent;
    private bool _isOnLine;
    private bool _isMyTurn;
    private int _overlapBlockCount;
    private GameObject _stageObj;

//BattleSingleを一番最後に設定する
    private enum Event
    {
        Title,
        BattleModeSelect,
        BattleReady,
        Battle,
        BattleResult,
        NameChange,
        SingleBattleResult,
        Setting,
        BattleSingle,
    }

    private void Start()
    {
        SwitchUiView((int)(Event.Title));
        Initialize();
        InitializeState();
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void Initialize()
    {
        advertisementObj.SetActive(false);
        photonManager.Initialize(userDataManager);
        userDataManager.Initialize(playFabUserDataManager);
        playFabTitleDataManager.Initialize(blockDataManager, stageDataManager, iconDataManager, ngWordDataManager);
        playFabLoginManager.Initialize(playFabTitleDataManager, userDataManager);
        titleView.Initialize();
    }

    private void InitializeState()
    {
        _stateMachine = new StateMachine<GameCore>(this);
        _stateMachine.Start<TitleState>();
        _stateMachine.AddAnyTransition<TitleState>((int)Event.Title);
        _stateMachine.AddTransition<TitleState, BattleModeSelectState>((int)Event.BattleModeSelect);
        _stateMachine.AddTransition<BattleReadyState, BattleModeSelectState>((int)Event.BattleModeSelect);
        _stateMachine.AddTransition<BattleModeSelectState, BattleReadyState>((int)Event.BattleReady);
        _stateMachine.AddTransition<SingleBattleResultState, BattleReadyState>((int)Event.BattleReady);
        _stateMachine.AddTransition<BattleResultState, BattleReadyState>((int)Event.BattleReady);
        _stateMachine.AddTransition<BattleReadyState, BattleState>((int)Event.Battle);
        _stateMachine.AddTransition<BattleReadyState, BattleSingleState>((int)Event.BattleSingle);
        _stateMachine.AddTransition<BattleState, BattleResultState>((int)Event.BattleResult);
        _stateMachine.AddTransition<BattleSingleState, SingleBattleResultState>((int)Event.SingleBattleResult);
        _stateMachine.AddTransition<TitleState, NameChangeState>((int)Event.NameChange);
        _stateMachine.AddTransition<TitleState, SettingState>((int)Event.Setting);
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