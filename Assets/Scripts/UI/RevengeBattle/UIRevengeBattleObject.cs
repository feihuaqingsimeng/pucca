using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevengeBattleObject : MonoBehaviour
{
    private TextLevelMaxEffect m_LevelMaxEffect;

    public List<UIMiniCharCard> m_MiniCharCardList;
    public Button m_UserInfoButton;
    public UIMiniCharCard m_LeaderMiniCharCard;
    public Text m_LevelText;
    public Text m_NameText;
    public Text m_TimeText;
    public Button m_RevengeButton;
    public Text m_RevengeButtonText;
    public Text m_HeartText;
    public Image m_CompleteImage;
    RectTransform m_RectTransform;

    public RectTransform rectTransform
    {
        get
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = transform as RectTransform;
            }

            return m_RectTransform;
        }
    }

    long m_Sequence;
    long m_AID;

    void Awake()
    {
        m_UserInfoButton.onClick.AddListener(OnUserInfoButtonClick);
        m_RevengeButton.onClick.AddListener(OnRevengeButtonClick);

        if (Kernel.entry != null)
        {
            m_HeartText.text = (-Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Revenge_Enter_Price)).ToString();
        }

        if (m_LevelText != null)
            m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit);
    }

    // Use this for initialization

    // Update is called once per frame

    void OnUserInfoButtonClick()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.REQ_PACKET_CG_READ_DETAIL_USER_INFO_SYN(m_AID);
        }
    }

    void OnRevengeButtonClick()
    {
        if (Kernel.entry != null)
        {
            CRevengeMatchInfo revengeMatchInfo = Kernel.entry.revengeBattle.FindRevengeMatchInfo(m_Sequence);
            if (revengeMatchInfo != null)
            {
                if (revengeMatchInfo.m_bIsRevenge)
                {
                    NetworkEventHandler.OnNetworkException(Result_Define.eResult.ALREADY_REVENGED_MATCH);
                }
                else
                {
                    UIDeckEditPopup deckEditPopup = Kernel.uiManager.Get<UIDeckEditPopup>(UI.DeckEditPopup, true, false);
                    if (deckEditPopup != null)
                    {
                        deckEditPopup.SetComposition(UIDeckEditPopup.Composition.Confirm_Edit);
                        deckEditPopup.sequence = m_Sequence;
                        Kernel.uiManager.Open(UI.DeckEditPopup);
                    }
                }
            }
            else Debug.LogError(string.Format("CRevengeMatchInfo could not be found. (sequence : {0})", m_Sequence));
        }
    }

    public void SetRevengeMatchInfo(CRevengeMatchInfo revengeMatchInfo)
    {
        if (revengeMatchInfo != null)
        {
            m_Sequence = revengeMatchInfo.m_Sequence;
            m_AID = revengeMatchInfo.m_Aid;

            if (!string.IsNullOrEmpty(revengeMatchInfo.m_sDeckInfo))
            {
                BattleLog battleLog;
                if (BattleLogUtility.TryGetBattleLog(revengeMatchInfo.m_sDeckInfo, out battleLog))
                {
                    for (int i = 0; i < m_MiniCharCardList.Count; i++)
                    {
                        // NullRef
                        long cid = battleLog.m_DeckData.m_CardCidList[i];
                        CCardInfo cardInfo = battleLog.m_CardInfoList.Find(item => item.m_Cid == cid);
                        if (cardInfo != null)
                        {
                            m_MiniCharCardList[i].SetCardInfo(cardInfo);
                        }
                    }

                    for (int i = 0; i < m_MiniCharCardList.Count; i++)
                    {
                        // NullRefExcpt 처리
                        m_MiniCharCardList[i].SetCardInfo(battleLog.m_CardInfoList[i]);
                    }
                }
                else Debug.LogError(string.Format("Failed to get BattleLog from string. ({0})", revengeMatchInfo.m_sDeckInfo));
            }
            else Debug.LogError("CRevengeMatchInfo.m_sDeckInfo is null.");

            m_LeaderMiniCharCard.SetCardInfo(revengeMatchInfo.m_iLeaderCardIndex);
            
            m_LevelText.text = string.Format("{0}{1}",
                                             Languages.ToString(TEXT_UI.LV),
                                             revengeMatchInfo.m_byLevel);

            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.Value = revengeMatchInfo.m_byLevel;

            m_NameText.text = revengeMatchInfo.m_sUserName;
            m_TimeText.text = Languages.TimeSpanToString(TimeUtility.currentServerTime - TimeUtility.ToDateTime(revengeMatchInfo.m_iBattleTime), true);

            string spriteName = revengeMatchInfo.m_bIsRevenge ? "ui_button_disable" : "ui_button_02";
            m_RevengeButton.image.sprite = TextureManager.GetSprite(SpritePackingTag.Extras, spriteName);
            Color color;
            if (Kernel.colorManager.TryGetColor(revengeMatchInfo.m_bIsRevenge ? "ui_button_disable_shadow" : "ui_button_02_shadow", out color))
            {
                m_RevengeButtonText.GetComponent<Shadow>().effectColor = color;
            }
            if (Kernel.colorManager.TryGetColor(revengeMatchInfo.m_bIsRevenge ? "ui_button_disable_outline" : "ui_button_02_outline", out color))
            {
                m_RevengeButtonText.GetComponent<Outline>().effectColor = color;
            }
            m_CompleteImage.gameObject.SetActive(revengeMatchInfo.m_bIsRevenge);
        }
    }
}
