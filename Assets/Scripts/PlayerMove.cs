using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour
{
    private static readonly string animationSpeed = "Speed";

    public Camera mainCamera;
    public float moveSpeed = 5f;
    public float rotateSpeed = 10f;
    public float minMoveDistance = 0.5f;

    private Animator animator;
    private NavMeshAgent agent;

    private float doorMoveTimer = 0f;
    private float doorInterval = 2f;

    private bool doorClick = false;

    private void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.angularSpeed = 0f;
        agent.updateRotation = false;

        doorClick = false;
    }

    private void Update()
    {
        if (doorClick)
        { 
            doorMoveTimer += Time.deltaTime;
        }

        if (doorMoveTimer > doorInterval)
        {
            doorClick = false;
            doorMoveTimer = 0f;
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                {
                    float distance = Vector3.Distance(transform.position, navHit.position);

                    if (hit.collider.CompareTag("FrontDoorway") && !doorClick && transform.position.z > -41)
                    {
                        doorClick = true;

                        Vector3 warpPosition = new Vector3(0f, 0f, -28f);
                        transform.position = warpPosition;
                        agent.Warp(warpPosition);

                        Debug.Log("프론트 도어 클릭");
                    }

                    if (hit.collider.CompareTag("BackDoorway") && !doorClick && transform.position.z < -25)
                    {
                        doorClick = true;

                        Vector3 warpPosition = new Vector3(0f, 0f, -38f);
                        transform.position = warpPosition;
                        agent.Warp(warpPosition);

                        Debug.Log("백 도어 클릭");
                    }

                    if (distance > minMoveDistance)
                    {
                        agent.SetDestination(navHit.position);
                    }

                }
            }
        }
        else
        {
            agent.ResetPath();
        }

        Vector3 velocity = agent.velocity;
        velocity.y = 0f;
        if (velocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }

        animator.SetFloat(animationSpeed, agent.velocity.magnitude);
    }
}
