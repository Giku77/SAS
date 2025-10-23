using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDef", menuName = "Scriptable Objects/SkillDef")]
public class SkillDef : ScriptableObject
{
    [Header("Info")]
    public SkillName skillname;
    public bool loop;
    public float coolTime;

    [Header("Ref")]
    public ParticleSystem skillEffect;
}