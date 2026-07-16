
using JHJ.Scripts.Interaction.Interactio;
using UnityEngine;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// 상호작용 가능한 대상 위에 F 프롬프트를 표시하는 화면 오버레이 UI.
    /// PlayerInteractor.OnTargetChanged 이벤트만 구독하므로,
    /// 감지 로직(PlayerInteractor)이나 데이터(IInteractable) 쪽 내부 구조는 몰라도 된다.
    ///
    /// 지금은 단순 텍스트("F") placeholder이고, 추후 아이콘/애니메이션이 들어간
    /// 진짜 UI로 교체할 때 이 스크립트 내부만 수정하면 된다 (다른 스크립트는 안 건드림).
    /// </summary>
    public class InteractionPromptUI : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private PlayerInteractor playerInteractor;
        [Tooltip("화면에 표시될 프롬프트 UI(Text/Image 등)의 RectTransform")]
        [SerializeField] private RectTransform promptRoot;
        [SerializeField] private Camera targetCamera;

        private IInteractable _trackedTarget;

        private void Reset()
        {
            targetCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (playerInteractor != null)
                playerInteractor.OnTargetChanged += HandleTargetChanged;
            else
                Debug.LogWarning("[InteractionPromptUI] PlayerInteractor가 연결되지 않았습니다.", this);

            SetVisible(false);
        }

        private void OnDisable()
        {
            if (playerInteractor != null)
                playerInteractor.OnTargetChanged -= HandleTargetChanged;
        }

        private void HandleTargetChanged(IInteractable target)
        {
            _trackedTarget = target;
            SetVisible(target != null);
        }

        private void LateUpdate()
        {
            if (_trackedTarget == null || targetCamera == null || promptRoot == null) return;

            Vector3 screenPos = targetCamera.WorldToScreenPoint(_trackedTarget.PromptWorldPosition);

            // 카메라 뒤쪽으로 넘어간 경우(플레이어가 지나쳐서 등 뒤가 된 경우) 숨김
            bool behindCamera = screenPos.z < 0f;
            if (promptRoot.gameObject.activeSelf == behindCamera)
                promptRoot.gameObject.SetActive(!behindCamera);

            if (!behindCamera)
                promptRoot.position = screenPos;
        }

        private void SetVisible(bool visible)
        {
            if (promptRoot != null)
                promptRoot.gameObject.SetActive(visible);
        }
    }
}