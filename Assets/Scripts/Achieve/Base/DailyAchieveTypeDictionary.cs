using System;
using System.Collections.Generic;

public /*static*/ class DailyAchieveTypeDictionary : Dictionary<int, Type>
{
    // Key is achieveIndex.
    // Value is DailyAchieveBase.

    public DailyAchieveTypeDictionary()
    {
        this[1] = typeof(DailyAchieveCompleteDailyAchieve); // 일일 업적 완료
        this[2] = typeof(DailyAchieveChestOpen); // 상자 열기
        this[3] = typeof(DailyAchieveGoldCollect); // 골드 획득
        this[4] = typeof(DailyAchievePvPComplete); // 매칭 클리어 (승/패 상관X)
        this[5] = typeof(DailyAchievePvEComplete); // 모험 클리어 (승/패 상관x)
        this[6] = typeof(DailyAchieveCardCollect); // 카드 획득
    }
}
