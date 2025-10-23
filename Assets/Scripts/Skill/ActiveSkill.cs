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
        // �ִϸ��̼� �� �ٸ� ȿ�� ó��
        Debug.Log($"[{gameObject.name}] StartCast {skillName}");
    }

    public override void OnCasting()
    {
        // ������ ���� ó��
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
