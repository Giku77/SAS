using Unity.VisualScripting;
using UnityEngine;

public class ActiveSkill : SkillBase
{
    protected float playerMp = 100; //TODO: 임시사용, 추후 플레이어와 바인딩
    public override bool CanCast()
    {
        Debug.Log($"[CanCast() ]{HasEnoughResource()}");
        Debug.Log($"[CanCast() ]{IsOnCooldown()}"); //true;
        if (HasEnoughResource() && !IsOnCooldown())
        return true;
        else return false;
    }

    //public override void CancelCast()
    //{ 
    //    base.CancelCast();
    //    throw new System.NotImplementedException();
    //}

    public override void Start()
    {
        base.Start();
    }
    public override void ConsumeResource()
    {
        Debug.Log($"[ConsumeResource] {skill.mp}");
        playerMp -= skill.mp;
    }

    public override void EndCast()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {

        if (CanCast())
        {
            StartCast();
        }
    }

    public override bool HasEnoughResource()
    {
        return playerMp >= skill.mp;
    }

    public override void OnCasting()
    {
        throw new System.NotImplementedException();
    }

    //public override void ResetSkill()
    //{
    //    base.ResetSkill();
    //    throw new System.NotImplementedException();
    //}

    public override void StartCast()
    {
        StartCooldown();
        ConsumeResource();
 
        skill.skillEffect.gameObject.SetActive(true);
        skill.skillEffect.Play();
    }

    // Unity Event
}
