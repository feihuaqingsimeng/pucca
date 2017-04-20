using System.Collections.Generic;
using UnityEngine.UI;

public class UIRanking : UIObject
{
    public List<Toggle> m_ToggleList;
    public UIRankingList m_UserRankingList;
    public UIRankingOwnInfo m_OwnInfo;
    public UIRankingList m_GuildRankingList;
    public UIRankingOwnGuildInfo m_OwnGuildInfo;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_ToggleList.Count; i++)
        {
            m_ToggleList[i].onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        base.OnEnable();
        OnToggleValueChanged(true);
    }

    void OnToggleValueChanged(bool value)
    {
        if (!value)
        {
            // To avoid successive invoke.
            return;
        }

        for (int i = 0; i < m_ToggleList.Count; i++)
        {
            if (m_ToggleList[i].isOn)
            {
                m_UserRankingList.gameObject.SetActive(i == 0);
                m_OwnInfo.gameObject.SetActive(i == 0);
                m_GuildRankingList.gameObject.SetActive(i != 0);
                m_OwnGuildInfo.gameObject.SetActive(i != 0);

                break;
            }
        }
    }
}
