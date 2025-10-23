using UnityEngine;
public class ActiveSkill : SkillBase
{
    protected float playerMp = 100f; // TODO: 플레이어 데이터와 바인딩

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
        // TODO: UI 갱신 등
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
