using System;
using System.Collections.Generic;
using Data;
using DefaultNamespace;
using Manager.DataManager;
using Photon;
using Photon.Pun;
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
    [SerializeField] private StageColliderManager stageColliderManager;
    [SerializeField] private TitleView titleView;
    [SerializeField] private BattleModeSelectView battleModeSelectView;
    [SerializeField] private BattleReadyView battleReadyView;
    [SerializeField] private BattleView battleView;
    [SerializeField] private BattleResultView battleResultView;
    [SerializeField] private NameChangeView nameChangeView;
    [SerializeField] private SettingView settingView;
    [SerializeField] private SingleBattleResultView singleBattleResultView;
    [SerializeField] private CommonView commonView;
    [SerializeField] private BlockFactory blockFactory;
    [SerializeField] private BlockParent blockParent;
    [SerializeField] private Transform blockGeneratePoint;
    [SerializeField] private GameOverLine[] gameOverLines;
    [SerializeField] private List<GameObject> uiObjects = new();
    [SerializeField] private GameObject advertisementObj;
    [SerializeField] private Transform stageParent;
    private bool _isOnLine;
    private bool _isMyTurn;
    private int _overlapBlockCount;
    private GameObject _stageObj;
    private Event _currentState;

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
        Application.targetFrameRate = GameCommonData.FPS;
        SwitchUiView((int)(Event.Title));
        Initialize();
        InitializeButton();
        InitializeState();
    }

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

    private void InitializeButton()
    {
        commonView.disconnectionView.okButton.onClick.AddListener(OnClickBackToTitle);
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
        _currentState = (Event)index;
        foreach (var obj in uiObjects)
        {
            obj.SetActive(false);
        }

        uiObjects[index].SetActive(true);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }

            if (_currentState != Event.Battle || _currentState != Event.BattleSingle)
            {
                return;
            }

            commonView.disconnectionView.disconnectionObj.SetActive(true);
        }
        else
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }

            if (_currentState != Event.Battle)
            {
                return;
            }

            _stateMachine.Dispatch((int)Event.Title);
        }
    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        _stateMachine.Dispatch((int)Event.Title);
    }

    private void OnClickBackToTitle()
    {
        commonView.disconnectionView.disconnectionObj.SetActive(false);
        _stateMachine.Dispatch((int)Event.Title);
    }
}