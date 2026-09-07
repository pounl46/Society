using UnityEngine;
using JHJ.Scripts.Interaction.Dialogue;

namespace JHJ.Scripts.Interaction.Movement
{
    /// <summary>
    /// 이 NPC와의 대화가 끝나면 즉시 사라지는 컴포넌트.
    ///
    /// 세팅: 사라져야 하는 NPC 오브젝트에 이 스크립트만 Add Component하면 끝.
    /// DialogueManager는 씬에서 자동으로 찾음.
    /// </summary>
    public class DisappearAfterDialogue : MonoBehaviour
    {
        [Tooltip("체크하면 오브젝트 자체를 파괴함. 끄면 SetActive(false)만 함(나중에 다시 켤 수 있음)")]
        [SerializeField] private bool destroyOnFinish = true;

        private DialogueManager _dialogueManager;
        private bool _isTargetSpeaker;

        private void OnEnable()
        {
            _dialogueManager = DialogueManager.Instance != null
                ? DialogueManager.Instance
                : FindFirstObjectByType<DialogueManager>();

            if (_dialogueManager == null)
            {
                Debug.LogWarning("[DisappearAfterDialogue] DialogueManager를 찾지 못했습니다.", this);
                return;
            }

            _dialogueManager.OnDialogueStarted += HandleDialogueStarted;
            _dialogueManager.OnDialogueEnded += HandleDialogueEnded;
        }

        private void OnDisable()
        {
            if (_dialogueManager == null) return;
            _dialogueManager.OnDialogueStarted -= HandleDialogueStarted;
            _dialogueManager.OnDialogueEnded -= HandleDialogueEnded;
        }

        private void HandleDialogueStarted()
        {
            // 대화가 "나"랑 시작된 건지 기록해둠 (OnDialogueEnded 시점엔 CurrentSpeaker가 이미 null이라 미리 저장)
            _isTargetSpeaker = DialogueManager.CurrentSpeaker == gameObject;
        }

        private void HandleDialogueEnded()
        {
            if (!_isTargetSpeaker) return;

            if (destroyOnFinish)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}