using UnityEngine;
public class ActiveSkill : SkillBase
{
    protected float playerMp = 100f; // TODO: �÷��̾� �����Ϳ� ���ε�

    public override bool CanCast()
    {
        return HasEnoughResource() && !IsOnCooldown();
    }

    public override bool HasEnoughResource()
    {
        if (skill == null) return false;
        return playerMp >= skill.mp;
    }

    public override void ConsumeResource()
    {
        if (skill == null) return;
        playerMp -= skill.mp;
        // TODO: UI ���� ��
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
