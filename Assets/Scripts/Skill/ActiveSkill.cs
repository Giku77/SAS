using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class ActiveSkill : SkillBase
{

    private async UniTask addingAttack()
    {
        var player = FindFirstObjectByType<Player>();
        player.addAttackDamage(20);
        await UniTask.Delay(5000);
        player.addAttackDamage(0);
    }

    private async UniTask useHitSkill(float delay, float radius, int dmg)
    {
        await UniTask.Delay((int)(delay * 1000));
        Vector3 center = FindFirstObjectByType<Player>().transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            var enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnDamage(dmg);
            }
        }
    }
    public override bool CanCast()
    {
        return HasEnoughResource() && !IsOnCooldown();
    }

    public override bool HasEnoughResource()
    {
        if (skill == null) return false;
        return playerEntity.mpValue >= skill.mp;
    }

    public override void ConsumeResource()
    {
        if (skill == null || playerEntity == null) return;

        playerEntity.mpValue -= skill.mp;
    }

    private Vector3 GetRallySlot(Vector3 center, int i, float spacing)
    {
        if (i == 0) return center;
        float r = spacing * Mathf.Sqrt(i);
        float theta = i * 2.39996323f;
        return center + new Vector3(Mathf.Cos(theta) * r, 0f, Mathf.Sin(theta) * r);
    }

    protected override void Awake()
    {
        if (playerEntity == null)
        {
            playerEntity = FindFirstObjectByType<Player>();
        }
    }

    public async override void Execute()
    {
        if (!CanCast()) return;

        StartCast();

        var player = FindFirstObjectByType<Player>();
        if (skill.skillEffect != null)
        {
            var fx = Instantiate(skill.skillEffect,
                            player.transform);
            fx.gameObject.SetActive(true);
            fx.Play();
            //skill.skillEffect.gameObject.SetActive(true);
            //skill.skillEffect.Play();

            Vector3 center = player.transform.position;
            if (skill.skillname == SkillName.Heal)
            {
                player.SetHealth(player.getHealth() + (player.getMaxHealth() * 0.1f));
            }

            if (skill.skillname == SkillName.Debuff)
            {
                await useHitSkill(0f, 5f, 10);
            }

            if (skill.skillname == SkillName.Explosion)
            {
                await useHitSkill(1f, 7f, 20);
            }

            if (skill.skillname == SkillName.Meteor)
            {
                await useHitSkill(1f, 7f, 25);
            }

            if (skill.skillname == SkillName.Knife)
            {
                await addingAttack();
            }

            if (skill.skillname == SkillName.MegaBlackHole)
            {
                int i = 0;
                Collider[] hitColliders3 = Physics.OverlapSphere(center, 20f, LayerMask.GetMask("Enemy"));
                foreach (var hitCollider in hitColliders3)
                {
                    var enemy = hitCollider.GetComponent<Enemy>();
                    var agent = hitCollider.GetComponent<NavMeshAgent>();
                    if (enemy != null && agent != null)
                    {
                        Vector3 slot = GetRallySlot(center, i++, 1.2f);
                        if (NavMesh.SamplePosition(slot, out var navHit, 2f, NavMesh.AllAreas))
                            agent.SetDestination(navHit.position);
                        else
                            agent.SetDestination(slot);
                        agent.stoppingDistance = 0.3f;
                        agent.isStopped = false;
                    }
                }
            }

        }

        ConsumeResource();
        StartCooldown();
    }

    public override void StartCast()
    {
        // 애니메이션 및 다른 효과 처리
        Debug.Log($"[{gameObject.name}] StartCast {skillName}");
    }

    public override void OnCasting()
    {
        // 프레임 단위 처리
    }

    public override void EndCast()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(skill.skillKey))
        {
            Execute();
        }
    }
}
