using Unity.VisualScripting;
using UnityEngine.EventSystems;
using State = StateMachine<GameCore>.State;

public partial class GameCore
{
    public class BattleState : State, IPointerDownHandler
    {
        private BattleView _battleView;

        protected override void OnEnter(State prevState)
        {
            Initialize();
        }

        private void Initialize()
        {
            _battleView = Owner.battleView;
            InitializeButton();
            Owner.SwitchUiView((int)Event.Battle);
        }

        private void InitializeButton()
        {
            _battleView.rotateButton.onClick.RemoveAllListeners();
            _battleView.rotateButton.onClick.AddListener(OnClickRotate);
        }


        private void OnClickRotate()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}