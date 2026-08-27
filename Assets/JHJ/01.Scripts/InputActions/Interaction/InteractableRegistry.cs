using System.Collections.Generic;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// 씬에 존재하는 모든 IInteractable 구현체를 모아두는 정적 레지스트리.
    /// 각 구현체(DialogueTrigger, InteractableObject 등)는 OnEnable에서 Register,
    /// OnDisable에서 Unregister만 호출하면 됨. PlayerInteractor는 이 목록만 순회해서
    /// 가장 가까운 대상을 고르므로, 콜라이더/트리거/태그/레이어가 전혀 필요 없음.
    /// </summary>
    public static class InteractableRegistry
    {
        private static readonly List<IInteractable> _all = new List<IInteractable>();

        public static IReadOnlyList<IInteractable> All => _all;

        public static void Register(IInteractable interactable)
        {
            if (interactable == null || _all.Contains(interactable)) return;
            _all.Add(interactable);
        }

        public static void Unregister(IInteractable interactable)
        {
            if (interactable == null) return;
            _all.Remove(interactable);
        }
    }
}