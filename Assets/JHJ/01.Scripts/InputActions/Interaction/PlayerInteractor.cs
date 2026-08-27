using JHJ.Scripts.Interaction.Interactio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JHJ.Scripts.Interaction.Interactio
{
    /// <summary>
    /// 플레이어에 부착되는 컴포넌트.
    /// - 더 이상 매 프레임 스스로 주변을 스캔하지 않음.
    /// - 각 상호작용 오브젝트에 붙은 InteractionRangeTrigger가 "플레이어 들어옴/나감"을
    ///   직접 알려주면, 그 목록 중 제일 가까운 대상을 현재 타겟으로 유지함.
    /// - InteractionInputReader의 F키 이벤트를 받으면 그 타겟의 Interact()를 실행한다.
    /// - 타겟이 바뀔 때마다 OnTargetChanged 이벤트를 발생시켜, UI 등 다른 시스템이
    ///   이 클래스 내부 구조를 몰라도 반응할 수 있게 한다.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("참조")]
        [Tooltip("F키 입력 이벤트를 받아올 Input Reader SO")]
        [SerializeField] private InteractionInputReader inputReader;

        /// <summary>감지된 상호작용 대상이 바뀔 때마다 발생 (없어지면 null로 발생)</summary>
        public event Action<IInteractable> OnTargetChanged;

        private readonly List<IInteractable> _inRangeInteractables = new List<IInteractable>();
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

        /// <summary>InteractionRangeTrigger가 플레이어 진입을 감지하면 호출함</summary>
        public void NotifyEnterRange(IInteractable interactable)
        {
            if (interactable == null) return;
            if (!_inRangeInteractables.Contains(interactable))
                _inRangeInteractables.Add(interactable);
        }

        /// <summary>InteractionRangeTrigger가 플레이어 이탈을 감지하면 호출함</summary>
        public void NotifyExitRange(IInteractable interactable)
        {
            if (interactable == null) return;
            _inRangeInteractables.Remove(interactable);
        }

        /// <summary>범위 안에 들어와 있는 대상들 중 가장 가까운 것을 현재 타겟으로 갱신</summary>
        private void RecomputeCurrentTarget()
        {
            IInteractable closest = null;
            float closestSqrDist = float.MaxValue;

            for (int i = _inRangeInteractables.Count - 1; i >= 0; i--)
            {
                IInteractable candidate = _inRangeInteractables[i];

                if (candidate == null || candidate is not Component comp || comp == null)
                {
                    _inRangeInteractables.RemoveAt(i);
                    continue;
                }

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
                Debug.Log("[PlayerInteractor] 주변에 상호작용 가능한 오브젝트가 없습니다.");
                return;
            }

            _currentTarget.Interact(gameObject);
        }
    }
}