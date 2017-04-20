using Common.Packet;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AchieveManager : Singleton<AchieveManager>
{
    #region Variables
    AchieveTypeDictionary m_AchieveTypeDictionary = new AchieveTypeDictionary();
    DailyAchieveTypeDictionary m_DailyAchieveTypeDictionary = new DailyAchieveTypeDictionary();
    // Key is achieveGroup.
    Dictionary<int, AchieveBase> m_AchieveBaseDictionary = new Dictionary<int, AchieveBase>();
    // Key is achieveIndex.
    Dictionary<int, DailyAchieveBase> m_DailyAchieveBaseDictionary = new Dictionary<int, DailyAchieveBase>();
    List<int> m_CompleteAchieveList = new List<int>(); // 완료된 일반 업적 achieveGroup 리스트
    List<int> m_CompleteDailyAchieveList = new List<int>(); // 완료된 일일 업적 achieveIndex 리스트
    #endregion

    #region Properties
    public int completeAchieveCount
    {
        get
        {
            return (m_CompleteAchieveList != null) ? m_CompleteAchieveList.Count : 0;
        }
    }

    public int completeDailyAchieveCount
    {
        get
        {
            return (m_CompleteDailyAchieveList != null) ? m_CompleteDailyAchieveList.Count : 0;
        }
    }
    #endregion

    #region Delegates
    public delegate void OnUpdateAchieveBase(int achieveIndex, int achieveGroup, int achieveStep, int achieveAccumulate, bool isCompleted);
    public OnUpdateAchieveBase onUpdateAchieveBase;

    public delegate void OnCompleteAchieve(int achieveIndex, int achieveGroup, byte achieveStep);
    public OnCompleteAchieve onCompleteAchieve;

    public delegate void OnUpdateDailyAchieveBase(int achieveIndex, int achieveAccumulate, bool isCompleted);
    public OnUpdateDailyAchieveBase onUpdateDailyAchieveBase;

    public delegate void OnCompleteDailyAchieve(int achieveIndex);
    public OnCompleteDailyAchieve onCompleteDailyAchieve;

    public delegate void OnChangedCompleteAchieveList(int completeAchieveCount);
    public OnChangedCompleteAchieveList onChangedCompleteAchieveList;

    public delegate void OnChangedCompleteDailyAchieveList(int completeDailyAchieveCount);
    public OnChangedCompleteDailyAchieveList onChangedCompleteDailyAchieveList;
    #endregion

    #region MonoBehaviour Life Cycles
    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.achieve.onUpdateAchieveList += OnUpdateAchieveList;
            Kernel.entry.achieve.onCompleteAchieveResult += OnCompleteAchieveResult;
            Kernel.entry.achieve.onUpdateDailyAchieveList += OnUpdateDailyAchieveList;
            Kernel.entry.achieve.onCompleteDailyAchieveResult += OnCompleteDailyAchieveResult;
            Kernel.entry.achieve.onUpdateAchieve += OnUpdateAchieve;
            Kernel.entry.achieve.onUpdateDailyAchieve += OnUpdateDailyAchieve;
        }
    }

    void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.achieve.onUpdateAchieveList -= OnUpdateAchieveList;
            Kernel.entry.achieve.onCompleteAchieveResult -= OnCompleteAchieveResult;
            Kernel.entry.achieve.onUpdateDailyAchieveList -= OnUpdateDailyAchieveList;
            Kernel.entry.achieve.onCompleteDailyAchieveResult -= OnCompleteDailyAchieveResult;
            Kernel.entry.achieve.onUpdateAchieve -= OnUpdateAchieve;
            Kernel.entry.achieve.onUpdateDailyAchieve -= OnUpdateDailyAchieve;
        }
    }
    #endregion

    #region Network Events
    void OnUpdateDailyAchieve(CDailyAchieve dailyAchieve)
    {
        if (dailyAchieve != null)
        {
            DailyAchieveBase dailyAchieveBase = FindDailyAchieveBase(dailyAchieve.m_iAchieveIndex);
            if (dailyAchieveBase != null)
            {
                dailyAchieveBase.achieveAccumulate = dailyAchieve.m_iAchieveAccumulatedAmount;
            }
            //else Debug.LogError(string.Format("DailyAchieveBase could not be found. (achieveIndex : {0})", dailyAchieve.m_iAchieveIndex));
        }
    }

    void OnUpdateAchieve(CAchieve achieve)
    {
        if (achieve != null)
        {
            AchieveBase achieveBase = FindAchieveBase(achieve.m_iAchieveGroup);
            if (achieveBase != null)
            {
                achieveBase.achieveAccumulate = achieve.m_iAchieveAccumulatedAmount;
            }
            //else Debug.LogError(string.Format("AchieveBase could not be found. (achieveGroup : {0})", achieve.m_iAchieveGroup));
        }
    }

    void OnCompleteDailyAchieveResult(CDailyAchieve dailyAchieve, CReceivedGoods receivedGoods, bool isLevelUp)
    {
        if (dailyAchieve != null)
        {
            DailyAchieveBase dailyAchieveBase = FindDailyAchieveBase(dailyAchieve.m_iAchieveIndex);
            if (dailyAchieveBase != null)
            {
                // 서버와 achieveAccumulate 동기화 X
                if (!dailyAchieveBase.isCompleted)
                {
                    Debug.LogError("DailyAchieveBase is not completed. (achieveIndex : " + dailyAchieve.m_iAchieveIndex + ").");
                }

                // 일일 업적을 완료하면 DailyAchieveBase를 삭제합니다.
                RemoveDailyAchieveBase(dailyAchieveBase);

                // Functionalize.
                if (m_CompleteDailyAchieveList != null
                    && m_CompleteDailyAchieveList.Remove(dailyAchieve.m_iAchieveIndex))
                {
                    if (onChangedCompleteDailyAchieveList != null)
                    {
                        onChangedCompleteDailyAchieveList(m_CompleteDailyAchieveList.Count);
                    }
                }
            }
            //else Debug.LogError(string.Format("DailyAchieveBase could not be found. (achieveIndex : {0})", dailyAchieve.m_iAchieveIndex));
        }
    }

    void OnUpdateDailyAchieveList(Dictionary<int, CDailyAchieve> dailyAchieveDictionary)
    {
        RemoveAllDailyAchieveBase();

        if (dailyAchieveDictionary != null)
        {
            foreach (var dailyAchieve in dailyAchieveDictionary.Values)
            {
                if (dailyAchieve != null)
                {
                    // 완료된 일일 업적은 DailyAchieveBase를 생성하지 않습니다.
                    if (!dailyAchieve.m_bIsComplete)
                    {
                        CreateDailyAchieveBase(dailyAchieve);
                    }
                }
            }
        }
    }

    void OnCompleteAchieveResult(int achieveGroup, byte achieveCompleteStep, int achieveAccumulate, CReceivedGoods receivedGoods, bool isLevelUp)
    {
        AchieveBase achieveBase = FindAchieveBase(achieveGroup);
        if (achieveBase != null)
        {
            bool initialized = false;
            if (achieveCompleteStep < Kernel.entry.achieve.achieveLastStep)
            {
                initialized = achieveBase.Initialize(achieveGroup, (byte)(achieveCompleteStep + 1), achieveAccumulate);
            }

            // 최종 achieveStep을 완료하면 AchieveBase를 삭제합니다.
            if (!initialized)
            {
                RemoveAchieveBase(achieveBase);
            }

            // Functionalize.
            if (m_CompleteAchieveList != null
                && m_CompleteAchieveList.Remove(achieveGroup))
            {
                if (onChangedCompleteAchieveList != null)
                {
                    onChangedCompleteAchieveList(m_CompleteAchieveList.Count);
                }
            }
        }
        //else Debug.LogError(string.Format("AchieveBase could not be found. (achieveGroup : {0})", achieveGroup));
    }

    void OnUpdateAchieveList(Dictionary<int, CAchieve> achieveDictionary)
    {
        RemoveAllAchieveBase();

        List<DB_AchieveList.Schema> table = DB_AchieveList.instance.schemaList;
        for (int i = 0; i < table.Count; i++)
        {
            DB_AchieveList.Schema schema = table[i];
            if (schema != null)
            {
                int achieveGroup = schema.Achieve_Group;
                AchieveBase achieveBase = FindAchieveBase(achieveGroup);
                // achieveGroup 당 하나의 AchieveBase를 생성합니다.
                if (achieveBase == null)
                {
                    int achieveAccumulate = 0;
                    byte achieveStep = 1;
                    CAchieve achieve;
                    if (achieveDictionary != null && achieveDictionary.TryGetValue(achieveGroup, out achieve))
                    {
                        achieveAccumulate = achieve.m_iAchieveAccumulatedAmount;
                        achieveStep = (byte)(achieve.m_byCompleteStep + 1);
                    }
                    else Debug.LogError(string.Format("CAchieve could not be found. (achieveGroup : {0})", achieveGroup));

                    // 완료된 업적은 AchieveBase를 생성하지 않습니다.
                    if (achieveStep <= Kernel.entry.achieve.achieveLastStep)
                    {
                        achieveBase = CreateAchiveBase(achieveGroup, achieveStep, achieveAccumulate);
                    }
                }
                else continue;
            }
        }
    }
    #endregion

    #region Daily Achieve
    public void UpdateAchieveBase(int achieveIndex, int achieveAccumulate, bool isCompleted)
    {
        if (onUpdateDailyAchieveBase != null)
        {
            onUpdateDailyAchieveBase(achieveIndex, achieveAccumulate, isCompleted);
        }
    }

    public void CompleteDailyAchieve(int achieveIndex)
    {
        DailyAchieveBase dailyAchieveBase = FindDailyAchieveBase(achieveIndex);
        if (dailyAchieveBase != null)
        {
            if (dailyAchieveBase.isCompleted)
            {
                // Functionalize.
                if (m_CompleteDailyAchieveList != null
                    && !m_CompleteDailyAchieveList.Contains(achieveIndex))
                {
                    m_CompleteDailyAchieveList.Add(achieveIndex);

                    if (onChangedCompleteDailyAchieveList != null)
                    {
                        onChangedCompleteDailyAchieveList(m_CompleteDailyAchieveList.Count);
                    }
                }
                //else Debug.LogError(achieveIndex);

                if (onCompleteDailyAchieve != null)
                {
                    onCompleteDailyAchieve(achieveIndex);
                }
            }
        }
        //else Debug.LogError(string.Format("DailyAchieveBase could not be found. (achieveIndex : {0})", achieveIndex));
    }

    void RemoveAllDailyAchieveBase()
    {
        if (m_DailyAchieveBaseDictionary != null && m_DailyAchieveBaseDictionary.Count > 0)
        {
            foreach (var dailyAchieveBase in m_DailyAchieveBaseDictionary.Values)
            {
                if (dailyAchieveBase != null)
                {
                    DestroyImmediate(dailyAchieveBase.gameObject);
                }
            }

            m_DailyAchieveBaseDictionary.Clear();
        }
    }

    bool RemoveDailyAchieveBase(DailyAchieveBase dailyAchieveBase)
    {
        if (dailyAchieveBase != null)
        {
            if (m_DailyAchieveBaseDictionary.ContainsKey(dailyAchieveBase.achieveIndex))
            {
                m_DailyAchieveBaseDictionary.Remove(dailyAchieveBase.achieveIndex);
                DestroyImmediate(dailyAchieveBase.gameObject);

                return true;
            }
            //else Debug.LogError(string.Format("DailyAchieveBase could not be found. (achieveIndex : {0})", dailyAchieveBase.achieveIndex));
        }

        return false;
    }

    public DailyAchieveBase FindDailyAchieveBase(int achieveIndex)
    {
        return m_DailyAchieveBaseDictionary.ContainsKey(achieveIndex) ? m_DailyAchieveBaseDictionary[achieveIndex] : null;
    }

    DailyAchieveBase CreateDailyAchieveBase(CDailyAchieve dailyAchieve)
    {
        DailyAchieveBase dailyAchieveBase = null;
        if (dailyAchieve != null)
        {
            // 완료한 일일 업적은 DailyAchieveBase를 생성하지 않습니다.
            if (!dailyAchieve.m_bIsComplete)
            {
                DB_DailyAchieveList.Schema dailyAchieveList = DB_DailyAchieveList.Query(DB_DailyAchieveList.Field.Index, dailyAchieve.m_iAchieveIndex);
                if (dailyAchieveList != null)
                {
                    Type dailyAchieveType;
                    if (m_DailyAchieveTypeDictionary.TryGetValue(dailyAchieve.m_iAchieveIndex, out dailyAchieveType))
                    {
                        GameObject gameObject = new GameObject(dailyAchieveType.ToString());
                        dailyAchieveBase = gameObject.AddComponent(dailyAchieveType) as DailyAchieveBase;
                        gameObject.transform.SetParent(transform);
                    }
                    //else Debug.LogError(string.Format("Could not be found type of DailyAchieveBase. (achieveIndex : {0])", dailyAchieve.m_iAchieveIndex));

                    if (dailyAchieveBase != null)
                    {
                        if (dailyAchieveBase.Initialize(dailyAchieve))
                        {
                            // Functionalize.
                            if (dailyAchieveBase.isCompleted
                                && m_CompleteDailyAchieveList != null
                                && !m_CompleteDailyAchieveList.Contains(dailyAchieve.m_iAchieveIndex))
                            {
                                m_CompleteDailyAchieveList.Add(dailyAchieve.m_iAchieveIndex);

                                if (onChangedCompleteDailyAchieveList != null)
                                {
                                    onChangedCompleteDailyAchieveList(m_CompleteDailyAchieveList.Count);
                                }
                            }
                            //else Debug.LogError(dailyAchieve.m_iAchieveIndex);

                            m_DailyAchieveBaseDictionary.Add(dailyAchieve.m_iAchieveIndex, dailyAchieveBase);
                        }
                        else
                        {
                            DestroyImmediate(dailyAchieveBase.gameObject);
                            //Debug.LogError(string.Format("Failed to initialize DailyAchieveBase with CDailyAchieve (isComplete : {0}, achieveAccumulatedAmount : {1}, achieveIndex, {2}, completeTime : {3}, sequence : {4}", dailyAchieve.m_bIsComplete, dailyAchieve.m_iAchieveAccumulatedAmount, dailyAchieve.m_iAchieveIndex, dailyAchieve.m_iCompleteTime, dailyAchieve.m_Sequence));
                        }
                    }
                }
            }
        }

        return dailyAchieveBase;
    }
    #endregion

    #region Achieve
    public void UpdateAchieveBase(int achieveIndex, int achieveGroup, int achieveStep, int achieveAccumulate, bool isCompleted)
    {
        if (onUpdateAchieveBase != null)
        {
            onUpdateAchieveBase(achieveIndex, achieveGroup, achieveStep, achieveAccumulate, isCompleted);
        }
    }

    public void CompleteAchieveBase(int achieveIndex, int achieveGroup, byte achieveStep)
    {
        AchieveBase achieveBase = FindAchieveBase(achieveGroup);
        if (achieveBase != null)
        {
            if (achieveBase.isCompleted)
            {
                // 최종 achieveStep이 아니면, 다음 단계로 진행합니다.
                /*
                if (achieveBase.achieveStep < Settings.Achieve.FinalAchieveStep)
                {
                    achieveBase.Initialize(achieveGroup, (byte)(achieveBase.achieveStep + 1), achieveBase.achieveAccumulate);
                }
                */

                // Functionalize.
                if (m_CompleteAchieveList != null
                    && !m_CompleteAchieveList.Contains(achieveGroup))
                {
                    m_CompleteAchieveList.Add(achieveGroup);

                    if (onChangedCompleteAchieveList != null)
                    {
                        onChangedCompleteAchieveList(m_CompleteAchieveList.Count);
                    }
                }
                //else Debug.LogError(achieveGroup);

                if (onCompleteAchieve != null)
                {
                    onCompleteAchieve(achieveIndex, achieveGroup, achieveStep);
                }
            }
        }
        //else Debug.LogError(string.Format("AchieveBase could not be found. (achieveGroup : {0})", achieveGroup));
    }

    void RemoveAllAchieveBase()
    {
        if (m_AchieveBaseDictionary != null && m_AchieveBaseDictionary.Count > 0)
        {
            foreach (var achieveBase in m_AchieveBaseDictionary.Values)
            {
                if (achieveBase != null)
                {
                    DestroyImmediate(achieveBase.gameObject);
                }
            }

            m_AchieveBaseDictionary.Clear();
        }
    }

    /*public*/
    bool RemoveAchieveBase(AchieveBase achieveBase)
    {
        if (achieveBase != null)
        {
            if (m_AchieveBaseDictionary.ContainsKey(achieveBase.achieveGroup))
            {
                m_AchieveBaseDictionary.Remove(achieveBase.achieveGroup);
                DestroyImmediate(achieveBase.gameObject);

                return true;
            }
            //else Debug.LogError(string.Format("AchieveBase could not be found. (achieveGroup : {0})", achieveBase.achieveGroup));
        }

        return false;
    }

    public AchieveBase FindAchieveBase(int achieveGroup)
    {
        return m_AchieveBaseDictionary.ContainsKey(achieveGroup) ? m_AchieveBaseDictionary[achieveGroup] : null;
    }

    AchieveBase CreateAchiveBase(int achieveGroup, byte achieveStep, int achieveAccumulate)
    {
        AchieveBase achieveBase = null;
        // 완료된 업적은 AchieveBase를 생성하지 않습니다.
        if (achieveStep <= Kernel.entry.achieve.achieveLastStep)
        {
            achieveBase = FindAchieveBase(achieveGroup);
            // achieveGroup 당 하나의 AchieveBase를 생성합니다.
            if (achieveBase == null)
            {
                Type achieveType;
                if (m_AchieveTypeDictionary.TryGetValue(achieveGroup, out achieveType))
                {
                    GameObject gameObject = new GameObject(achieveType.ToString());
                    achieveBase = gameObject.AddComponent(achieveType) as AchieveBase;
                    gameObject.transform.SetParent(transform);
                }
                //else Debug.LogError(string.Format("Could not be found type of AchieveBase. (achieveGroup : {0})", achieveGroup));

                if (achieveBase != null)
                {
                    if (achieveBase.Initialize(achieveGroup, achieveStep, achieveAccumulate))
                    {
                        // Functionalize.
                        if (achieveBase.isCompleted
                            && m_CompleteAchieveList != null
                            && !m_CompleteAchieveList.Contains(achieveGroup))
                        {
                            m_CompleteAchieveList.Add(achieveGroup);

                            if (onChangedCompleteAchieveList != null)
                            {
                                onChangedCompleteAchieveList(m_CompleteAchieveList.Count);
                            }
                        }

                        m_AchieveBaseDictionary.Add(achieveGroup, achieveBase);
                    }
                    else
                    {
                        DestroyImmediate(achieveBase.gameObject);
                        //Debug.LogError(string.Format("Failed to initialize AchieveBase. (achieveGroup : {0}, achieveStep : {1}, achieveAccumulate : {2})", achieveGroup, achieveStep, achieveAccumulate));
                    }
                }
            }
        }

        return achieveBase;
    }
    #endregion
}
