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

    void Awake()
    {
        attackVfxPool = ObjectPool.GetOrCreate(defaultAttackVfxRef.gameObject);
        copydamagedVfx = Instantiate(damagedVfxRef.gameObject, vfxAnchors[(int)VfxType.Damage]);
        skillVfxs = new SkillDef[skillVfxsRef.Length];
        for (int i = 0; i < skillVfxsRef.Length; i++)
        {
            Debug.Log("Instantiate skill VFX: " + skillVfxsRef[i].skillname);
            //skillVfxs[i] = ScriptableObject.CreateInstance<SkillDef>();
            skillVfxs[i] = ScriptableObject.CreateInstance<SkillDef>();
            skillVfxs[i].skillname = skillVfxsRef[i].skillname;
            skillVfxs[i].mp = skillVfxsRef[i].mp;
            skillVfxs[i].coolTime = skillVfxsRef[i].coolTime;
            skillVfxs[i].skillEffect = Instantiate(skillVfxsRef[i].skillEffect, vfxAnchors[(int)VfxType.Skill]);
            skillVfxs[i].skillEffect.gameObject.SetActive(false);
        }
    }

    void OnPlayAttackVfx()
    {
        attackVfxPool.Pop(vfxAnchors[(int)VfxType.Attack].position, Quaternion.identity);
    }
}