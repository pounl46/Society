using UnityEngine;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// 상호작용 가능한 오브젝트(NPC, 문 등)에 붙는 감지용 컴포넌트.
    /// 이 오브젝트에 원형(또는 다른 모양) Trigger 콜라이더를 하나 추가하고,
    /// 플레이어가 그 안에 들어오면 자동으로 PlayerInteractor에게 "나 범위 안에 있어"라고 알림.
    /// 플레이어가 나가면 "나갔어"라고 알림.
    ///
    /// 세팅:
    /// 1. 이 오브젝트(NPC 등)에 이 스크립트 Add Component
    /// 2. 같은 오브젝트에 Sphere Collider(또는 원하는 모양) 추가로 Add Component
    /// 3. 그 Collider의 Is Trigger 체크 ON
    /// 4. Radius(또는 Size)로 감지 범위 조절 (F가 뜨는 거리)
    /// 5. 플레이어 오브젝트의 Tag를 "Player"로 지정 (유니티 기본 제공 태그, 새로 안 만들어도 됨)
    ///
    /// 기존 물리 충돌용 Box Collider(Is Trigger 꺼짐)는 그대로 둬도 됨 -
    /// 한 오브젝트에 콜라이더 여러 개 있어도 되고, 이 트리거는 감지 전용으로 별도 동작함.
    /// </summary>
    public class InteractionRangeTrigger : MonoBehaviour
    {
        [Tooltip("플레이어 오브젝트의 Tag (유니티 기본 Player 태그 사용 권장)")]
        [SerializeField] private string playerTag = "Player";

        private IInteractable _interactable;

        private void Awake()
        {
            _interactable = GetComponent<IInteractable>();
            if (_interactable == null)
                Debug.LogError($"[InteractionRangeTrigger] {gameObject.name}에 IInteractable을 구현한 컴포넌트(DialogueTrigger, InteractableObject 등)가 없습니다.", this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_interactable == null || !other.CompareTag(playerTag)) return;

            var playerInteractor = other.GetComponentInParent<PlayerInteractor>();
            if (playerInteractor != null)
                playerInteractor.NotifyEnterRange(_interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_interactable == null || !other.CompareTag(playerTag)) return;

            var playerInteractor = other.GetComponentInParent<PlayerInteractor>();
            if (playerInteractor != null)
                playerInteractor.NotifyExitRange(_interactable);
        }
    }
}