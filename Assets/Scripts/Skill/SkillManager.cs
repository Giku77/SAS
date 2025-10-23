using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillManager : MonoBehaviour
{
    public AudioClip basicBGM;
    public static SkillManager I { get; private set; }

    private readonly List<SkillBase> skills = new List<SkillBase>();

    public float globalCooldownMultiplier = 1f;

    public bool isPaused = false;

    public ResourceBar mpBar;
    public Entity playerEntity;

    void Awake()
    {
        AudioManager.I.PlayBGM(basicBGM, 2f);
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
        if (playerEntity == null)
        {
            Debug.LogWarning("SkillManager: playerEntity is null");
        }
        if (mpBar != null && playerEntity != null)
        {
            mpBar.InitSlider(playerEntity.maxMp);
            mpBar.UpdateSlider(playerEntity.mpValue);

            playerEntity.OnMpChanged += OnPlayerMpChanged;
        }
    }
    private void OnPlayerMpChanged(float curMp)
    {
        if (mpBar != null) mpBar.UpdateSlider(curMp);
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

    void OnDestroy()
    {
        if (playerEntity != null) playerEntity.OnMpChanged -= OnPlayerMpChanged;
    }
}
