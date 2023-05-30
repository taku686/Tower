using System;
using System.Linq;
using System.Threading;
using Block;
using Cysharp.Threading.Tasks;
using Data;
using Manager.DataManager;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleSingleState : State
    {
        private BattleView _battleView;
        private BlockFactory _blockFactory;
        private BlockDataManager _blockDataManager;
        private StageColliderManager _stageColliderManager;
        private CancellationTokenSource _cancellationTokenSource;
        private BlockGameObject _currentBlockObj;
        private GameOverLine[] _gameOverLines;
        private StateMachine<GameCore> _stateMachine;
        private float _time;
        private bool _push;
        private readonly Subject<int> _nextBlock = new();
        private int _blockCount;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        protected override void OnExit(State nextState)
        {
            _currentBlockObj = null;
            Destroy(Owner._stageObj);
            DestroyAllBlock();
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

            if (Input.GetMouseButton(0))
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

            if (Input.GetMouseButtonUp(0))
            {
                OnPointerUp(_currentBlockObj).Forget();
            }
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _battleView = Owner.battleView;
            _blockDataManager = Owner.blockDataManager;
            _blockFactory = Owner.blockFactory;
            _gameOverLines = Owner.gameOverLines;
            _stateMachine = Owner._stateMachine;
            _stageColliderManager = Owner.stageColliderManager;
            _blockCount = 0;
            _stageColliderManager.Initialize();
            InitializeButton();
            InitializeSubscribe();
            _battleView.turnText.gameObject.SetActive(false);
            _battleView.rotateButton.gameObject.SetActive(true);
            Owner.SwitchUiView((int)Event.Battle);
            Owner._currentState = Event.BattleSingle;
            var index = _blockDataManager.GetRandomBlockData().Id;
            _nextBlock.OnNext(index);
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
            _nextBlock.Subscribe(index => UniTask.Void(async () =>
            {
                _blockCount = 0;
                var blockData = _blockDataManager.GetBlockData(index, 3);
                var block = await _blockFactory.GenerateBlock(blockData);
                _currentBlockObj = block.GetComponent<BlockGameObject>();
                var blocks = GameObject.FindGameObjectsWithTag(GameCommonData.BlockTag).ToList();
                foreach (var blockObj in blocks)
                {
                    var blockGameObjectSc = blockObj.GetComponent<BlockGameObject>();
                    blockGameObjectSc.BlockStateReactiveProperty.Subscribe(state =>
                    {
                        if (state != BlockSate.Stop)
                        {
                            return;
                        }

                        _blockCount++;
                        if (_blockCount == blocks.Count)
                        {
                            Owner._overlapBlockCount = blocks.Count;
                            var blockIndex = _blockDataManager.GetRandomBlockData().Id;
                            _nextBlock.OnNext(blockIndex);
                        }
                    }).AddTo(_cancellationTokenSource.Token);
                }
            })).AddTo(_cancellationTokenSource.Token);

            foreach (var gameOverLine in _gameOverLines)
            {
                gameOverLine.GameEnd.Skip(1).Subscribe(value =>
                {
                    DestroyAllBlock();
                    _stateMachine.Dispatch((int)Event.SingleBattleResult);
                }).AddTo(_cancellationTokenSource.Token);
            }
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
            if (_currentBlockObj == null)
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
            if (_currentBlockObj == null)
            {
                return;
            }

            _push = false;
            var transform1 = _currentBlockObj.transform;
            transform1.localPosition = new Vector3(0, transform1.localPosition.y, 0);
        }

        private void SuccessiveRotate(float time)
        {
            if (_currentBlockObj == null)
            {
                return;
            }

            _currentBlockObj.transform.localEulerAngles -= new Vector3(0f, 0f, time * GameCommonData.RotationSpeed);
        }

        private void DestroyAllBlock()
        {
            var blocks = GameObject.FindGameObjectsWithTag(GameCommonData.BlockTag);
            foreach (var block in blocks)
            {
                Destroy(block);
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