using UnityEngine;
public class ActiveSkill : SkillBase
{
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

        bool success = playerEntity.TryConsumeMp(skill.mp);
        if (!success)
        {
            Debug.LogWarning($"{skillName} - Not enough MP!");
        }
    }

    public override void Execute()
    {
        if (!CanCast()) return;

        StartCast();

        if (skill.skillEffect != null)
        {
            skill.skillEffect.gameObject.SetActive(true);
            skill.skillEffect.Play();
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
