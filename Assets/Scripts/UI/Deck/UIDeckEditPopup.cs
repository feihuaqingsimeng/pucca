using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDeckEditPopup : UIObject
{
    public enum Composition
    {
        Confirm_Edit,
        Confirm_Cancel,
    }

    public Text m_DescriptionText;
    public List<UIMiniCharCard> m_MiniCharCardList;
    public Button m_EditButton;
    public Button m_ConfirmButton;
    public Button m_CancelButton;

    public long sequence
    {
        get;
        set;
    }

    protected override void Awake()
    {
        base.Awake();

        m_EditButton.onClick.AddListener(OnEditButtonClick);
        m_ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
        m_CancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            CDeckData deckData = Kernel.entry.character.FindMainDeckData();
            if (deckData != null
                && deckData.m_CardCidList != null)
            {
                for (int i = 0; i < m_MiniCharCardList.Count; i++)
                {
                    UIMiniCharCard miniCharCard = m_MiniCharCardList[i];
                    if (miniCharCard != null)
                    {
                        // NullRefExcpt 처리
                        CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(deckData.m_CardCidList[i]);
                        if (cardInfo != null)
                        {
                            miniCharCard.SetCardInfo(cardInfo);
                        }
                        else Debug.LogError(deckData.m_CardCidList[i]);
                    }
                }
            }
        }
    }


    public void SetComposition(Composition composition)
    {
        string description = string.Empty;
        bool cancel = false, edit = false;
        switch (composition)
        {
            case Composition.Confirm_Cancel:
                description = Languages.ToString(TEXT_UI.DECK_EDIT_BATTLE_INFO);
                cancel = true;
                edit = false;
                break;
            case Composition.Confirm_Edit:
                description = Languages.ToString(TEXT_UI.REVENGEBATTLE_INFO);
                cancel = false;
                edit = true;
                break;
        }

        m_DescriptionText.text = description;
        m_CancelButton.gameObject.SetActive(cancel);
        m_EditButton.gameObject.SetActive(edit);
    }

    bool OnBackButtonClick()
    {
        UIDeckEditPopup deckEditPopup = Kernel.uiManager.Get<UIDeckEditPopup>(UI.DeckEditPopup, true, false);
        if (deckEditPopup != null)
        {
            deckEditPopup.SetComposition(Composition.Confirm_Cancel);
            deckEditPopup.sequence = Kernel.entry.revengeBattle.revengeSequence;
            Kernel.uiManager.Open(UI.DeckEditPopup);

            return true;
        }

        return false;
    }

    void OnEditButtonClick()
    {
        if (Kernel.entry != null)
        {
            Kernel.sceneManager.LoadScene(Scene.Deck);
            Kernel.entry.revengeBattle.revengeSequence = sequence;
            UIHUD.instance.onBackButtonClicked = OnBackButtonClick;
        }
    }

    void OnConfirmButtonClick()
    {
        if (Kernel.entry != null)
        {
            // 임시 처리
            if (Kernel.entry.character.isDirty)
            {
                Kernel.entry.character.REQ_PACKET_CG_CARD_EDIT_DECK_INFO_SYN();
            }

            Kernel.entry.revengeBattle.REQ_PACKET_CG_GAME_START_REVENGE_MATCH_SYN(sequence);
        }
    }

    void OnCancelButtonClick()
    {
        if (Kernel.entry != null)
        {
            Kernel.sceneManager.LoadScene(Scene.RevengeBattle);
            UIHUD.instance.onBackButtonClicked = null;
            sequence = 0;
        }
    }
}
