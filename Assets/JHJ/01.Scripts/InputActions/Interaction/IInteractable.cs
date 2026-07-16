using UnityEngine;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// 상호작용 가능한 모든 오브젝트가 구현해야 하는 인터페이스.
    /// PlayerInteractor는 이 인터페이스에만 의존하므로,
    /// 구현체가 무엇이든(문, 아이템, NPC...) 동일한 방식으로 다룰 수 있다.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>UI 등에 표시할 상호작용 안내 텍스트 (예: "F - 문 열기")</summary>
        string InteractionPrompt { get; }

        /// <summary>현재 상호작용이 가능한 상태인지 (잠김/쿨다운 등에 사용)</summary>
        bool CanInteract { get; }

        /// <summary>F 프롬프트 등 UI를 표시할 월드 좌표 (보통 오브젝트 머리 위)</summary>
        Vector3 PromptWorldPosition { get; }

        /// <summary>실제 상호작용 로직 실행</summary>
        void Interact(GameObject interactor);
    }
}