using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ActiveGroup
{
    public int              m_nGroupNumber;
    public float            m_fGroupWaitTime;
    public List<GameObject> m_listGroupAnim     = new List<GameObject>();
}

public class UIActiveAnimationUtility : MonoBehaviour 
{
    private const float F_ITEMS_ACTIVE_WAIT_TIME    = 0.07f;    //DefultTime

    public List<ActiveGroup> m_listActiveGroup      = new List<ActiveGroup>();
	
    private void Start()
    {
        SetInit();
    }

    //** 외부 코드에서 아이템들을 등록할때.
    public void SetData(int groupNum, GameObject anim, float waitTime = F_ITEMS_ACTIVE_WAIT_TIME)
    {
        ActiveGroup group = m_listActiveGroup.Find(item => groupNum == item.m_nGroupNumber) as ActiveGroup;

        if (group != null)
            group.m_listGroupAnim.Add(anim);
        else
        {
            ActiveGroup newAcitveGroup      = new ActiveGroup();
            newAcitveGroup.m_nGroupNumber   = groupNum;
            newAcitveGroup.m_fGroupWaitTime = waitTime;

            newAcitveGroup.m_listGroupAnim.Add(anim);
            m_listActiveGroup.Add(newAcitveGroup);
        }
    }

    private void SetInit()
    {
        if (m_listActiveGroup == null || m_listActiveGroup.Count <= 0)
            return;

        // 액티브를 모두 꺼준다. -> 액티브를 켜줌과 동시에 애니메이션이 실행됨.
        for (int group = 0; group < m_listActiveGroup.Count; group++)
        {
            ActiveGroup activeGroup = m_listActiveGroup[group];

            if (activeGroup.m_fGroupWaitTime == 0)
                activeGroup.m_fGroupWaitTime = F_ITEMS_ACTIVE_WAIT_TIME;

            for (int groupAnim = 0; groupAnim < activeGroup.m_listGroupAnim.Count; groupAnim++)
            {
                GameObject anim = m_listActiveGroup[group].m_listGroupAnim[groupAnim];

                if (anim == null)
                    continue;

                anim.gameObject.SetActive(false);
            }
        }

        // Sort
        m_listActiveGroup.Sort(delegate(ActiveGroup x, ActiveGroup y) { return x.m_nGroupNumber.CompareTo(y.m_nGroupNumber); });

        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
         // 액티브를 차례대로 켜준다.
        for (int group = 0; group < m_listActiveGroup.Count; group++)
        {
            yield return new WaitForSeconds(m_listActiveGroup[group].m_fGroupWaitTime);

            for (int groupAnim = 0; groupAnim < m_listActiveGroup[group].m_listGroupAnim.Count; groupAnim++)
            {
                GameObject anim = m_listActiveGroup[group].m_listGroupAnim[groupAnim];
                
                if (anim == null)
                    continue;

                anim.gameObject.SetActive(true);
            }
        }
    }
}
