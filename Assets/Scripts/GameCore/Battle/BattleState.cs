﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Block;
using Cysharp.Threading.Tasks;
using Data;
using Manager.DataManager;
using Photon;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleState : State
    {
        private BattleView _battleView;
        private CommonView _commonView;
        private PhotonManager _photonManager;
        private BlockFactory _blockFactory;
        private BlockDataManager _blockDataManager;
        private CancellationTokenSource _cancellationTokenSource;
        private BlockGameObject _currentBlockObj;
        private GameOverLine[] _gameOverLines;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;
        private StageDataManager _stageDataManager;
        private StageColliderManager _stageColliderManager;
        private Transform _stageParent;
        private const string MyTurnText = "あなたの番";
        private const string EnemyTurnText = "相手の番";
        private float _time;
        private bool _push;


        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        protected override void OnExit(State nextState)
        {
            _stageColliderManager.Cancel();
            Owner._isMyTurn = false;
            _currentBlockObj = null;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.DestroyAll();
            }

            PhotonNetwork.LeaveRoom();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }

            Cancel();
        }

        protected override void OnUpdate()

        {
            if (_currentBlockObj == null)
            {
                return;
            }

            if (_push)
            {
                _time = Time.deltaTime;
                SuccessiveRotate(_time);
            }

            if (Input.GetMouseButton(0) && Owner._isMyTurn)
            {
                if (_push)
                {
                    return;
                }

                var mousePos = Input.mousePosition;
                _currentBlockObj.BlockStateReactiveProperty.Value = BlockSate.Operating;
                if (Camera.main != null)
                {
                    var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
                    var transform1 = _currentBlockObj.transform;
                    var position = transform1.position;
                    position = new Vector3(worldPos.x, position.y, position.z);
                    transform1.position = position;
                }
            }

            if (Input.GetMouseButtonUp(0) && Owner._isMyTurn)
            {
                OnPointerUp(_currentBlockObj).Forget();
            }
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _battleView = Owner.battleView;
            _commonView = Owner.commonView;
            _photonManager = Owner.photonManager;
            _blockDataManager = Owner.blockDataManager;
            _blockFactory = Owner.blockFactory;
            _gameOverLines = Owner.gameOverLines;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
            _stageDataManager = Owner.stageDataManager;
            _stageParent = Owner.stageParent;
            _stageColliderManager = Owner.stageColliderManager;
            _stageColliderManager.Initialize();
            InitializeButton();
            InitializeSubscribe();
            _battleView.turnText.gameObject.SetActive(true);
            Owner.SwitchUiView((int)Event.Battle);
            if (PhotonNetwork.IsMasterClient)
            {
                GenerateStage();
                Owner._isMyTurn = true;
                _battleView.turnText.text = MyTurnText;
                var index = _blockDataManager.GetRandomBlockData().Id;
                PhotonNetwork.CurrentRoom.SetBlockIndex(index);
            }
            else
            {
                _battleView.turnText.text = EnemyTurnText;
            }
        }

        private void InitializeButton()
        {
            EventTrigger trigger = _battleView.rotateButton.GetComponent<EventTrigger>();
            trigger.triggers.RemoveRange(0, trigger.triggers.Count);
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entry.callback.AddListener((eventDate) => { OnClickPushDown(); });
            trigger.triggers.Add(entry);
            EventTrigger.Entry entry1 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            entry1.callback.AddListener((eventDate) => { OnClickPushUp(); });
            trigger.triggers.Add(entry1);
        }

        private void InitializeSubscribe()
        {
            _photonManager.EnemyRate.Subscribe(rate => { _userDataManager.SetEnemyRate(rate); })
                .AddTo(_cancellationTokenSource.Token);

            _photonManager.ChangeIndex.Subscribe(index => UniTask.Void(async () =>
            {
                var blocks = GameObject.FindGameObjectsWithTag(GameCommonData.BlockTag).ToList();
                FreezeAllBlocks(blocks);
                Owner._overlapBlockCount = blocks.Count;
                if (!Owner._isMyTurn)
                {
                    _battleView.turnText.text = EnemyTurnText;
                    _battleView.rotateButton.gameObject.SetActive(false);
                    Owner._isMyTurn = !Owner._isMyTurn;
                    return;
                }

                _battleView.turnText.text = MyTurnText;
                _battleView.rotateButton.gameObject.SetActive(true);
                var blockData = _blockDataManager.GetBlockData(index, 3);
                var block = await _blockFactory.GenerateBlock(blockData);
                _currentBlockObj = block.GetComponent<BlockGameObject>();
                _currentBlockObj.BlockStateReactiveProperty.Subscribe(state =>
                {
                    if (state != BlockSate.Stop)
                    {
                        return;
                    }

                    var blockIndex = _blockDataManager.GetRandomBlockData().Id;
                    PhotonNetwork.CurrentRoom.SetBlockIndex(blockIndex);
                }).AddTo(_cancellationTokenSource.Token);
            })).AddTo(_cancellationTokenSource.Token);

            foreach (var gameOverLine in _gameOverLines)
            {
                gameOverLine.GameEnd.Skip(1).Subscribe(value => { PhotonNetwork.CurrentRoom.SetBattleEnd(1); })
                    .AddTo(_cancellationTokenSource.Token);
            }

            _photonManager.BattleEnd.Subscribe(value => { _stateMachine.Dispatch((int)Event.BattleResult); })
                .AddTo(_cancellationTokenSource.Token);

            _photonManager.ForcedTermination.Subscribe(_ => { ForcedTermination(); })
                .AddTo(_cancellationTokenSource.Token);
        }

        private void GenerateStage()
        {
            var stageData = _stageDataManager.GetRandomStageData();

            PhotonNetwork.InstantiateRoomObject(
                GameCommonData.StagePrefabPass + stageData.Stage + "/" + stageData.Name, _stageParent.position,
                _stageParent.rotation);
        }

        private async UniTaskVoid OnPointerUp(BlockGameObject blockSc)
        {
            if (_currentBlockObj == null)
            {
                return;
            }

            if (_currentBlockObj.BlockStateReactiveProperty.Value != BlockSate.Operating)
            {
                return;
            }

            var rigid = blockSc.GetComponent<Rigidbody2D>();
            rigid.gravityScale = GameCommonData.GravityScale;
            _currentBlockObj = null;
            Owner._isMyTurn = !Owner._isMyTurn;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            blockSc.BlockStateReactiveProperty.Value = BlockSate.Moving;
        }

        private void OnClickPushDown()
        {
            if (!Owner._isMyTurn || _currentBlockObj == null)
            {
                return;
            }

            _push = true;
            SoundManager.Instance.DecideSe();
            _currentBlockObj.BlockStateReactiveProperty.Value = BlockSate.Rotating;
            var transform1 = _currentBlockObj.transform;
            transform1.localPosition = new Vector3(0, transform1.localPosition.y, 0);
        }

        private void OnClickPushUp()
        {
            if (!Owner._isMyTurn || _currentBlockObj == null)
            {
                return;
            }

            _push = false;
            var transform1 = _currentBlockObj.transform;
            transform1.localPosition = new Vector3(0, transform1.localPosition.y, 0);
        }

        private void SuccessiveRotate(float time)
        {
            if (!Owner._isMyTurn || _currentBlockObj == null)
            {
                return;
            }

            _currentBlockObj.transform.localEulerAngles -= new Vector3(0f, 0f, time * GameCommonData.RotationSpeed);
        }

        private void ForcedTermination()
        {
            //     _battleEndCount = 0;
            _currentBlockObj = null;
            //     PhotonNetwork.LeaveRoom();
            _commonView.disconnectionView.disconnectionObj.SetActive(true);
        }

        private void DestroyAllBlock()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            var blocks = GameObject.FindGameObjectsWithTag(GameCommonData.BlockTag);
            foreach (var block in blocks)
            {
                if (block == null)
                {
                    continue;
                }

                PhotonNetwork.Destroy(block);
            }
        }

        private void FreezeAllBlocks(List<GameObject> blocks)
        {
            foreach (var block in blocks)
            {
                var rigid = block.GetComponent<Rigidbody2D>();
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        private void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}