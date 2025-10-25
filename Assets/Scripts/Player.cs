using CrusaderUI.Scripts;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public AudioClip basicBGM;

    private static readonly string animationSpeed = "Speed";
    private static readonly string animationAttack = "Attack";
    private static readonly string mainScene = "Main";

    public Camera mainCamera;
    public float moveSpeed = 5f;
    public float rotateSpeed = 10f;
    public float minMoveDistance = 0.5f;

    private float attackDistance = 4f;
    private int attackDamage;
    private int addDamage;

    public bool isGodMode { get; set; } = false;

    private Animator animator;
    private NavMeshAgent agent;

    private float doorMoveTimer = 0f;
    private float doorInterval = 2f;

    private bool doorClick = false;

    private Transform targetEnemy;
    private bool isAttacking = false;

    private float maxHealth;

    private float stoppingDistance = 1.5f;

    public CrusaderUI.Scripts.HPFlowController hpFlowController;

    public GameObject bigSword;
    public GameObject smallSword;

    // LSY: Event when Mp changes
    public float maxMp;
    private float mp;
    public float mpValue
    {
        get => mp;
        set 
        {
            mp = Mathf.Clamp(value, 0f, maxMp);
            mpFlowController.UpdateMpValue(mp);
        }
    }

    public CrusaderUI.Scripts.HPFlowController mpFlowController;

    // LSY: Initialize MP to maxMp
    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        AudioManager.I.PlayBGM(basicBGM, 2f);
        agent.speed = moveSpeed;
        agent.angularSpeed = 0f;
        agent.updateRotation = false;
        agent.stoppingDistance = stoppingDistance;

        if (bigSword) bigSword.SetActive(false);
        if (smallSword) smallSword.SetActive(true);

        health = 400;
        maxHealth = health;
        maxMp = 100f;
        mp = maxMp;

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

        if (targetEnemy != null)
        {
            if (!targetEnemy.gameObject.activeInHierarchy)
            {
                targetEnemy = null;
                agent.stoppingDistance = 0f;
                return;
            }

            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);

            if (distanceToEnemy <= attackDistance)
            {
  
                if (!isAttacking)
                {
                    agent.ResetPath();
                    AttackEnemy();
                }
            }
            else if (!isAttacking)
            {
                agent.SetDestination(targetEnemy.position);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                {
                    float distance = Vector3.Distance(transform.position, navHit.position);

                    if (hit.collider.CompareTag("Enemy"))
                    {
                        agent.stoppingDistance = stoppingDistance;
                        targetEnemy = hit.collider.transform;
                        agent.SetDestination(targetEnemy.position);
                    }
                    else if (hit.collider.CompareTag("FrontDoorway") && !doorClick && transform.position.z > -41)
                    {
                        targetEnemy = null;
                        doorClick = true;
                        Vector3 warpPosition = new Vector3(0f, 0f, -28f);
                        transform.position = warpPosition;
                        agent.Warp(warpPosition);
                        Debug.Log("프론트 도어 클릭");
                    }
                    else if (hit.collider.CompareTag("BackDoorway") && !doorClick && transform.position.z < -25)
                    {
                        targetEnemy = null;
                        doorClick = true;
                        Vector3 warpPosition = new Vector3(0f, 0f, -38f);
                        transform.position = warpPosition;
                        agent.Warp(warpPosition);
                        Debug.Log("백 도어 클릭");
                    }
                    else if (distance > minMoveDistance)
                    {
                        targetEnemy = null;
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
        mpFlowController.SetValue(mp, maxMp);
        hpFlowController.SetValue(health, maxHealth);
        //mpFlowController.UpdateMpValue(mp);
    }

    private void AttackEnemy()
    {
        if (isAttacking) return;

        isAttacking = true;

        animator.SetTrigger(animationAttack);

        Invoke(nameof(ResetAttack), 1f);
    }

    private void ResetAttack()
    {
        isAttacking = false;

        if (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy)
        {
            targetEnemy = null;
        }
    }

    protected override void Die()
    {
        base.Die();
        var Uimanager = FindFirstObjectByType<UIManager>();
        if (Uimanager != null)
        {
            Uimanager.ShowOverUI();
        }
        //SceneManager.LoadScene(mainScene);
    }

    public override void OnDamage(int damage)
    { 
        if (isGodMode) return;
        base.OnDamage(damage);
       // Debug.Log($"Player OnDamage {damage}, health {health}");
        //healthSlider.value = health;

        var hf = GetComponent<HitFlash>();
        if (hf) hf.Play();

        //if (DamageTextManager.I != null)
        //    DamageTextManager.I.Show(damage, hitPoint);
    }

    public void Hit()
    {
        Entity enemyEntity = targetEnemy?.GetComponent<Entity>();
        if (enemyEntity != null)
        {
            attackDamage = UnityEngine.Random.Range(30, 51);
            attackDamage += addDamage;
            enemyEntity.OnDamage(attackDamage);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BigSword"))
        {
            Debug.Log("대검 획득");

            attackDamage = 100;
            attackDistance = 4f;
            stoppingDistance = 3f;

            agent.stoppingDistance = stoppingDistance;

            if (smallSword) smallSword.SetActive(false);
            if (bigSword) bigSword.SetActive(true);

            other.gameObject.SetActive(false);
        }
    }

    public void SetHealth(float value)
    {
        health = (int)Mathf.Clamp(value, 0f, maxHealth);
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public float getHealth()
    {
        return health;
    }

    public void addAttackDamage(int value)
    {
        addDamage = value;
    }
}
