using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected SkillDef skill;
    [SerializeField] protected Player playerEntity;

    public SkillName skillName;

    protected float remainingCoolDown = 0f;
    public float RemainingCooldown => remainingCoolDown;

    public Action<float> OnCooldownChanged;
    public Action OnCooldownFinished;

    [Header("UI (optional)")]
    public ResourceBar resourceBar;
    protected virtual void Awake()
    {
     
    }

    protected virtual void Start()
    {
        if (skill == null)
        {
            skill = VfxManager.skillVfxs[(int)skillName];
        }

        if (resourceBar != null && skill != null)
        {
            resourceBar.InitSlider(skill.coolTime);
            resourceBar.UpdateSlider(0f);
            OnCooldownChanged += resourceBar.UpdateSlider;
        }

        if (resourceBar != null)
        {
            resourceBar.InitSlider(skill.coolTime);
        }

        remainingCoolDown = 0f;

        SkillManager.I?.RegisterSkill(this);
    }

    protected virtual void OnEnable()
    {
        RebindPlayer();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    protected virtual void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene prev, Scene next)
    {
        RebindPlayer();
    }

    protected virtual void OnDestroy()
    {
        SkillManager.I?.UnregisterSkill(this);
        if (resourceBar != null)
            OnCooldownChanged -= resourceBar.UpdateSlider;
    }

    protected void RebindPlayer()
    {
        var p = FindFirstObjectByType<Player>(FindObjectsInactive.Exclude);
        if (p != null) playerEntity = p;
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
        if (resourceBar != null) resourceBar.UpdateSlider(0f);
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
        remainingCoolDown = Mathf.Max(0f, remainingCoolDown);
        OnCooldownChanged?.Invoke(remainingCoolDown);
        if (remainingCoolDown <= 0f) OnCooldownFinished?.Invoke();
    }
}
