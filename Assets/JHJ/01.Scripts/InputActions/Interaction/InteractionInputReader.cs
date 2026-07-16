using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// Controls(생성된 Input Actions 클래스)를 감싸서
    /// C# 이벤트로 노출하는 ScriptableObject.
    ///
    /// PlayerInteractor는 이 이벤트만 구독하면 되므로,
    /// 키 바인딩이나 입력 방식(키보드/게임패드)이 바뀌어도 상호작용 로직은 건드릴 필요가 없다.
    /// </summary>
    [CreateAssetMenu(fileName = "InteractionInputReader", menuName = "Game/Input/Interaction Input Reader")]
    public class InteractionInputReader : ScriptableObject, Controls.IInteractionActions
    {
        /// <summary>F키(또는 매핑된 다른 버튼)가 눌렸을 때 발생하는 이벤트</summary>
        public event Action OnInteractPressed;

        private Controls _actions;

        private void OnEnable()
        {
            if (_actions == null)
            {
                _actions = new Controls();
                _actions.Interaction.SetCallbacks(this);
            }
            _actions.Interaction.Enable();
        }

        private void OnDisable()
        {
            _actions?.Interaction.Disable();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInteractPressed?.Invoke();
        }
    }
}