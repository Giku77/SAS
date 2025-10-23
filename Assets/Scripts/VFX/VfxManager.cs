using UnityEngine;

public class VfxManager : MonoBehaviour, IPoolable
{
    [Header("FX: Ref")]
    [SerializeField] private ParticleSystem attackVfx;
    [SerializeField] private ParticleSystem damageVfx;
    [SerializeField] private ParticleSystem skillVfx;

    [Tooltip("0: Attack, 1: Damage, 2: Skill")]
    [SerializeField] private Transform[] vfxAnchors = new Transform[(int)VfxType.Count];

    private GameObject[] vfxs = new GameObject[3];
    private ObjectPool attackVfxPool;
    
    public void OnPoppedFromPool()
    {
       
    }

    public void OnPushedToPool()
    {
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackVfxPool = ObjectPool.GetOrCreate(attackVfx.gameObject);
        vfxs[(int)VfxType.Damage] = Instantiate(damageVfx.gameObject, vfxAnchors[(int)VfxType.Damage]);
        vfxs[(int)VfxType.Skill] = Instantiate(skillVfx.gameObject, vfxAnchors[(int)VfxType.Skill]);
    }

    void OnPlayAttackVfx()
    {
        attackVfxPool.Pop(vfxAnchors[(int)VfxType.Attack].position, Quaternion.identity);
    }
}