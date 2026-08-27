using UnityEngine;

namespace JHJ.Scripts.Interaction.Movement
{
    /// <summary>
    /// NPC의 정면 방향으로 플레이어가 있는지 감지하는 전용 컴포넌트.
    /// NPCPatroller 등 이동 스크립트가 이 컴포넌트를 참조해서
    /// "지금 앞에 플레이어 있으니 멈춰야 하나?"를 물어보는 방식으로 씀 (책임 분리).
    ///
    /// 세팅:
    /// 1. 플레이어 오브젝트의 Layer를 "Player"로 지정 (없으면 새로 만들기)
    /// 2. NPC에 이 컴포넌트 Add Component
    /// 3. Player Layer 필드에 "Player" 레이어 체크
    /// </summary>
    public class PlayerPathBlockDetector : MonoBehaviour
    {
        [Header("감지 설정")]
        [SerializeField] private LayerMask playerLayer;
        [Tooltip("정면으로 얼마나 멀리까지 감지할지")]
        [SerializeField] private float detectionDistance = 1.2f;
        [Tooltip("감지용 구체의 반지름 (두꺼울수록 살짝 옆에 있어도 감지됨)")]
        [SerializeField] private float detectionRadius = 0.4f;
        [Tooltip("감지 시작 높이 오프셋 (NPC 발밑 기준이면 허리 높이 정도로)")]
        [SerializeField] private Vector3 originOffset = new Vector3(0f, 1f, 0f);

        /// <summary>지금 이 순간 NPC 정면에 플레이어가 있는지 검사</summary>
        public bool IsPlayerBlocking()
        {
            Vector3 origin = transform.position + originOffset;
            Vector3 direction = transform.forward;

            bool hit = Physics.SphereCast(
                origin,
                detectionRadius,
                direction,
                out RaycastHit hitInfo,
                detectionDistance,
                playerLayer,
                QueryTriggerInteraction.Collide // 플레이어 콜라이더가 트리거여도 감지되게
            );

            if (hit)
                Debug.Log($"[PlayerPathBlockDetector] 감지됨! 대상: {hitInfo.collider.name}, 레이어: {LayerMask.LayerToName(hitInfo.collider.gameObject.layer)}", this);

            return hit;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position + originOffset;
            Vector3 end = origin + transform.forward * detectionDistance;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin, detectionRadius);
            Gizmos.DrawWireSphere(end, detectionRadius);
            Gizmos.DrawLine(origin, end);
        }
    }
}