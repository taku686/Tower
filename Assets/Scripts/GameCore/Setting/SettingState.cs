using System;
using UnityEngine;
using UnityEngine.Events;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class SettingState : State
    {
        private SettingView _settingView;
        private StateMachine<GameCore> _stateMachine;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        private void Initialize()
        {
            _settingView = Owner.settingView;
            _stateMachine = Owner._stateMachine;
            InitializeButton();
            SoundManager.Instance.DecideSe();
            Owner.SwitchUiView((int)Event.Setting);
        }

        protected override void OnUpdate()
        {
            var seValue = _settingView.seSlider.value;
            var bgmValue = _settingView.bgmSlider.value;
            OnValueChangeBgmVolume(bgmValue);
            OnValueChangeSeVolume(seValue);
        }

        private void InitializeButton()
        {
            _settingView.closeButton.onClick.RemoveAllListeners();
            _settingView.closeButton.onClick.AddListener(OnClickCloseView);
        }

        private void OnClickCloseView()
        {
            SoundManager.Instance.CancelSe();
            _stateMachine.Dispatch((int)Event.Title);
        }

        private void OnValueChangeSeVolume(float volume)
        {
            SoundManager.Instance.SeChangeVolume(volume);
        }

        private void OnValueChangeBgmVolume(float volume)
        {
            SoundManager.Instance.BgmChangeVolume(volume);
        }
    }
}