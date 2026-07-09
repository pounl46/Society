using JHJ.Scripts.Interaction.Interaction;
using UnityEngine;

namespace JHJ.Scripts.Interaction.Interaction
{
    /// <summary>
    /// 월드에 배치되는 개별 상호작용 오브젝트.
    /// InteractableDataSO를 참조해 데이터를 가져오고, 실제 동작은 여기서 구현한다.
    /// 지금은 디버그 로그만 출력 (추후 이 부분만 확장하면 됨).
    ///
    /// 필요 조건: 이 오브젝트에 Collider(IsTrigger 여부 무관)가 있어야 하고,
    /// PlayerInteractor가 감지할 수 있도록 Layer를 "Interactable" 등으로 지정해야 함.
    /// </summary>
    [DisallowMultipleComponent]
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteractableDataSO data;
        [SerializeField] private bool canInteract = true;

        public string InteractionPrompt => data != null ? data.InteractionPrompt : "F - 상호작용";
        public bool CanInteract => canInteract;
        public InteractableDataSO Data => data;

        public void Interact(GameObject interactor)
        {
            if (!CanInteract)
            {
                Debug.Log($"[Interactable] {gameObject.name} 은(는) 현재 상호작용 불가 상태입니다.", this);
                return;
            }

            // TODO: 실제 상호작용 로직 (아이템 획득, 문 열기, 대화 시작 등)으로 교체
            string label = data != null ? data.InteractableName : gameObject.name;
            Debug.Log($"[Interact] {interactor.name} -> {label} 상호작용 실행됨", this);
        }

        private void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.InteractionRange);
        }
    }
}