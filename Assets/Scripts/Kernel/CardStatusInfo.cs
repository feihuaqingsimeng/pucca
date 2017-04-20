using UnityEngine;
using System.Collections;

public class CardStatusInfo
{
    public  int     Info_HP;
    public  int     Info_AP;
    public  int     Info_DP;

    public CardStatusInfo(int cardIndex, int level)
    {
        CalculateCardStatus(cardIndex, level, ref Info_HP, ref Info_AP, ref Info_DP);
    }

    public static bool CalculateCardStatus(int CardIndex, int Level, ref int HP, ref int AP, ref int DP)
    {
        DB_Card.Schema DBCardData = DB_Card.Query(DB_Card.Field.Index, CardIndex);
        if (DBCardData != null)
        {
            //장비 데이터 추가.

            //스킬에 따른 증가치 처리.

            HP = DBCardData.LvBase_Hp; //+(DBCardData.LvUp_Hp * Level);
            AP = DBCardData.LvBase_Ap; //+(DBCardData.LvUp_Ap * Level);
            DP = DBCardData.LvBase_Dp; //+(DBCardData.LvUp_Dp * Level);

            return true;
        }

        return false;
    }
}
