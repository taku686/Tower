using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager.DataManager;
using UnityEngine;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class NameChangeState : State
    {
        private NameChangeView _nameChangeView;
        private StateMachine<GameCore> _stateMachine;
        private UserDataManager _userDataManager;
        private IconDataManager _iconDataManager;
        private NgWordDataManager _ngWordDataManager;
        private readonly List<IconGrid> _iconGrids = new();

        protected override void OnEnter(State prevState)
        {
            Initialize();
            InitializeButton();
            SetupUiContent();
            Owner.SwitchUiView((int)Event.NameChange);
        }

        protected override void OnExit(State nextState)
        {
            DestroyIconGrids();
        }

        private void Initialize()
        {
            _nameChangeView = Owner.nameChangeView;
            _stateMachine = Owner._stateMachine;
            _userDataManager = Owner.userDataManager;
            _iconDataManager = Owner.iconDataManager;
            _ngWordDataManager = Owner.ngWordDataManager;
        }

        private void InitializeButton()
        {
            _nameChangeView.closeButton.onClick.RemoveAllListeners();
            _nameChangeView.closeButton.onClick.AddListener(() =>
                UniTask.Void(async () => { await OnClickCloseView(); }));
        }

        private void SetupUiContent()
        {
            _nameChangeView.inputField.text = _userDataManager.GetUserName();
            GenerateIconGrid();
        }

        private async UniTask OnClickCloseView()
        {
            SoundManager.Instance.DecideSe();
            var name = _nameChangeView.inputField.text;
            if (!_ngWordDataManager.Validate(name))
            {
                Debug.Log("名前が適切ではありません。");
                return;
            }

            var result = await _userDataManager.SetUserNameAsync(name);
            if (!result)
            {
                Debug.Log("名前が適切ではありません。");
                return;
            }

            await _userDataManager.UpdateUserData();
            _stateMachine.Dispatch((int)Event.Title);
        }

        private void OnClickIconGrid(int index)
        {
            SoundManager.Instance.DecideSe();
            _userDataManager.SetIconIndex(index);
        }

        private void GenerateIconGrid()
        {
            foreach (var iconData in _iconDataManager.GetIconDatum())
            {
                var grid = Instantiate(_nameChangeView.iconGrid.gameObject, _nameChangeView.iconParent);
                var iconGrid = grid.GetComponent<IconGrid>();
                iconGrid.iconImage.sprite = iconData.Sprite;
                iconGrid.index = iconData.Index;
                iconGrid.iconButton.onClick.AddListener(() => { OnClickIconGrid(iconData.Index); });
                _iconGrids.Add(iconGrid);
            }
        }

        private void DestroyIconGrids()
        {
            foreach (var iconGrid in _iconGrids)
            {
                Destroy(iconGrid.gameObject);
            }

            _iconGrids.Clear();
        }
    }
}