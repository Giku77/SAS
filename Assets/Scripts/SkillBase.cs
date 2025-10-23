using UnityEngine;
using System;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected SkillDef skill;
    public SkillName skillName;

    protected float remainingCoolDown = 0f;
    public float RemainingCooldown => remainingCoolDown;

    public Action<float> OnCooldownChanged;
    public Action OnCooldownFinished;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        if (skill == null)
        {
            skill = VfxManager.skillVfxs[(int)skillName];
        }
        remainingCoolDown = 0f;

        SkillManager.I?.RegisterSkill(this);
    }

    protected virtual void OnDestroy()
    {
        SkillManager.I?.UnregisterSkill(this);
    }

    public abstract bool CanCast();
    public abstract void StartCast();
    public abstract void OnCasting();
    public abstract void Execute();
    public virtual void EndCast() { }
    public virtual void CancelCast() { }
    public virtual void ResetSkill()
    {
        remainingCoolDown = 0f;
        OnCooldownChanged?.Invoke(remainingCoolDown);
    }
    public abstract bool HasEnoughResource();
    public abstract void ConsumeResource();

    public virtual void StartCooldown()
    {
        if (skill == null) return;
        remainingCoolDown = skill.coolTime;
        OnCooldownChanged?.Invoke(remainingCoolDown);
    }

    public virtual bool IsOnCooldown()
    {
        return remainingCoolDown > 0f;
    }

    public virtual void UpdateCooldown(float deltaTime)
    {
        if (remainingCoolDown <= 0f) return;

        remainingCoolDown -= deltaTime;
        if (remainingCoolDown <= 0f)
        {
            remainingCoolDown = 0f;
            OnCooldownChanged?.Invoke(remainingCoolDown);
            OnCooldownFinished?.Invoke();
        }
        else
        {
            OnCooldownChanged?.Invoke(remainingCoolDown);
        }
    }
}
