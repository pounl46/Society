using System.Collections;
using UnityEngine;
using JHJ.Scripts.Interaction.Dialogue;

namespace JHJ.Scripts.Interaction.Movement
{
    /// <summary>
    /// NPC가 정해진 동선(waypoints)을 따라 천천히 순회하는 컴포넌트.
    /// 빈 오브젝트를 따로 만들 필요 없이, 좌표 배열을 이 컴포넌트 자체에 저장함.
    /// Scene 뷰에서 초록 점 + 선으로 경로가 보이니 대략적인 위치 파악이 쉬움.
    ///
    /// 대화 연동: 이 NPC가 지금 플레이어와 대화 중(CurrentSpeaker == 자기 자신)이면
    /// 자동으로 이동을 멈춤. 다른 NPC가 대화 중일 땐 이 NPC는 계속 순회함.
    /// </summary>
    public enum PatrolLoopMode
    {
        Loop,       // 마지막 지점 찍으면 다시 0번으로 돌아가서 순회 반복
        PingPong,   // 끝까지 갔다가 거꾸로 되돌아오는 걸 반복
        Once        // 한 번만 돌고 마지막 지점에서 멈춤
    }

    [DisallowMultipleComponent]
    public class NPCPatroller : MonoBehaviour
    {
        [Header("동선 (월드 좌표 직접 입력, 빈 오브젝트 불필요)")]
        [Tooltip("이 NPC가 순서대로 이동할 좌표들. Scene 뷰에 초록 점으로 표시됨.")]
        [SerializeField] private Vector3[] waypoints;

        [Header("이동 설정")]
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float waitTimeAtPoint = 1.0f;
        [SerializeField] private PatrolLoopMode loopMode = PatrolLoopMode.Loop;
        [SerializeField] private bool rotateTowardsMovement = true;
        [SerializeField] private float rotateSpeed = 10f;

        [Header("대화 연동")]
        [Tooltip("이 NPC가 지금 플레이어와 대화 중일 때 이동을 멈춤")]
        [SerializeField] private bool pauseWhileTalking = true;

        [Header("플레이어 충돌 방지")]
        [Tooltip("비워두면 플레이어 감지 기능 없이 그냥 계속 순회함")]
        [SerializeField] private PlayerPathBlockDetector playerBlockDetector;

        private int _currentIndex;
        private int _pingPongDirection = 1;
        private bool _isWaiting;

        private void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            if (_isWaiting) return;
            if (pauseWhileTalking && IsBeingTalkedTo()) return;
            if (playerBlockDetector != null && playerBlockDetector.IsPlayerBlocking()) return;

            MoveTowardsCurrentWaypoint();
        }

        private bool IsBeingTalkedTo()
        {
            return DialogueManager.IsDialogueActive && DialogueManager.CurrentSpeaker == gameObject;
        }

        private void MoveTowardsCurrentWaypoint()
        {
            Vector3 target = waypoints[_currentIndex];

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (rotateTowardsMovement)
            {
                Vector3 toTarget = target - transform.position;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
                }
            }

            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                StartCoroutine(WaitThenAdvance());
            }
        }

        private IEnumerator WaitThenAdvance()
        {
            _isWaiting = true;
            yield return new WaitForSeconds(waitTimeAtPoint);
            AdvanceIndex();
            _isWaiting = false;
        }

        private void AdvanceIndex()
        {
            switch (loopMode)
            {
                case PatrolLoopMode.Loop:
                    _currentIndex = (_currentIndex + 1) % waypoints.Length;
                    break;

                case PatrolLoopMode.PingPong:
                    if (waypoints.Length == 1) break;
                    _currentIndex += _pingPongDirection;
                    if (_currentIndex >= waypoints.Length)
                    {
                        _currentIndex = waypoints.Length - 2;
                        _pingPongDirection = -1;
                    }
                    else if (_currentIndex < 0)
                    {
                        _currentIndex = 1;
                        _pingPongDirection = 1;
                    }
                    break;

                case PatrolLoopMode.Once:
                    if (_currentIndex < waypoints.Length - 1)
                        _currentIndex++;
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.15f);
                if (i < waypoints.Length - 1)
                    Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
                else if (loopMode == PatrolLoopMode.Loop && waypoints.Length > 1)
                    Gizmos.DrawLine(waypoints[i], waypoints[0]);
            }
        }
    }
}