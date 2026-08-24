using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    

    public enum Gear { Forward, Reverse }
    [Header("바퀴 오브젝트 (비주얼 회전용, 선택사항)")]
    public Transform leftWheel;
    public Transform rightWheel;

    [Header("바퀴 접지 감지 (WheelGroundCheck)")]
    [Tooltip("왼쪽 바퀴에 붙인 WheelGroundCheck. 비워두면 접지 체크 없이 항상 입력 허용")]
    public WheelGroundCheck leftWheelGroundCheck;
    [Tooltip("오른쪽 바퀴에 붙인 WheelGroundCheck. 비워두면 접지 체크 없이 항상 입력 허용")]
    public WheelGroundCheck rightWheelGroundCheck;

    [Header("휠체어 규격")]
    [Tooltip("바퀴 반지름 (m)")]
    public float wheelRadius = 0.3f;
    [Tooltip("좌우 바퀴 사이 거리 (m) - 넓을수록 회전이 느려짐")]
    public float trackWidth = 0.6f;

    [Header("속도 설정")]
    [Tooltip("바퀴 최대 각속도 (도/초)")]
    public float maxWheelSpeed = 250f;
    [Tooltip("가속도 (클수록 빨리 최고속도 도달)")]
    public float acceleration = 6f;
    [Tooltip("손을 뗐을 때 감속되는 정도 (마찰)")]
    public float wheelFriction = 4f;
    [Tooltip("Shift 누르고 있을 때 곱해지는 속도 배율")]
    public float sprintMultiplier = 1.8f;

    private Rigidbody rb;
    private float leftWheelSpeed;   // 현재 왼쪽 바퀴 각속도 (도/초)
    private float rightWheelSpeed;  // 현재 오른쪽 바퀴 각속도 (도/초)

    [Header("기어 상태 (읽기 전용, 확인용)")]
    [SerializeField] private Gear currentGear = Gear.Forward; // 기본값: 전진기어
    public Gear CurrentGear => currentGear;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 넘어지지 않도록 X, Z축 회전은 고정 (Y축 회전 = 좌우 방향 전환만 허용)
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        HandleInput();
        RotateWheelMeshes();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void HandleInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // 키보드가 인식 안 될 때 예외 방지

        // 기어 전환: W = 전진기어, S = 후진기어
        if (keyboard.wKey.wasPressedThisFrame)
            currentGear = Gear.Forward;
        else if (keyboard.sKey.wasPressedThisFrame)
            currentGear = Gear.Reverse;

        // 기어 방향에 따라 목표 속도의 부호가 바뀜 (전진: +, 후진: -)
        float gearSign = (currentGear == Gear.Forward) ? 1f : -1f;

        // Shift를 누르고 있으면 속도 배율 적용 (왼쪽 또는 오른쪽 Shift 둘 다 인식)
        bool sprintPressed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        float speedMultiplier = sprintPressed ? sprintMultiplier : 1f;

        float targetSpeed = maxWheelSpeed * gearSign * speedMultiplier;

        // 바퀴가 바닥에 안 닿아있으면(=WheelGroundCheck가 false) 해당 바퀴 입력을 막는다.
        // GroundCheck를 안 꽂아뒀으면(null) 예전처럼 항상 입력 허용.
        bool leftGrounded = leftWheelGroundCheck == null || leftWheelGroundCheck.isGrounded;
        bool rightGrounded = rightWheelGroundCheck == null || rightWheelGroundCheck.isGrounded;

        bool leftPressed = keyboard.aKey.isPressed && leftGrounded;
        bool rightPressed = keyboard.dKey.isPressed && rightGrounded;

        leftWheelSpeed = leftPressed
            ? Mathf.MoveTowards(leftWheelSpeed, targetSpeed, acceleration * 100f * Time.deltaTime)
            : Mathf.MoveTowards(leftWheelSpeed, 0f, wheelFriction * 100f * Time.deltaTime);

        rightWheelSpeed = rightPressed
            ? Mathf.MoveTowards(rightWheelSpeed, targetSpeed, acceleration * 100f * Time.deltaTime)
            : Mathf.MoveTowards(rightWheelSpeed, 0f, wheelFriction * 100f * Time.deltaTime);
    }

    void ApplyMovement()
    {
        // 바퀴 각속도(도/초) -> 바퀴 접지면 선속도(m/s)
        float leftLinearSpeed = leftWheelSpeed * Mathf.Deg2Rad * wheelRadius;
        float rightLinearSpeed = rightWheelSpeed * Mathf.Deg2Rad * wheelRadius;

        // 차동 구동 공식: 전진 속도 = 평균, 회전 속도 = 좌우 차이
        float forwardSpeed = (leftLinearSpeed + rightLinearSpeed) * 0.5f;
        float turnRateRad = (rightLinearSpeed - leftLinearSpeed) / trackWidth; // rad/s

        // 전/후진 이동 (Y축 속도는 중력 등 기존 값 유지)
        Vector3 moveVelocity = transform.forward * forwardSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

        // 좌우 회전 적용
        float turnAngleThisFrame = turnRateRad * Mathf.Rad2Deg * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAngleThisFrame, 0f));
    }

    void RotateWheelMeshes()
    {
        // 바퀴 메쉬가 실제로 굴러가는 것처럼 보이게 로컬 X축 기준 회전
        if (leftWheel != null)
            leftWheel.Rotate(Vector3.right, leftWheelSpeed * Time.deltaTime, Space.Self);
        if (rightWheel != null)
            rightWheel.Rotate(Vector3.right, rightWheelSpeed * Time.deltaTime, Space.Self);
    }
}