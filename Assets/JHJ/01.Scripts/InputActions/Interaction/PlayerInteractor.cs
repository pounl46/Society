using JHJ.Scripts.Interaction.Interactio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// 플레이어에 부착되는 컴포넌트.
    /// - 씬에 있는 모든 IInteractable 구현체를 정적 레지스트리(IInteractableRegistry)에서
    ///   가져와서, 그 중 CanInteract == true인 것들 중 가장 가까운 대상을 매 프레임 갱신함.
    /// - 트리거/콜라이더/태그/레이어 전혀 필요 없음. 거리 판단은 각 IInteractable이
    ///   CanInteract 안에서 알아서 하거나(DialogueTrigger처럼), 안 해도 됨(문처럼 항상 true여도
    ///   여기서 거리로 가장 가까운 것만 고르므로 크게 문제 없음).
    /// - InteractionInputReader의 F키 이벤트를 받으면 현재 타겟의 Interact()를 실행한다.
    /// - 타겟이 바뀔 때마다 OnTargetChanged 이벤트를 발생시켜, UI 등 다른 시스템이
    ///   이 클래스 내부 구조를 몰라도 반응할 수 있게 한다.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("참조")]
        [Tooltip("F키 입력 이벤트를 받아올 Input Reader SO")]
        [SerializeField] private InteractionInputReader inputReader;

        [Header("감지 설정")]
        [Tooltip("이 범위 밖에 있는 대상은 아무리 CanInteract가 true여도 후보에서 제외함")]
        [SerializeField] private float maxInteractDistance = 3f;

        /// <summary>감지된 상호작용 대상이 바뀔 때마다 발생 (없어지면 null로 발생)</summary>
        public event Action<IInteractable> OnTargetChanged;

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
            RecomputeCurrentTarget();
        }

        private void RecomputeCurrentTarget()
        {
            IInteractable closest = null;
            float closestSqrDist = maxInteractDistance * maxInteractDistance;

            foreach (var candidate in InteractableRegistry.All)
            {
                if (candidate == null || candidate is not Component comp || comp == null) continue;
                if (!candidate.CanInteract) continue;

                float sqrDist = (comp.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    closest = candidate;
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
                Debug.Log($"[PlayerInteractor] 주변에 상호작용 가능한 오브젝트가 없습니다. (레지스트리 전체 개수: {InteractableRegistry.All.Count})");

                foreach (var candidate in InteractableRegistry.All)
                {
                    if (candidate is not Component comp || comp == null) continue;
                    float dist = Vector3.Distance(comp.transform.position, transform.position);
                    Debug.Log($"  - {comp.name}: CanInteract={candidate.CanInteract}, 거리={dist:F2} (허용 거리={maxInteractDistance})");
                }

                return;
            }

            _currentTarget.Interact(gameObject);
        }
    }
}