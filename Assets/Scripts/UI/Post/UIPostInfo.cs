using UnityEngine;
using System.Collections;
using Common.Packet;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Common.Util;

public class UIPostInfo : MonoBehaviour 
{
    public ePostType        m_ePostType;

    private bool            m_bIsSettingComplet = false;

    public  long            m_lsequence;
    private ePostSendType   m_ePostSendType;
    private DateTime        m_RevTime;
    private DateTime        m_EndTime;
    
    private UIPost          m_Owner;
    private RectTransform   m_RectTrans;

    [SerializeField]
    private Sprite          m_NoticeIcon;
    [SerializeField]
    private Sprite          m_NormalIcon;

    public Text             m_PostName;
    public Text             m_PostDec;
    public Text             m_PostRemainTime;
    public Text             m_ItemCount;
    public Text             m_PostRev;

    public Image            m_PostIcon;
    public Image            m_ItemIcon;
    public Image            m_ItemFrameForChar;
    public Image            m_ItemBackGround;
    public RectTransform    m_ItemIconRectTrans;

    public Sprite           m_BasetBackGroundImg;
    public Sprite           m_ChestBackGroundImg;

    public Button           m_CheckPostButton;

    public UITooltipObject  m_tooltipObject;

    private TimeSpan remainTime
    {
        set
        {
            string space = " ";
            string day      = value.Days > 0                            ? value.Days.ToString()     + Languages.ToString(TEXT_UI.DAY)       + space : "";
            string hour     = (value.Hours > 0) && (value.Days <= 0)    ? value.Hours.ToString()    + Languages.ToString(TEXT_UI.HOUR)      + space : "";
            string minute   = value.Minutes > 0 && (value.Hours <= 0)   ? value.Minutes.ToString()  + Languages.ToString(TEXT_UI.MINUTE)    + space : "";
            string seconds  = value.Seconds > 0 && (value.Minutes <= 0) ? value.Seconds.ToString()  + Languages.ToString(TEXT_UI.SECOND)    + space : "";

            if (value.Days > Kernel.entry.post.N_NORMAL_POST_REMAINTIME || value.Days > Kernel.entry.post.N_NOTICE_POST_REMAINTIME)
            {
                m_PostRemainTime.text = Languages.ToString(TEXT_UI.MAIL_REMAIN_INFINITE);
                return;
            }

            m_PostRemainTime.text = day + hour + minute + seconds + Languages.ToString(TEXT_UI.LATER);

            //m_PostRemainTime.text = Languages.TimeSpanToString(value);
        }
    }

#if UNITY_EITOR
    private void Reset()
    {
        string strTextureBasePath = "Assets/Textures/UI/Post/";

        m_NoticeIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_notice_icon.png");
        m_NormalIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_mail_icon.png");
    }
#endif

    private void Awake()
    {
        m_CheckPostButton.onClick.AddListener(OnClickCheckButton);

        if(m_RectTrans == null)
            m_RectTrans = GetComponent<RectTransform>();

        if (m_ItemIconRectTrans == null)
            m_ItemIconRectTrans = m_ItemIcon.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!m_bIsSettingComplet)
            return;

        if (TimeUtility.currentServerTime > m_EndTime)
        {
            m_Owner.RemoveItem(m_lsequence);
            return;
        }
        remainTime = m_EndTime - m_RevTime;
    }
    
    //** 데이터 세팅
    public void SetData(CPostBox postData, UIPost owner)
    {
        if (postData.m_ePostType == ePostType.RandomBox)
            m_ePostType = Common.Util.ePostType.RandomBox;
        else if (postData.m_ePostType == ePostType.Card)
            m_ePostType = Common.Util.ePostType.Card;
        else
            m_ePostType = Common.Util.ePostType.Goods;

        m_Owner         = owner;
        m_lsequence     = postData.m_Sequence;
        m_ePostSendType = postData.m_ePostSendType;
        m_RevTime       = TimeUtility.ToDateTime(postData.m_iRegTime);
        m_EndTime       = TimeUtility.ToDateTime(postData.m_EndTime);

        SetUI(postData);

        m_bIsSettingComplet = true;
    }

    //** UI 세팅
    private void SetUI(CPostBox postData)
    {
        // Text Setting
        m_ItemCount.text    = postData.m_iGoodsCount.ToString();
        m_PostRev.text      = Languages.ToString(TEXT_UI.GET);

        if (postData.m_ePostSendType == ePostSendType.Rank)
        {
            string titleString = "";
            string DecString = "";

            string[] sprlitTitleString = postData.m_sTitle.Split(':');
            string[] sprlitDecString = postData.m_sMsg.Split(':');

            TEXT_UI strPostName = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), sprlitTitleString[0]);
            TEXT_UI strPostDec  = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), sprlitDecString[0]);

            //** Title 설정 **//
            // 01. 서버에서 {0}값을 주는 경우.
            if (sprlitTitleString.Length > 1)
            {
                titleString = Languages.ToString(strPostName, sprlitTitleString[1]);
            }
            // 02. {0} 없고 Text_UI인 기본.
            else
            {
                titleString = Languages.ToString(strPostName);
            }

            //** Dec 설정 **//
            // 01. 서버에서 {0}값을 주는 경우.
            if(sprlitDecString.Length > 1)
            {
                DecString = Languages.ToString(strPostDec, sprlitDecString[1]);
            }
            // 02. TEXT_UI가 MAIL_REWARD_ACCEPT일 때, 재화를 표현 해줘야하는 경우.
            else if (strPostDec == TEXT_UI.MAIL_REWARD_ACCEPT)
            {
                DB_Goods.Schema goods = DB_Goods.Query(DB_Goods.Field.Index, postData.m_eGoodsType);
                DecString = Languages.ToString(strPostDec, Languages.ToString(goods.Goods_Type));
            }
            // 03. {0} 없고 Text_UI인 기본.
            else
            {
                DecString = Languages.ToString(strPostDec);
            }

            m_PostName.text = titleString;
            m_PostDec.text = DecString;
        }
        // 기본 바로 띄워주기.
        else
        {
            m_PostName.text     = postData.m_sTitle;
            m_PostDec.text      = postData.m_sMsg;
        }

        // ImageSetting
        m_PostIcon.sprite = postData.m_ePostSendType == ePostSendType.Notice ? m_NoticeIcon : m_NormalIcon;
        m_ItemFrameForChar.gameObject.SetActive(m_ePostType == Common.Util.ePostType.Card);

        if (m_ePostType == Common.Util.ePostType.Goods)
        {
            DB_Goods.Schema goodsData = DB_Goods.Query(DB_Goods.Field.Index, (int)postData.m_eGoodsType);

            if (goodsData != null)
            {
                m_ItemIcon.sprite = TextureManager.GetGoodsTypeSprite(goodsData.Goods_Type);
                m_ItemIcon.SetNativeSize();
            }

            m_ItemIconRectTrans.anchoredPosition = new Vector2(0.0f, 3.0f);
            
            m_ItemBackGround.sprite = m_BasetBackGroundImg;

            if (goodsData.Goods_Type == Goods_Type.Gold || goodsData.Goods_Type == Goods_Type.Ruby || goodsData.Goods_Type == Goods_Type.Heart
                || goodsData.Goods_Type == Goods_Type.StarPoint || goodsData.Goods_Type == Goods_Type.SmilePoint || goodsData.Goods_Type == Goods_Type.RankingPoint
                || goodsData.Goods_Type == Goods_Type.RevengePoint || goodsData.Goods_Type == Goods_Type.GuildPoint)
            {
                m_ItemIconRectTrans.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
            }
            else if (goodsData.Goods_Type == Goods_Type.EquipUpAccessory || goodsData.Goods_Type == Goods_Type.EquipUpArmor || goodsData.Goods_Type == Goods_Type.EquipUpWeapon
                    || goodsData.Goods_Type == Goods_Type.SkillUpHealer || goodsData.Goods_Type == Goods_Type.SkillUpHitter || goodsData.Goods_Type == Goods_Type.SkillUpKeeper
                    || goodsData.Goods_Type == Goods_Type.SkillUpRanger || goodsData.Goods_Type == Goods_Type.SkillUpWizard)
            {
                m_ItemIconRectTrans.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            }
            else
                m_ItemIconRectTrans.transform.localScale = Vector3.one;
        }
        else if (m_ePostType == Common.Util.ePostType.Card)
        {
            DB_Card.Schema cardData = DB_Card.Query(DB_Card.Field.Index, (int)postData.m_eGoodsType);

            if (cardData != null)
            {
                m_ItemIcon.sprite = TextureManager.GetPortraitSprite(cardData.Index);
                m_ItemIcon.SetNativeSize();
            }

            m_ItemIconRectTrans.anchoredPosition = new Vector2(0.0f, -5.8f);
            m_ItemIconRectTrans.transform.localScale = new Vector2(0.6f, 0.6f);
            m_ItemFrameForChar.sprite = TextureManager.GetGradeTypeFrameSprite(cardData.Grade_Type);
            m_ItemBackGround.sprite = TextureManager.GetGradeTypeBackgroundSprite(cardData.Grade_Type);
        }
        else if (m_ePostType == Common.Util.ePostType.RandomBox)
        {
            string boxImg = "";

            DB_BoxGet.Schema boxData = DB_BoxGet.Query(DB_BoxGet.Field.Index, (int)postData.m_eGoodsType);

            if (boxData == null)
            {
                DB_Package_BoxGet.Schema packageBoxData = DB_Package_BoxGet.Query(DB_Package_BoxGet.Field.Index, (int)postData.m_eGoodsType);

                if(packageBoxData == null)
                {
                    //EventBox
                }
                else
                    boxImg = packageBoxData.Box_IdentificationName;
            }
            else
                boxImg = boxData.Box_IdentificationName;

            if (boxImg != string.Empty)
            {
                m_ItemIcon.sprite = TextureManager.GetSprite(SpritePackingTag.Chest, boxImg);
                m_ItemIcon.SetNativeSize();
            }
                
            else
                Debug.LogError(string.Format("[UIPostInfo] Box Data is Null (index : {0}", (int)postData.m_eGoodsType));

              m_ItemIconRectTrans.anchoredPosition = new Vector2(0.0f, 3.0f);
              m_ItemIconRectTrans.transform.localScale = new Vector2(0.3f, 0.3f);
              m_ItemBackGround.sprite = m_ChestBackGroundImg;
        }

        // Item Size 조절
        //float minorSize = 0.5f;
        //m_ItemIcon.SetNativeSize();
        //RectTransform iconRect = m_ItemIcon.GetComponent<RectTransform>();
        //if(iconRect != null)
        //    iconRect.sizeDelta = new Vector2(iconRect.rect.width * minorSize, iconRect.rect.height * minorSize);

        if (m_tooltipObject != null)
        {
            DB_Goods.Schema goodsTypeData = DB_Goods.Query(DB_Goods.Field.Index, (int)postData.m_eGoodsType);

            if (goodsTypeData == null)
                return;

            m_tooltipObject.content = Languages.ToString(goodsTypeData.Goods_Type);
        }
    }

    //** 확인 버튼 눌렀을때
    public void OnClickCheckButton()
    {
        // 현재 메일 타입은 재화만 있기 때문에 나중에 기획상 변경되면 사용하기.
        //if (m_PostType == "Notice")
        //    CheckPost();
        //else if (m_PostType == "Normal")
        //    RevItem();


        if (m_Owner == null)
            return;

        m_Owner.OneItemRev(m_lsequence, m_ePostType);
    }
}
