using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JHJ.Scripts.Interaction.Interaction
{
    /// <summary>
    /// 마우스 좌클릭(대화 진행용) 입력을 감싸서 이벤트로 노출하는 SO.
    ///
    /// InteractionInputReader(F키, 상호작용 시작)와는 완전히 별개의 도메인이라
    /// 별도의 Reader로 분리함 (OCP 유지 - Interaction 액션맵에 뭘 추가해도
    /// 이 클래스는 영향 없고, Dialogue 액션맵에 뭘 추가해도 InteractionInputReader는 영향 없음).
    ///
    /// 사용 전 준비:
    /// 1. Controls.inputactions 에 Dialogue 맵 / Advance 액션(좌클릭) 추가 후 Inspector에서 Apply
    /// 2. 이 스크립트로 SO 에셋 생성 (Create -> Game -> Input -> Dialogue Input Reader)
    /// 3. DialogueManager에 해당 SO 에셋을 드래그해서 연결
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueInputReader", menuName = "Game/Input/Dialogue Input Reader")]
    public class DialogueInputReader : ScriptableObject, Controls.IDialogueActions
    {
        /// <summary>좌클릭(또는 매핑된 다른 버튼)이 눌렸을 때 발생하는 이벤트</summary>
        public event Action OnAdvancePressed;

        private Controls _actions;

        private void OnEnable()
        {
            if (_actions == null)
            {
                _actions = new Controls();
                _actions.Dialogue.SetCallbacks(this);
            }
            _actions.Dialogue.Enable();
        }

        private void OnDisable()
        {
            _actions?.Dialogue.Disable();
        }

        public void OnAdvance(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAdvancePressed?.Invoke();
        }
    }
}