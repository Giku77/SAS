using System.Collections.Generic;
using UnityEngine;
using System;

public  class SkillManager : MonoBehaviour
{
    public static SkillManager I { get; private set; }

    private readonly List<SkillBase> skills = new List<SkillBase>();

    public float globalCooldownMultiplier = 1f;

    public bool isPaused = false;

    void Awake()
    {
        if (I == null) I = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {

    }
    public void RegisterSkill(SkillBase skill)
    {
        if (skill == null) return;
        if (!skills.Contains(skill)) skills.Add(skill);
    }

    public void UnregisterSkill(SkillBase skill)
    {
        if (skill == null) return;
        skills.Remove(skill);
    }

    void Update()
    {
        if (isPaused) return;

        float dt = Time.deltaTime * globalCooldownMultiplier;

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].UpdateCooldown(dt);
        }
    }

    public void ResetAllCooldowns()
    {
        for (int i = 0; i < skills.Count; i++)
            skills[i].ResetSkill();
    }
}
