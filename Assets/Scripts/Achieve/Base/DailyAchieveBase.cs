using Common.Packet;
using UnityEngine;

public class DailyAchieveBase : MonoBehaviour
{
    #region Variables
    protected int m_AchieveIndex;
    protected int m_AchieveAccumulate;
    protected int m_AchieveGoal;
    #endregion

    #region Properties
    public int achieveIndex
    {
        get
        {
            return m_AchieveIndex;
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

                Kernel.achieveManager.UpdateAchieveBase(m_AchieveIndex, m_AchieveAccumulate, isCompleted);

                if (isCompleted)
                {
                    Kernel.achieveManager.CompleteDailyAchieve(m_AchieveIndex);
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

    public bool Initialize(CDailyAchieve dailyAchieve)
    {
        if (dailyAchieve != null && !dailyAchieve.m_bIsComplete)
        {
            DB_DailyAchieveList.Schema dailyAchieveList = DB_DailyAchieveList.Query(DB_DailyAchieveList.Field.Index, dailyAchieve.m_iAchieveIndex);
            if (dailyAchieveList != null)
            {
                m_AchieveIndex = dailyAchieve.m_iAchieveIndex;
                m_AchieveAccumulate = dailyAchieve.m_iAchieveAccumulatedAmount;
                m_AchieveGoal = dailyAchieveList.Terms_Count;

                return true;
            }
        }

        return false;
    }
}
