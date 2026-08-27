using UnityEngine;
using JHJ.Scripts.Interaction.Interaction;
using JHJ.Scripts.Interaction.Interactio;

namespace JHJ.Scripts.Interaction.Dialogue
{
    /// <summary>
    /// 대화 가능한 NPC에 붙는 컴포넌트. IInteractable을 구현하므로
    /// PlayerInteractor는 InteractableRegistry를 통해 다른 오브젝트와 완전히 동일한
    /// 방식으로 이 NPC를 후보로 취급함 (거리 판단은 PlayerInteractor가 알아서 함).
    ///
    /// RequiredOrder: 이 NPC가 전체 대화 순서 중 몇 번째인지 (1부터 시작).
    /// DialogueManager.CompletedCount가 (RequiredOrder - 1) 이상일 때만
    /// 즉, "내 앞 순서 NPC들이 전부 대화를 끝냈을 때만" 상호작용 가능해짐.
    ///
    /// 필요 조건: 없음 (콜라이더/트리거/태그 전혀 필요 없음 - 레지스트리 + 거리 계산 방식이라)
    /// </summary>
    [DisallowMultipleComponent]
    public class DialogueTrigger : MonoBehaviour, IInteractable
    {
        [Header("표시용 (프롬프트 텍스트, 문이랑 동일한 SO 재사용)")]
        [SerializeField] private InteractableDataSO promptData;

        [Header("실제 대사 내용")]
        [SerializeField] private DialogueDataSO dialogueData;

        [Header("프롬프트 표시 높이")]
        [Tooltip("머리 위 F 프롬프트를 띄울 높이. 문 쪽 SO에 별도 오프셋 필드가 있으면 그 값으로 교체해도 됨.")]
        [SerializeField] private float promptHeightOffset = 1.6f;

        [Header("대화 순서")]
        [Tooltip("이 NPC가 몇 번째로 대화해야 하는 NPC인지 (1부터 시작). 앞 순서가 아직 안 끝났으면 상호작용 불가.")]
        [SerializeField] private int requiredOrder = 1;

        public string InteractionPrompt =>
            promptData != null ? promptData.InteractionPrompt : "F - 대화하기";

        // 이미 대화중이 아니고, 내 앞 순서(requiredOrder - 1)까지 끝났으면 가능.
        // 거리 판단은 PlayerInteractor(maxInteractDistance)가 담당하므로 여기선 신경 안 씀.
        public bool CanInteract =>
            !DialogueManager.IsDialogueActive &&
            DialogueManager.CompletedCount >= requiredOrder - 1;

        public Vector3 PromptWorldPosition => transform.position + Vector3.up * promptHeightOffset;

        private void OnEnable()
        {
            InteractableRegistry.Register(this);
            Debug.Log($"[DialogueTrigger] 레지스트리 등록됨: {gameObject.name}", this);
        }

        private void OnDisable()
        {
            InteractableRegistry.Unregister(this);
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract) return;

            if (dialogueData == null)
            {
                Debug.LogWarning($"[DialogueTrigger] {gameObject.name}에 DialogueDataSO가 연결되지 않았습니다.", this);
                return;
            }

            DialogueManager.Instance.StartDialogue(dialogueData, gameObject);
        }
    }
}