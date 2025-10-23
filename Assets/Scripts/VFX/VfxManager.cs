using System;
using System.Linq;
using UnityEngine;

public class VfxManager : MonoBehaviour, IPoolable
{
    [Header("FX: Ref")]
    [SerializeField] private ParticleSystem defaultAttackVfxRef;
    [SerializeField] private ParticleSystem damagedVfxRef;
    [SerializeField] private SkillDef[] skillVfxsRef;

    [Tooltip("0: Attack, 1: Damage, 2: Skill")]
    [SerializeField] private Transform[] vfxAnchors = new Transform[(int)VfxType.Count];

    private ObjectPool attackVfxPool;
    private GameObject copydamagedVfx;
    public static SkillDef[] skillVfxs;
    public void OnPoppedFromPool()
    {
       
    }

    public void OnPushedToPool()
    {
       
    }
    void OnPlayAttackVfx()
    {
        attackVfxPool.Pop(vfxAnchors[(int)VfxType.Attack].position, Quaternion.identity);
    }
    void Awake()
    {
        attackVfxPool = ObjectPool.GetOrCreate(defaultAttackVfxRef.gameObject);
        copydamagedVfx = Instantiate(damagedVfxRef.gameObject, vfxAnchors[(int)VfxType.Damage]);
        CreateSkillVfxsFromRefs();
    }

    void CreateSkillVfxsFromRefs()
    {
        int enumCount = (int)SkillName.Count;
        if (skillVfxs == null || skillVfxs.Length != enumCount)
            skillVfxs = new SkillDef[enumCount];

        for (int i = 0; i < skillVfxsRef.Length; i++)
        {
            var src = skillVfxsRef[i];
            if (src == null)
            {
                Debug.LogWarning($"skillVfxsRef[{i}] is null - skip.");
                continue;
            }

            SkillName name;
            name = src.skillname;

            int idx = (int)name;
            if (idx < 0 || idx >= enumCount)
            {
                Debug.LogWarning($"SkillName index out of range: {name} ({idx}) - skip.");
                continue;
            }

            if (skillVfxs[idx] != null)
            {
                Debug.LogWarning($"skillVfxs[{name}] already has value. Overwriting.");
                continue;
            }

            var clone = ScriptableObject.CreateInstance<SkillDef>();

            clone.skillname = src.skillname;
            clone.mp = src.mp;
            clone.coolTime = src.coolTime;
            clone.loop = src.loop;
            clone.skillKey = src.skillKey;
            clone.damage = src.damage;

            if (src.skillEffect != null)
            {
                Transform parent = vfxAnchors[(int)VfxType.Skill];
                var vfxInstance = Instantiate(src.skillEffect, parent);
                vfxInstance.gameObject.SetActive(false);
                clone.skillEffect = vfxInstance;
            }
            else
            {
                clone.skillEffect = null;
            }

            skillVfxs[idx] = clone;
            Debug.Log($"Assigned skillVfxs[{name}] (index {idx}) from ref[{i}]");
        }
    }
}