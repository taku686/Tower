using System;
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
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleState : State
    {
        private BattleView _battleView;
        private PhotonManager _photonManager;
        private BlockFactory _blockFactory;
        private BlockDataManager _blockDataManager;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isMyTurn;
        private BlockGameObject _currentBlockObj;
        private GameOverLine _gameOverLine;
        private StateMachine<GameCore> _stateMachine;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        protected override void OnExit(State nextState)
        {
            Cancel();
        }

        protected override void OnUpdate()
        {
            if (_currentBlockObj == null)
            {
                return;
            }

            if (Input.GetMouseButton(0) && _isMyTurn)
            {
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

            if (Input.GetMouseButtonUp(0) && _isMyTurn)
            {
                OnPointerUp(_currentBlockObj).Forget();
            }
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _battleView = Owner.battleView;
            _photonManager = Owner.photonManager;
            _blockDataManager = Owner.blockDataManager;
            _blockFactory = Owner.blockFactory;
            _gameOverLine = Owner.gameOverLine;
            _stateMachine = Owner._stateMachine;
            InitializeButton();
            InitializeSubscribe();
            Owner.SwitchUiView((int)Event.Battle);
            if (PhotonNetwork.IsMasterClient)
            {
                _isMyTurn = true;
                var index = _blockDataManager.GetRandomBlockData().Id;
                PhotonNetwork.CurrentRoom.SetBlockIndex(index);
            }
        }

        private void InitializeButton()
        {
            _battleView.rotateButton.onClick.RemoveAllListeners();
            _battleView.rotateButton.onClick.AddListener(OnClickRotate);
        }

        private void InitializeSubscribe()
        {
            _photonManager.ChangeIndex.Subscribe(index => UniTask.Void(async () =>
            {
                if (!_isMyTurn)
                {
                    _isMyTurn = !_isMyTurn;
                    return;
                }

                var blockData = _blockDataManager.GetBlockData(index);
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
                    _isMyTurn = !_isMyTurn;
                }).AddTo(block.GetCancellationTokenOnDestroy());
            })).AddTo(_cancellationTokenSource.Token);

            _gameOverLine.GameEnd.Subscribe(value => { _stateMachine.Dispatch((int)Event.BattleResult); }).AddTo(_cancellationTokenSource.Token);
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
            rigid.gravityScale = 1;
            _currentBlockObj = null;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            blockSc.BlockStateReactiveProperty.Value = BlockSate.Moving;
        }

        private void OnClickRotate()
        {
            if (!_isMyTurn || _currentBlockObj == null)
            {
                return;
            }

            _currentBlockObj.BlockStateReactiveProperty.Value = BlockSate.Rotating;
            _currentBlockObj.transform.localEulerAngles += new Vector3(0, 0, 45);
        }

        private void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}