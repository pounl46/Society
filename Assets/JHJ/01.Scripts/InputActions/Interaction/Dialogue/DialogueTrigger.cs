using UnityEngine;
using JHJ.Scripts.Interaction.Interaction;
using JHJ.Scripts.Interaction.Interactio;

namespace JHJ.Scripts.Interaction.Dialogue
{
    /// <summary>
    /// 대화 가능한 NPC에 붙는 컴포넌트. IInteractable을 구현하므로
    /// PlayerInteractor는 문(InteractableObject)이랑 완전히 동일한 방식으로 감지/실행함.
    ///
    /// RequiredOrder: 이 NPC가 전체 대화 순서 중 몇 번째인지 (1부터 시작).
    /// DialogueManager.CompletedCount가 (RequiredOrder - 1)과 같을 때만
    /// 즉, "내 앞 순서 NPC들이 전부 대화를 끝냈을 때만" 상호작용 가능해짐.
    ///
    /// [변경] 범위 감지를 PlayerInteractor의 전역 OverlapSphere에만 맡기지 않고,
    /// DialogueTrigger 자신이 "플레이어가 내 감지 범위 안에 있는가"를 직접 판단한다.
    /// PlayerInteractor는 CanInteract가 false인 대상은 애초에 후보에서 제외하므로,
    /// 이 자체 범위 체크 결과를 CanInteract에 합치기만 하면 PlayerInteractor 쪽은
    /// 전혀 수정할 필요가 없다.
    ///
    /// 필요 조건: Collider + Layer를 Interactable로 지정 (기존 상호작용 오브젝트와 동일)
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

        [Header("자체 범위 감지")]
        [Tooltip("이 NPC 스스로 플레이어와의 거리를 재는 반경. PlayerInteractor의 감지 반경과는 별개.")]
        [SerializeField] private float detectionRadius = 2f;
        [Tooltip("플레이어 태그. 씬에 Player 태그가 붙은 오브젝트가 있어야 함.")]
        [SerializeField] private string playerTag = "Player";

        private Transform _player;
        private bool _isPlayerInRange;

        public string InteractionPrompt =>
            promptData != null ? promptData.InteractionPrompt : "F - 대화하기";

        // 이미 대화중이 아니고, 내 앞 순서(requiredOrder - 1)까지 끝났고,
        // 플레이어가 내 감지 범위 안에 있어야 가능.
        // ">="라서 한 번 차례가 되면 그 이후로도(재대화 포함) 계속 상호작용 가능.
        public bool CanInteract
        {
            get
            {
                bool result = _isPlayerInRange &&
                              !DialogueManager.IsDialogueActive &&
                              DialogueManager.CompletedCount >= requiredOrder - 1;

                // TODO: 원인 확인되면 이 로그 지우기
                Debug.Log($"[DialogueTrigger:{gameObject.name}] CanInteract={result} " +
                          $"(inRange={_isPlayerInRange}, dialogueActive={DialogueManager.IsDialogueActive}, " +
                          $"completed={DialogueManager.CompletedCount}, requiredOrder={requiredOrder})");

                return result;
            }
        }

        public Vector3 PromptWorldPosition => transform.position + Vector3.up * promptHeightOffset;

        private void Awake()
        {
            // 플레이어 참조를 미리 캐싱해서 매 프레임 GameObject.FindWithTag를 호출하지 않는다.
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                _player = playerObj.transform;
                Debug.Log($"[DialogueTrigger:{gameObject.name}] 플레이어 찾음: {playerObj.name}"); // TODO: 확인되면 지우기
            }
            else
                Debug.LogWarning($"[DialogueTrigger] '{playerTag}' 태그를 가진 오브젝트를 찾지 못했습니다.", this);
        }

        private void Update()
        {
            UpdatePlayerRangeState();
        }

        /// <summary>플레이어와의 거리를 재서 _isPlayerInRange를 갱신한다.</summary>
        private void UpdatePlayerRangeState()
        {
            if (_player == null)
            {
                _isPlayerInRange = false;
                return;
            }

            float sqrDist = (_player.position - transform.position).sqrMagnitude;
            _isPlayerInRange = sqrDist <= detectionRadius * detectionRadius;

            // TODO: 원인 확인되면 이 로그 지우기
            Debug.Log($"[DialogueTrigger:{gameObject.name}] 실제거리={Mathf.Sqrt(sqrDist):F2}, " +
                      $"detectionRadius={detectionRadius}, inRange={_isPlayerInRange}");
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}