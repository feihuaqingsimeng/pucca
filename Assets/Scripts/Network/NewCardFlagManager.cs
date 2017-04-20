using Common.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class Character
{
    #region Variables
    int m_NewCardCount;
    // 연출 완료 목록
    List<int> m_DirectionList = new List<int>();
    #endregion

    #region Properties
    public int newCardCount
    {
        get
        {
            return m_NewCardCount;
        }

        set
        {
            if (m_NewCardCount != value)
            {
                m_NewCardCount = value;

                if (onChangedNewCardCount != null)
                {
                    onChangedNewCardCount(m_NewCardCount);
                }
            }
        }
    }
    #endregion

    #region Delegates
    public delegate void OnChangedNewCardCount(int newCardCount);
    public OnChangedNewCardCount onChangedNewCardCount;
    #endregion

    public void Directed(int cardIndex)
    {
        if (!m_DirectionList.Contains(cardIndex))
        {
            m_DirectionList.Add(cardIndex);
        }
    }

    public int Undirected(int cardIndex)
    {
        return m_DirectionList.RemoveAll(item => int.Equals(item, cardIndex));
    }

    public bool IsDirected(int cardIndex)
    {
        return m_DirectionList.Contains(cardIndex);
    }

    #region REQ
    public void REQ_PACKET_CG_CARD_CLEAR_NEW_FLAG_SYN()
    {
        if (m_CardInfoList != null
            && m_CardInfoList.Count > 0)
        {
            for (int i = 0; i < m_CardInfoList.Count; i++)
            {
                m_CardInfoList[i].m_bIsNew = false;
            }
        }

        newCardCount = 0;

        Kernel.networkManager.WebRequest(new PACKET_CG_CARD_CLEAR_NEW_FLAG_SYN());
    }
    #endregion
}
