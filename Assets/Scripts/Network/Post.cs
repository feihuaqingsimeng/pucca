using UnityEngine;
using System.Collections;
using Common.Packet;
using System.Collections.Generic;
using System.Linq;
using System;

public class Post : Node 
{

    [HideInInspector]
    public float N_REV_POSTLIST_UPDATE_TIME;
    [HideInInspector]
    public int N_NORMAL_POST_REMAINTIME;
    [HideInInspector]
    public int N_NOTICE_POST_REMAINTIME; 
    [HideInInspector]
    public int N_CREATE_ITEM_GROUP_COUNT; 
    

    //** Post List 받기고 새로운 우편이 있는지 체크(Lobby의 new표시를 위한)
    public delegate void OnActivePostCallback(bool newPost);
    public OnActivePostCallback onActivePostCallback;

    //** CreateUIItem After Post Recive 
    public delegate void OnCreateUIItemPostCallback();
    public OnCreateUIItemPostCallback onCreateUIItemPostCallback;

    //** Post One Reward CallBack
    public delegate void OnOpenPostCallback(long sequence);
    public OnOpenPostCallback onOpenPostCallback;

    //** Post All Reward CallBack
    public delegate void OnOpenAllPostCallback(List<CPostBox> listRemainPostBox);
    public OnOpenAllPostCallback onOpenAllPostCallback;

    //** Reward 후 Open RewardPopup CallBack
    public delegate void OnOpenPoupCallback(List<CReceivedGoods> sequence, eReceivePopupType rewardType);
    public OnOpenPoupCallback onOpenPoupCallback;

    //** 보유중인 우편 데이터
    public Dictionary<long, CPostBox> m_dicPost = new Dictionary<long, CPostBox>();

    public  long        m_lastPostSequence;

    //** PostTotalCount
    public int          m_nTotalCount;

    //** 새로운 우편이 있는지
    private bool  m_bNewPostExist;
    public  bool  newPostExist
    {
        get
        {
            return m_bNewPostExist;
        }
        set
        {
            if(value)
                newPostCheckExist = value;

            m_bNewPostExist = value;
        }
    }

    //** 새로운 우편을 볼 것이 있는지
    private bool m_bNewPostCheckExist;
    public  bool newPostCheckExist
    {
        get
        {
            return m_bNewPostCheckExist;
        }
        set
        {
            m_bNewPostCheckExist = value;

            if (onActivePostCallback != null)
                onActivePostCallback(value);
        }
    }

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_POST_BOX_LIST_ACK>(REV_PACKET_CG_READ_POST_BOX_LIST_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_RECEIVE_POST_BOX_ACK>(REV_PACKET_CG_GAME_RECEIVE_POST_BOX_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_RECEIVE_POST_ALL_ACK>(REV_PACKET_CG_GAME_RECEIVE_POST_ALL_ACK);

        return base.OnCreate();
    }

    public void PostInit()
    {
        if (Kernel.entry != null)
        {
            N_REV_POSTLIST_UPDATE_TIME = Kernel.entry.data.GetValue<float>(Const_IndexID.Const_Post_Refresh_Cycle_Sec);
            N_NORMAL_POST_REMAINTIME = Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Post_Keep_Day);
            N_NOTICE_POST_REMAINTIME = Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Post_Notice_Keep_Day);
            N_CREATE_ITEM_GROUP_COUNT = Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Post_List);
        }
    }

    //** 우편 List 업데이트를 위한 델리게이트 연결 (Post 팝업 창이 떴을때 Update를 안하기 위함)
    public void LinkUpdate(bool linked)
    {
        if (linked)
        {
            if (Kernel.packetRequestIterator)
            {
                REQ_PACKET_CG_READ_POST_BOX_LIST_SYN();
                Kernel.packetRequestIterator.AddPacketRequestInfo<PACKET_CG_READ_POST_BOX_LIST_SYN>(N_REV_POSTLIST_UPDATE_TIME, PACKET_CG_READ_POST_BOX_LIST_SYN);
            }
        }
        else
        {
            Kernel.packetRequestIterator.RemovePacketRequestInfo<PACKET_CG_READ_POST_BOX_LIST_SYN>();
        }
    }

    //** 우편 Data 추가하기
    private void AddPostData(List<CPostBox> postData)
    {
        if (postData == null && postData.Count == 0)
            return;

        if (m_dicPost == null)
            m_dicPost = new Dictionary<long, CPostBox>();

        bool isNewData = false;

        // Add Data
        for (int i = 0; i < postData.Count; i++)
        {
            if (m_dicPost.ContainsKey(postData[i].m_Sequence))
            {
                m_dicPost[postData[i].m_Sequence] = postData[i];
                continue;
            }

            m_dicPost.Add(postData[i].m_Sequence, postData[i]);
            isNewData = true;
        }

        // Last PostSequence
        m_lastPostSequence = postData.Count > 0 ? postData[postData.Count - 1].m_Sequence : m_lastPostSequence;

        // dicData Sorting
        SetOrderByRecent();

        // NewCheck
        newPostExist = isNewData;

        if (onCreateUIItemPostCallback != null)
            onCreateUIItemPostCallback();
    }

    //** 최신순으로 Sorting하기.
    private void SetOrderByRecent()
    {
        var orderDescending = from time in m_dicPost orderby time.Value.m_iRegTime descending select time;
        m_dicPost = orderDescending.ToDictionary(p => p.Key, p => p.Value);
    }

    //** 해당 우편 지우기
    public void RemovePost(long sequence)
    {
        if (m_dicPost == null)
            return;

        if (m_dicPost.ContainsKey(sequence))
            m_dicPost.Remove(sequence);
    }

    //** Data 찾기
    public CPostBox FindPostData(long sequence)
    {
        if (m_dicPost == null)
            return null;

        return m_dicPost.ContainsKey(sequence) ? m_dicPost[sequence] : null;
    }

    public CPostBox FindFirstPostData()
    {
        var first = m_dicPost.First();
        return first.Value;
    }

    public PACKET_CG_READ_POST_BOX_LIST_SYN PACKET_CG_READ_POST_BOX_LIST_SYN()
    {
        return new PACKET_CG_READ_POST_BOX_LIST_SYN()
        {
            m_PostSequence = m_lastPostSequence,
        };
    }

    #region REQ

    //** Post List 정보 요청
    public void REQ_PACKET_CG_READ_POST_BOX_LIST_SYN()
    {
        
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_POST_BOX_LIST_SYN()
        {
            m_PostSequence = m_lastPostSequence
        }, false);
    }

    //** One Post Reward 정보 요청
    public void REQ_PACKET_CG_GAME_RECEIVE_POST_BOX_SYN(long sequence, Common.Util.ePostType postType)
    {
        if (!m_dicPost.ContainsKey(sequence))
            return;

        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_RECEIVE_POST_BOX_SYN()
        {
            m_BoxSequence = sequence,
            m_ePostType = postType
        });
    }

    //** All Post Reward 정보 요청
    public void REQ_PACKET_CG_GAME_RECEIVE_POST_ALL_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_RECEIVE_POST_ALL_SYN());
    }

    #endregion

    #region REV

    //** Post List 정보 받기
    private void REV_PACKET_CG_READ_POST_BOX_LIST_ACK(PACKET_CG_READ_POST_BOX_LIST_ACK packet)
    {
        m_nTotalCount = packet.m_iTotalPostCount;
        AddPostData(packet.m_PostBoxList);
    }

    //** One Post Reward 정보 받기
    private void REV_PACKET_CG_GAME_RECEIVE_POST_BOX_ACK(PACKET_CG_GAME_RECEIVE_POST_BOX_ACK packet)
    {
        if (packet.m_SoulList != null)
        {
            for (int i = 0; i < packet.m_SoulList.Count; i++)
            {
                CSoulInfo soulInfo = packet.m_SoulList[i];
                Kernel.entry.character.UpdateSoulInfo(soulInfo);
            }
        }

        if (packet.m_CardList != null)
        {
            for (int i = 0; i < packet.m_CardList.Count; i++)
            {
                CCardInfo cardInfo = packet.m_CardList[i];
                Kernel.entry.character.UpdateCardInfo(cardInfo);
            }
        }

        List<CReceivedGoods> reciveGoods = new List<CReceivedGoods>();

        if (reciveGoods != null)
            reciveGoods.Add(packet.m_ReceivedGoods);

        if (packet.m_ePostType == Common.Util.ePostType.Goods)
        {
            Kernel.entry.account.SetValue(packet.m_ReceivedGoods.m_eGoodsType, packet.m_ReceivedGoods.m_iTotalAmount);

            if (onOpenPoupCallback != null)
                onOpenPoupCallback(reciveGoods, eReceivePopupType.RT_ONE);
        }
        else if (packet.m_ePostType == Common.Util.ePostType.Card)
        {
            UIStrangeShopDirector strangeShopDirector = Kernel.uiManager.Get<UIStrangeShopDirector>(UI.StrangeShopDirector, true, false);
            if (strangeShopDirector != null)
            {
                if (packet.m_CardList != null)
                {
                    strangeShopDirector.SetOnlyAct(packet.m_ReceivedCardInfo, packet.m_iReceivedCardCount);
                    Kernel.uiManager.Open(UI.StrangeShopDirector);
                }
            }
        }
        else if (packet.m_ePostType == Common.Util.ePostType.RandomBox)
        {
            Kernel.entry.account.SetValue(Common.Util.eGoodsType.Gold, packet.m_iTotalGold);

            UIChestDirector chestDirector = Kernel.uiManager.Get<UIChestDirector>(UI.ChestDirector, true, false);
            if (chestDirector != null)
            {
                CPostBox boxData = FindPostData(packet.m_BoxSequence);

                string skeletonName = "";

                if (boxData != null)
                {
                    DB_BoxGet.Schema boxGet = DB_BoxGet.Query(DB_BoxGet.Field.Index, (int)boxData.m_eGoodsType);

                    if (boxGet == null)
                    {
                        DB_Package_BoxGet.Schema packageBoxGet = DB_Package_BoxGet.Query(DB_Package_BoxGet.Field.Index, (int)boxData.m_eGoodsType);

                        if (packageBoxGet == null)
                        {
                            //이벤트 박스
                        }
                        else
                            skeletonName = packageBoxGet.Box_IdentificationName;
                    }
                    else
                        skeletonName = boxGet.Box_IdentificationName;


                    chestDirector.SetReward(0, packet.m_iEarnGold, packet.m_BoxResultList, skeletonName);
                    Kernel.uiManager.Open(UI.ChestDirector);
                    chestDirector.DirectionByCoroutine();
                }
            }
        }

        if (onOpenPostCallback != null)
            onOpenPostCallback(packet.m_BoxSequence);
    }

    //** All Post Reward 정보 받기
    private void REV_PACKET_CG_GAME_RECEIVE_POST_ALL_ACK(PACKET_CG_GAME_RECEIVE_POST_ALL_ACK packet)
    {
        for (int i = 0; i < packet.m_ReceivedGoodsList.Count; i++)
            Kernel.entry.account.SetValue(packet.m_ReceivedGoodsList[i].m_eGoodsType, packet.m_ReceivedGoodsList[i].m_iTotalAmount);

        // 새로 정비
        m_dicPost.Clear();
        for(int i = 0; i < packet.m_PostBoxList.Count; i++)
        {
            CPostBox postBox = packet.m_PostBoxList[i];
            m_dicPost.Add(postBox.m_Sequence, postBox);
        }
        m_nTotalCount = packet.m_PostBoxList.Count;

        if (onOpenAllPostCallback != null)
            onOpenAllPostCallback(packet.m_PostBoxList);

        if (onOpenPoupCallback != null)
            onOpenPoupCallback(packet.m_ReceivedGoodsList, eReceivePopupType.RT_MORE);
    }

    #endregion
}
