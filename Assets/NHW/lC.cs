using UnityEngine;

public class lC : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform rayOrigin; // 없으면 transform 사용

    private bool isGrounded;

    void Update()
    {
        CheckGround();
        Debug.Log(isGrounded);
    }

    private void CheckGround()
    {
        Vector3 origin = rayOrigin != null ? rayOrigin.position : transform.position;
        RaycastHit hit;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public bool IsGrounded => isGrounded;

    private void OnDrawGizmos()
    {
        Vector3 origin = rayOrigin != null ? rayOrigin.position : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayDistance);
        Gizmos.DrawSphere(origin + Vector3.down * rayDistance, 0.05f);
    }
}
