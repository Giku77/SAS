using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected SkillDef skill;
    public SkillName skillName;
    protected float remaingCoolDown;

    // ─── 초기화 & 상태 ───
    public virtual void Start() 
    {
        Initialize();
    }

    //스킬 정보(쿨타임, 범위, 대상 등) 초기화
    public void Initialize()
    {
        skill = VfxManager.skillVfxs[(int)skillName];
    }

    //현재 스킬을 사용할 수 있는 조건 검사 (쿨타임, MP 등)
    public abstract bool CanCast();

    //시전 시작 (모션, 애니메이션 트리거)
    public abstract void StartCast();

    //시전 중 프레임 단위로 호출되는 처리(차징, 타겟 트래킹 등)
    public abstract void OnCasting();

    //스킬 효과 발동 (실제 데미지, 이펙트, 사운드 처리)
    public abstract void Execute();

    //스킬 종료 처리 (상태 초기화, 콜백 등)
    public abstract void EndCast();

    //시전 도중 취소 처리
    public virtual void CancelCast()
    {

    }

    //스킬 상태 완전 초기화 (예외 상황 대비)
    public virtual void ResetSkill()
    {

    }

    // ─── 쿨타임 & 리소스 ───

    //쿨타임 카운트 시작
    public virtual void StartCooldown()
    {
        //if (skill == null) return;
        Debug.Log($"{skill.coolTime}");
        remaingCoolDown = skill.coolTime;
    }

    //현재 쿨타임 중인지 반환
    public virtual bool IsOnCooldown()
    {
        if (remaingCoolDown >= 0) return true;
        return false;
    }

    //프레임마다 쿨타임 감소 처리
    public virtual void UpdateCooldown(float deltaTime)
    {
        remaingCoolDown -= deltaTime;
        if (0 >= remaingCoolDown)
        {
            remaingCoolDown = skill.coolTime;
            Debug.Log("[ActiveSkill: UpdateCooldown] SkillCool is finished");
            return;
        }
    }

    //MP/에너지 등 리소스 조건 검사
    public abstract bool HasEnoughResource();

    //스킬 사용 시 리소스 차감
    public abstract void ConsumeResource();

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("skill key : Z");
            Execute();
        }
    }
}
