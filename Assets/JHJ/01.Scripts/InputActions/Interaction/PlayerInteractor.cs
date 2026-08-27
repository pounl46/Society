using JHJ.Scripts.Interaction.Interactio;
using System;
using UnityEngine;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// 플레이어에 부착되는 컴포넌트.
    /// - 매 프레임 주변의 IInteractable을 감지해 가장 가까운 대상을 추적하고,
    /// - InteractionInputReader의 F키 이벤트를 받으면 해당 대상의 Interact()를 실행한다.
    /// - 타겟이 바뀔 때마다 OnTargetChanged 이벤트를 발생시켜, UI 등 다른 시스템이
    ///   이 클래스 내부 구조를 몰라도 반응할 수 있게 한다.
    ///
    /// 감지 중심점은 항상 "이 오브젝트(플레이어) 자신의 위치 + 오프셋"으로 고정된다.
    /// 예전처럼 다른 오브젝트의 Transform을 드래그해서 잘못 연결하는 실수가
    /// 구조적으로 불가능하도록, Transform 참조가 아니라 숫자(Vector3 오프셋)로만 조절한다.
    ///
    /// 무빙 스크립트와는 완전히 독립적으로 동작하므로 다른 스크립트를 건드릴 필요가 없다.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("참조")]
        [Tooltip("F키 입력 이벤트를 받아올 Input Reader SO")]
        [SerializeField] private InteractionInputReader inputReader;

        [Header("감지 설정")]
        [SerializeField] private float detectionRadius = 2.5f;
        [SerializeField] private LayerMask interactableLayer;
        [Tooltip("감지 중심점을 이 오브젝트 위치에서 얼마나 띄울지 (보통 0,0,0이면 충분함)")]
        [SerializeField] private Vector3 detectionOriginOffset = Vector3.zero;

        /// <summary>감지된 상호작용 대상이 바뀔 때마다 발생 (없어지면 null로 발생)</summary>
        public event Action<IInteractable> OnTargetChanged;

        private readonly Collider[] _overlapBuffer = new Collider[10];
        private IInteractable _currentTarget;

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.OnInteractPressed += HandleInteractPressed;
            else
                Debug.LogWarning("[PlayerInteractor] InteractionInputReader가 연결되지 않았습니다.", this);
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.OnInteractPressed -= HandleInteractPressed;
        }

        private void Update()
        {
            DetectClosestInteractable();
        }

        /// <summary>범위 내 IInteractable 중 가장 가까운 대상을 찾아 _currentTarget에 저장</summary>
        private void DetectClosestInteractable()
        {
            Vector3 origin = transform.position + detectionOriginOffset;
            int count = Physics.OverlapSphereNonAlloc(origin, detectionRadius, _overlapBuffer, interactableLayer);

            IInteractable closest = null;
            float closestSqrDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!_overlapBuffer[i].TryGetComponent(out IInteractable interactable)) continue;
                if (!interactable.CanInteract) continue;

                float sqrDist = (_overlapBuffer[i].transform.position - origin).sqrMagnitude;
                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    closest = interactable;
                }
            }

            if (!ReferenceEquals(closest, _currentTarget))
            {
                _currentTarget = closest;

                if (_currentTarget != null)
                    Debug.Log($"[PlayerInteractor] 감지됨: {_currentTarget.InteractionPrompt}");

                OnTargetChanged?.Invoke(_currentTarget);
            }
        }

        private void HandleInteractPressed()
        {
            if (_currentTarget == null)
            {
                Debug.Log("[PlayerInteractor] 주변에 상호작용 가능한 오브젝트가 없습니다.");
                return;
            }

            _currentTarget.Interact(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position + detectionOriginOffset;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(origin, detectionRadius);
        }
    }
}