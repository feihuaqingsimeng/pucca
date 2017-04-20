using UnityEngine;

public class AchieveBase : MonoBehaviour
{
    #region Variables
    protected int m_AchieveIndex;
    protected int m_AchieveGroup;
    protected byte m_AchieveStep; // 현재 진행 중인 단계, CAchieve.m_byCompleteStep와 의미가 다릅니다.
    protected int m_AchieveAccumulate;
    protected int m_AchieveGoal;
    protected bool m_Invoked; // 임시
    #endregion

    #region Properties
    public int achieveIndex
    {
        get
        {
            return m_AchieveIndex;
        }
    }

    public int achieveGroup
    {
        get
        {
            return m_AchieveGroup;
        }
    }

    public byte achieveStep
    {
        get
        {
            return m_AchieveStep;
        }
    }

    public int achieveAccumulate
    {
        get
        {
            return m_AchieveAccumulate;
        }

        set
        {
            if (value != m_AchieveAccumulate)
            {
                m_AchieveAccumulate = value;

                Kernel.achieveManager.UpdateAchieveBase(m_AchieveIndex, m_AchieveGroup, m_AchieveStep, m_AchieveAccumulate, isCompleted);

                if (!m_Invoked && isCompleted)
                {
                    m_Invoked = true;
                    Kernel.achieveManager.CompleteAchieveBase(m_AchieveIndex, m_AchieveGroup, m_AchieveStep);
                }
            }
        }
    }

    public int achieveGoal
    {
        get
        {
            return m_AchieveGoal;
        }
    }

    public bool isCompleted
    {
        get
        {
            return (m_AchieveAccumulate >= m_AchieveGoal);
        }
    }

    #endregion

    #region MonoBehaviour Life Cycles
    // Use this for initialization

    // Update is called once per frame

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }
    #endregion

    public bool Initialize(int achieveGroup, byte achieveStep, int achieveAccumulate)
    {
        if (achieveStep <= Kernel.entry.achieve.achieveLastStep)
        {
            DB_AchieveList.Schema achieveList = DB_AchieveList.Query(DB_AchieveList.Field.Achieve_Group, achieveGroup,
                                                                     DB_AchieveList.Field.Achieve_Step, achieveStep);
            if (achieveList != null)
            {
                m_AchieveGroup = achieveGroup;
                m_AchieveStep = achieveStep;
                m_AchieveAccumulate = achieveAccumulate;
                m_AchieveIndex = achieveList.Index;
                m_AchieveGoal = achieveList.Terms_COUNT;

                return true;
            }
        }

        return false;
    }
}
