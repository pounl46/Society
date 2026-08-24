using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 바퀴 위치에서 아래로 레이를 쏴서 지면 접촉 여부를 판단하는 컴포넌트.
/// 각 바퀴(또는 바퀴 바로 위 지점)에 하나씩 붙여서 사용합니다.
/// </summary>
public class WheelGroundCheck : MonoBehaviour
{
    [Header("레이캐스트 설정")]
    [Tooltip("레이를 쏘는 시작점. 비워두면 이 오브젝트의 Transform을 사용합니다.")]
    [SerializeField] private Transform rayOrigin;

    [Tooltip("rayOrigin 기준 로컬 오프셋. X/Z로 옆/앞뒤 위치도 조절 가능, Y로 높이 조절")]
    [SerializeField] private Vector3 rayOriginOffset = Vector3.zero;

    [Tooltip("바퀴 반지름 등을 고려한 레이 길이")]
    [SerializeField] private float rayLength = 0.3f;

    [Tooltip("지면으로 인식할 레이어")]
    [SerializeField] private LayerMask groundLayer = ~0;

    [Tooltip("약간의 여유값. 반지름보다 살짝 크게 잡으면 미세한 뜸도 감지")]
    [SerializeField] private float skinWidth = 0.02f;

    [Header("디버그")]
    [SerializeField] private bool drawGizmo = true;

    // 외부에서 구독 가능한 이벤트 (접지 상태가 "바뀔 때"만 호출)
    public UnityEvent<bool> OnGroundStateChanged;

    [Header("접지 상태 (읽기 전용, 인스펙터에서 실시간 확인용)")]
    [Tooltip("현재 이 바퀴가 바닥에 닿아있는지 여부. 코드로 직접 바꾸지 마세요, CheckGround()가 갱신합니다.")]
    public bool isGrounded;

    /// <summary> 현재 이 바퀴가 바닥에 닿아있는지 여부 (isGrounded와 동일한 값) </summary>
    public bool IsGrounded => isGrounded;

    /// <summary> 마지막으로 감지된 지면의 법선 벡터 (경사 계산 등에 활용) </summary>
    public Vector3 GroundNormal { get; private set; } = Vector3.up;

    /// <summary> 마지막으로 감지된 지면까지의 거리 (닿지 않았으면 rayLength) </summary>
    public float GroundDistance { get; private set; }

    private void Reset()
    {
        rayOrigin = transform;
    }

    private void Awake()
    {
        if (rayOrigin == null)
            rayOrigin = transform;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    /// <summary> rayOrigin의 로컬 축 기준으로 오프셋이 적용된 실제 레이 시작 월드 좌표 </summary>
    private Vector3 GetRayOriginWorldPosition()
    {
        // TransformPoint를 쓰면 오브젝트가 회전해 있어도 오프셋이 로컬 축(좌우/앞뒤/위아래) 기준으로 적용됩니다.
        return rayOrigin.TransformPoint(rayOriginOffset);
    }

    private void CheckGround()
    {
        Vector3 origin = GetRayOriginWorldPosition();
        float castLength = rayLength + skinWidth;

        bool hit = Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hitInfo,
            castLength,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        bool wasGrounded = isGrounded;
        isGrounded = hit;

        if (hit)
        {
            GroundNormal = hitInfo.normal;
            GroundDistance = hitInfo.distance;
        }
        else
        {
            GroundNormal = Vector3.up;
            GroundDistance = castLength;
        }

        // 상태가 바뀐 순간에만 이벤트 발행 (매 프레임 호출 방지)
        if (wasGrounded != isGrounded)
        {
            OnGroundStateChanged?.Invoke(isGrounded);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Transform origin = rayOrigin != null ? rayOrigin : transform;
        Gizmos.color = Application.isPlaying
            ? (IsGrounded ? Color.green : Color.red)
            : Color.yellow;

        Gizmos.DrawLine(origin.position, origin.position + Vector3.down * (rayLength + skinWidth));
        Gizmos.DrawWireSphere(origin.position + Vector3.down * (rayLength + skinWidth), 0.03f);
    }
}