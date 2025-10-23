﻿using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Entity
{
    private NavMeshAgent agent;
    private Animator animator;
    //private Slider healthSlider;
    public EnemyData enemyData;
    private EnemyIds enemyId;
    private EnemyAttackType attackType;

    public enum State
    {
        Idle,
        Trace,
        Attack,
        Die
    }
    private enum Type
    {
        Default,
        Speed,
        Heavy
    }

    private static readonly int hashDie = Animator.StringToHash("Die");
    private static readonly int hashTarget = Animator.StringToHash("HasTarget");
    private static readonly int hashSpeed = Animator.StringToHash("Speed");
    private static readonly int hashAttack = Animator.StringToHash("Attack");
    private static readonly int hashAttack2 = Animator.StringToHash("Attack2");

    private State currentState;

    private Transform target;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;

    public float lastAttackTime;
    public float attackDelay = 1.0f;

    private CapsuleCollider capsuleCollider;

    private bool sinking = false;

    private GameObject weapon;

    private GameObject rangedOBJ;

    public State state
    {
        get { return currentState; }
        set
        {
            var prev = currentState;
            currentState = value;
            switch (currentState)
            {
                case State.Idle:
                    animator.SetBool(hashTarget, false);
                    AgentStopSafe(true);
                    break;
                case State.Trace:
                    animator.SetBool(hashTarget, true);
                    animator.SetFloat(hashSpeed, agent.speed);
                    AgentStopSafe(false);
                    break;
                case State.Attack:
                    animator.SetBool(hashTarget, false);
                    AgentStopSafe(true);
                    break;
                case State.Die:
                    animator.SetTrigger(hashDie);
                    AgentStopSafe(true);
                    break;
            }
        }
    }

    public void SetEnemyData(EnemyData data)
    {
        enemyId = data.enemyId;
        maxhealth = data.health;
        health = data.health;
        damage = data.damage;
        attackType = data.attackType;
        weapon = data.attackPrefab;
        speed = data.speed;
    }

    private void FireProjectile()
    {
        Transform muzzle = transform;

        var dir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.forward;

        //var fwdFlat = Vector3.ProjectOnPlane(muzzle.forward, Vector3.up).normalized;

        const float spawnForward = 0.35f;
        //const float spawnUp = 1f;
        var spawnPos = transform.position + dir * spawnForward;
        spawnPos += Vector3.up * 1.5f;

        var rot = Quaternion.LookRotation(dir, Vector3.up);
        //rangedOBJ = Instantiate(weapon, spawnPos, rot);
        if (rangedOBJ == null)
            rangedOBJ = Instantiate(weapon, spawnPos, rot);
        else
        {
            rangedOBJ.transform.SetPositionAndRotation(spawnPos, rot);
            rangedOBJ.SetActive(true);
        }

        var proj = rangedOBJ.GetComponent<Projectile>();

        Vector3 target = GameObject.FindGameObjectWithTag("Player").transform.position + Vector3.up * 1.0f;
        float flightTime = 0.8f; 
        proj.LaunchTo(spawnPos, target, flightTime, owner: transform);
    }

    public void Awake()
    {
        //healthSlider = GetComponentInChildren<Slider>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (enemyData != null)
            SetEnemyData(enemyData);
        //healthSlider.maxValue = maxhealth;
        //healthSlider.value = health;
    }

    private void Update()
    {
        if (sinking)
            transform.Translate(Vector3.down * 2f * Time.deltaTime, Space.World);
        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Trace:
                UpdateTrace();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.Die:
                UpdateDie();
                break;
        }
    }

    private void UpdateDie()
    {
        //Debug.Log("Zombie is dead.");
    }


    private void UpdateAttack()
    {
        if (target == null || (target != null && Vector3.Distance(transform.position, target.position) > attackDist))
        {
            state = State.Trace;
            animator.SetBool(hashAttack, false);
            if (enemyId == EnemyIds.BOSS) animator.SetBool(hashAttack2, false);
            return;
        }
        //transform.LookAt(target);
        var lookPos = target.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        if (Time.time - lastAttackTime > attackDelay)
        {
            lastAttackTime = Time.time;
            if (enemyId == EnemyIds.BOSS)
            {
                int attackAnim = Random.Range(0, 2);
                if (attackAnim == 0)
                    animator.SetBool(hashAttack, true);
                else
                    animator.SetBool(hashAttack2, true);
            }
            else
                animator.SetBool(hashAttack, true);
        }
    }

    public void Hit() 
    {
        float dist = Vector3.Distance(transform.position, target.position);
        if (attackType == EnemyAttackType.MELEE)
        {
            if (target != null)
            {
                Player player = target.GetComponent<Player>();
                if (player != null)
                {
                    player.OnDamage(damage);
                }
            }

        }
        else if (attackType == EnemyAttackType.RANGED && dist > 2)
        {
            FireProjectile();
        }
    }

    private void UpdateTrace()
    {
        if (target == null) { state = State.Idle; return; }

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackDist) { state = State.Attack; return; }
        if (dist > traceDist) { state = State.Idle; return; }

        if (!AgentUsable()) return;

        agent.speed = speed;

        if (NavMesh.SamplePosition(target.position, out var hit, 1f, NavMesh.AllAreas))
            AgentSetDestinationSafe(hit.position);
    }

    private void UpdateIdle()
    {
        target = FindTargetT(traceDist);
        if (target != null && Vector3.Distance(transform.position, target.position) <= traceDist)
        {
            state = State.Trace;
        }
    }

    //protected override void OnEnable()
    //{
    //    base.OnEnable();
    //}

    //protected override void Die()
    //{
    //    base.Die();
    //    EventBus.EnemyDied?.Invoke();
    //}


    public override void OnDamage(int damage)
    {
        //base.OnDamage(damage);
        //Debug.Log($"Enemy OnDamage {damage}, health {health}");
        //healthSlider.value = health;
        //StartCoroutine(bloodEffect(hitPoint));
        OnDamage(damage, transform.position + Vector3.up * 1.0f);
    }

    public void OnDamage(int damage, Vector3 hitPoint)
    {
        base.OnDamage(damage);
        Debug.Log($"Enemy OnDamage {damage}, health {health}");
        //healthSlider.value = health;

        var hf = GetComponent<HitFlash>();
        if (hf) hf.Play();

        if (DamageTextManager.I != null)
            DamageTextManager.I.Show(damage, hitPoint);
    }

    //public void StartSinking()
    //{
    //    if (agent) agent.enabled = false;

    //    var rb = GetComponent<Rigidbody>();
    //    if (rb)
    //    {
    //        rb.isKinematic = true;
    //        rb.detectCollisions = false;
    //    }

    //    sinking = true;
    //    Destroy(gameObject, 5f);
    //}

    private bool AgentUsable()
    {
        return agent && agent.isActiveAndEnabled && agent.isOnNavMesh;
    }

    private void AgentStopSafe(bool stop)
    {
        if (!AgentUsable()) return;
        agent.isStopped = stop;
        if (stop) agent.ResetPath();
    }

    private bool AgentSetDestinationSafe(Vector3 dst)
    {
        if (!AgentUsable()) return false;
        agent.SetDestination(dst);
        return true;
    }

    private bool AgentWarpSafe(Vector3 pos)
    {
        if (!agent || !agent.isActiveAndEnabled) return false;
        if (!NavMesh.SamplePosition(pos, out var hit, 2f, NavMesh.AllAreas)) return false;
        return agent.Warp(hit.position);
    }

    bool inKnockback;

    public void Knockback(Vector3 dir, float force, float duration = 0.18f)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(CoKnockback(dir, force, duration));
    }

    IEnumerator CoKnockback(Vector3 dir, float force, float duration)
    {
        if (inKnockback) yield break;
        inKnockback = true;

        AgentStopSafe(true);
        if (agent)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            var prevKin = rb.isKinematic;
            var prevGrav = rb.useGravity;
            var prevCons = rb.constraints;
            var prevCd = rb.collisionDetectionMode;

            rb.isKinematic = false;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.constraints = prevCons | RigidbodyConstraints.FreezePositionY;

            dir.y = 0f; dir.Normalize();

            rb.AddForce(dir * force, ForceMode.VelocityChange);

            float t = 0f;
            while (t < duration)
            {
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector3.zero;
            rb.constraints = prevCons;
            rb.useGravity = prevGrav;
            rb.isKinematic = prevKin;
            rb.collisionDetectionMode = prevCd;
        }

        if (AgentWarpSafe(transform.position))
        {
            if (agent)
            {
                agent.updatePosition = true;
                agent.updateRotation = true;
            }
            AgentStopSafe(false);
        }
        else
        {
            if (agent) agent.enabled = false;
        }

        inKnockback = false;
    }

    public void Stomp() {}
    protected override void Die()
    {
        //audioSource.PlayOneShot(zombieDie, AudioManager.instance.sfxVolume);
        //base.Die();
        //healthSlider.gameObject.SetActive(false);
        capsuleCollider.enabled = false;
        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        state = State.Die;
        animator.SetTrigger(hashDie);

        Destroy(gameObject, 3f);

        if (enemyId == EnemyIds.BOSS)
        {
            var uimanager = FindFirstObjectByType<UIManager>();
            if (uimanager != null)
            {
                uimanager.ShowEndUI();
            }
        }
    }

    public void ResetEnemy()
    {
        //healthSlider.gameObject.SetActive(true);
        capsuleCollider.enabled = true;
        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
        health = maxhealth;
        //healthSlider.value = health;
        animator.ResetTrigger(hashDie);
        //if (agent && !agent.isOnNavMesh)
        //{
        //    AgentWarpSafe(transform.position);
        //    if (!agent.isOnNavMesh) agent.enabled = false;
        //}
        state = State.Idle;
        //animator.Rebind();
        //animator.Update(0f);
    }


}
