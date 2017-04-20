using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class UIBadge : MonoBehaviour
{
    public enum BadgeType
    {
        All, // 전체 (카드, 일반 업적 및 일일 업적, 수상한 상점, 보물 찾기)
        Character, // 카드
        Achieve, // 일반 업적
        DailyAchieve, // 일일 업적
        AchieveAll, // 일반 업적 및 일일 업적
        StrangeShop, // 수상한 상점
        Treasure, // 보물 찾기
        Franchise, //가맹점
    }

    [SerializeField]
    Image m_Image;
    [SerializeField]
    BadgeType m_BadgeType;

    void Reset()
    {
        m_Image = GetComponent<Image>();
    }

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        if (Kernel.entry != null)
        {
            #region Delegates
            switch (m_BadgeType)
            {
                case BadgeType.All:
                    Kernel.entry.character.onChangedNewCardCount += OnChangedNewCardCount;
                    Kernel.achieveManager.onChangedCompleteAchieveList += OnChangedCompleteAchieveList;
                    Kernel.achieveManager.onChangedCompleteDailyAchieveList += OnChangedCompleteDailyAchieveList;
                    Kernel.entry.strangeShop.onChangedNewItemCount += OnChangedNewItemCount;
                    Kernel.entry.treasure.onChangedOpenableTreasureBoxCount += OnChangedOpenableTreasureBoxCount;
                    Kernel.entry.franchise.onChangedOpenableFranchiseRewardCount += OnChangedFranchiseRewardCount;
                    break;
                case BadgeType.Character:
                    Kernel.entry.character.onChangedNewCardCount += OnChangedNewCardCount;
                    break;
                case BadgeType.Achieve:
                    Kernel.achieveManager.onChangedCompleteAchieveList += OnChangedCompleteAchieveList;
                    break;
                case BadgeType.DailyAchieve:
                    Kernel.achieveManager.onChangedCompleteDailyAchieveList += OnChangedCompleteDailyAchieveList;
                    break;
                case BadgeType.AchieveAll:
                    Kernel.achieveManager.onChangedCompleteAchieveList += OnChangedCompleteAchieveList;
                    Kernel.achieveManager.onChangedCompleteDailyAchieveList += OnChangedCompleteDailyAchieveList;
                    break;
                case BadgeType.StrangeShop:
                    Kernel.entry.strangeShop.onChangedNewItemCount += OnChangedNewItemCount;
                    break;
                case BadgeType.Treasure:
                    Kernel.entry.treasure.onChangedOpenableTreasureBoxCount += OnChangedOpenableTreasureBoxCount;
                    break;
                case BadgeType.Franchise:
                    Kernel.entry.franchise.onChangedOpenableFranchiseRewardCount += OnChangedFranchiseRewardCount;
                    break;
            }
            #endregion
            #region Updates
            switch (m_BadgeType)
            {
                case BadgeType.All:
                    UpdateByAllBadgeType();
                    break;
                case BadgeType.Character:
                    UpdateByCharacterBadgeType();
                    break;
                case BadgeType.Achieve:
                    UpdateByAchieveBadgeType();
                    break;
                case BadgeType.DailyAchieve:
                    UpdateByDailyAchieveBadgeType();
                    break;
                case BadgeType.AchieveAll:
                    UpdateByAchieveAllBadgeType();
                    break;
                case BadgeType.StrangeShop:
                    UpdateByStrangeShopBadgeType();
                    break;
                case BadgeType.Treasure:
                    UpdateByTreasureBadgeType();
                    break;
                case BadgeType.Franchise:
                    UpdateByFranchiseBadgeType();
                    break;
            }
            #endregion
        }
    }

    void OnDisable()
    {
        if (Kernel.entry != null)
        {
            #region Delegates
            switch (m_BadgeType)
            {
                case BadgeType.All:
                    Kernel.entry.character.onChangedNewCardCount -= OnChangedNewCardCount;
                    Kernel.achieveManager.onChangedCompleteAchieveList -= OnChangedCompleteAchieveList;
                    Kernel.achieveManager.onChangedCompleteDailyAchieveList -= OnChangedCompleteDailyAchieveList;
                    Kernel.entry.strangeShop.onChangedNewItemCount -= OnChangedNewItemCount;
                    Kernel.entry.treasure.onChangedOpenableTreasureBoxCount -= OnChangedOpenableTreasureBoxCount;
                    Kernel.entry.franchise.onChangedOpenableFranchiseRewardCount -= OnChangedFranchiseRewardCount;
                    break;
                case BadgeType.Character:
                    Kernel.entry.character.onChangedNewCardCount -= OnChangedNewCardCount;
                    break;
                case BadgeType.Achieve:
                    Kernel.achieveManager.onChangedCompleteAchieveList -= OnChangedCompleteAchieveList;
                    break;
                case BadgeType.DailyAchieve:
                    Kernel.achieveManager.onChangedCompleteDailyAchieveList -= OnChangedCompleteDailyAchieveList;
                    break;
                case BadgeType.AchieveAll:
                    Kernel.achieveManager.onChangedCompleteAchieveList -= OnChangedCompleteAchieveList;
                    Kernel.achieveManager.onChangedCompleteDailyAchieveList -= OnChangedCompleteDailyAchieveList;
                    break;
                case BadgeType.StrangeShop:
                    Kernel.entry.strangeShop.onChangedNewItemCount -= OnChangedNewItemCount;
                    break;
                case BadgeType.Treasure:
                    Kernel.entry.treasure.onChangedOpenableTreasureBoxCount -= OnChangedOpenableTreasureBoxCount;
                    break;
                case BadgeType.Franchise:
                    Kernel.entry.franchise.onChangedOpenableFranchiseRewardCount -= OnChangedFranchiseRewardCount;
                    break;
            }
            #endregion
        }
    }

    #region Updates
    bool UpdateByAllBadgeType()
    {
        return m_Image.enabled = (UpdateByCharacterBadgeType()
                                  || UpdateByAchieveAllBadgeType()
                                  || UpdateByStrangeShopBadgeType()
                                  || UpdateByTreasureBadgeType()
                                  || UpdateByFranchiseBadgeType());
    }

    bool UpdateByCharacterBadgeType()
    {
        return m_Image.enabled = (Kernel.entry.character.newCardCount > 0);
    }

    bool UpdateByAchieveBadgeType()
    {
        return m_Image.enabled = (Kernel.achieveManager.completeAchieveCount > 0);
    }

    bool UpdateByDailyAchieveBadgeType()
    {
        return m_Image.enabled = (Kernel.achieveManager.completeDailyAchieveCount > 0);
    }

    bool UpdateByAchieveAllBadgeType()
    {
        return m_Image.enabled = ((Kernel.achieveManager.completeAchieveCount > 0)
                                  || (Kernel.achieveManager.completeDailyAchieveCount > 0));
    }

    bool UpdateByStrangeShopBadgeType()
    {
        return m_Image.enabled = (Kernel.entry.strangeShop.newItemCount > 0);
    }

    bool UpdateByTreasureBadgeType()
    {
        return m_Image.enabled = (Kernel.entry.treasure.openableTreasureBoxCount > 0);
    }

    bool UpdateByFranchiseBadgeType()
    {
        return m_Image.enabled = (Kernel.entry.franchise.rewardCompletCount > 0);
    }
    #endregion

    #region Listeners
    void OnChangedNewCardCount(int newCardCount)
    {
        switch (m_BadgeType)
        {
            case BadgeType.All:
                UpdateByAllBadgeType();
                break;
            case BadgeType.Character:
                UpdateByCharacterBadgeType();
                break;
        }
    }

    void OnChangedCompleteAchieveList(int completeAchieveCount)
    {
        switch (m_BadgeType)
        {
            case BadgeType.All:
                UpdateByAllBadgeType();
                break;
            case BadgeType.Achieve:
                UpdateByAchieveBadgeType();
                break;
            case BadgeType.AchieveAll:
                UpdateByAchieveAllBadgeType();
                break;
        }
    }

    void OnChangedCompleteDailyAchieveList(int completeDailyAchieveCount)
    {
        switch (m_BadgeType)
        {
            case BadgeType.All:
                UpdateByAllBadgeType();
                break;
            case BadgeType.DailyAchieve:
                UpdateByDailyAchieveBadgeType();
                break;
            case BadgeType.AchieveAll:
                UpdateByAchieveAllBadgeType();
                break;
        }
    }

    void OnChangedNewItemCount(int newItemCount)
    {
        switch (m_BadgeType)
        {
            case BadgeType.All:
                UpdateByAllBadgeType();
                break;
            case BadgeType.StrangeShop:
                UpdateByStrangeShopBadgeType();
                break;
        }
    }

    void OnChangedOpenableTreasureBoxCount(int openableTreasureBoxCount)
    {
        switch (m_BadgeType)
        {
            case BadgeType.All:
                UpdateByAllBadgeType();
                break;
            case BadgeType.Treasure:
                UpdateByTreasureBadgeType();
                break;
        }
    }

    void OnChangedFranchiseRewardCount(int rewardCount)
    {
        switch (m_BadgeType)
        {
            case BadgeType.All:
                UpdateByAllBadgeType();
                break;
            case BadgeType.Franchise:
                UpdateByFranchiseBadgeType();
                break;
        }
    }
    #endregion
}
