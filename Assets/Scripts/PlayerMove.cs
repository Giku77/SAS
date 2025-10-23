using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private static readonly string animationSpeed = "Speed";

    public Camera mainCamera;
    public float moveSpeed = 5f;
    public float rotateSpeed = 10f;
    public float minMoveDistance = 0.5f; // 너무 가까우면 무시할 거리

    private Animator animator;
    private Vector3 lastPosition;

    private void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 dir = hit.point - transform.position;
                dir.y = 0f;

                float distance = dir.magnitude;

                // --- ✅ 너무 가까우면 무시 ---
                if (distance < minMoveDistance)
                {
                    animator.SetFloat(animationSpeed, 0f);
                    return; // 이동 안 함
                }

                dir.Normalize();

                // --- 회전 ---
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

                // --- 이동 ---
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }

        // --- 속도 계산 ---
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        animator.SetFloat(animationSpeed, speed);
        lastPosition = transform.position;
    }
}
