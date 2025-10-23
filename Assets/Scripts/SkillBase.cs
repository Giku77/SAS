using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected SkillDef skill;
    public SkillName skillName;
    protected float remaingCoolDown;

    // ������ �ʱ�ȭ & ���� ������
    public virtual void Start() 
    {
        Initialize();
    }

    //��ų ����(��Ÿ��, ����, ��� ��) �ʱ�ȭ
    public void Initialize()
    {
        skill = VfxManager.skillVfxs[(int)skillName];
    }

    //���� ��ų�� ����� �� �ִ� ���� �˻� (��Ÿ��, MP ��)
    public abstract bool CanCast();

    //���� ���� (���, �ִϸ��̼� Ʈ����)
    public abstract void StartCast();

    //���� �� ������ ������ ȣ��Ǵ� ó��(��¡, Ÿ�� Ʈ��ŷ ��)
    public abstract void OnCasting();

    //��ų ȿ�� �ߵ� (���� ������, ����Ʈ, ���� ó��)
    public abstract void Execute();

    //��ų ���� ó�� (���� �ʱ�ȭ, �ݹ� ��)
    public abstract void EndCast();

    //���� ���� ��� ó��
    public virtual void CancelCast()
    {

    }

    //��ų ���� ���� �ʱ�ȭ (���� ��Ȳ ���)
    public virtual void ResetSkill()
    {

    }

    // ������ ��Ÿ�� & ���ҽ� ������

    //��Ÿ�� ī��Ʈ ����
    public virtual void StartCooldown()
    {
        //if (skill == null) return;
        Debug.Log($"{skill.coolTime}");
        remaingCoolDown = skill.coolTime;
    }

    //���� ��Ÿ�� ������ ��ȯ
    public virtual bool IsOnCooldown()
    {
        if (remaingCoolDown >= 0) return true;
        return false;
    }

    //�����Ӹ��� ��Ÿ�� ���� ó��
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

    //MP/������ �� ���ҽ� ���� �˻�
    public abstract bool HasEnoughResource();

    //��ų ��� �� ���ҽ� ����
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
