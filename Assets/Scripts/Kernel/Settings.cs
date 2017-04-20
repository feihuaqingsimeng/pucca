using UnityEngine;

public static class Settings
{
    public static class Equipment
    {
        public static bool TryGetStat(int cardIndex, byte accessoryLevel, byte weaponLevel, byte armorLevel, out int hp, out int ap, out int dp)
        {
            if (Kernel.entry != null)
            {
                byte maximumLevel = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Equipment_Level_Limit);
                if (accessoryLevel <= maximumLevel && weaponLevel <= maximumLevel && armorLevel <= maximumLevel)
                {
                    DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardIndex);
                    if (card != null)
                    {
                        hp = card.Acc_HP * accessoryLevel;
                        ap = card.Weapon_AP * weaponLevel;
                        dp = card.Armor_DP * armorLevel;

                        return true;
                    }
                }
            }

            hp = ap = dp = 0;

            return false;
        }

        public static bool TryGetLevelUpCondition(Grade_Type gradeType, Goods_Type goodsType, byte level, out int requiredTicket, out int requiredGold)
        {
            requiredTicket = 0;
            requiredGold = 0;

            DB_Const.Schema equipmentLevelLimit = DB_Const.Query(DB_Const.Field.Const_IndexID, Const_IndexID.Const_Equipment_Level_Limit);
            if (equipmentLevelLimit != null)
            {
                byte maximumLevel = (byte)equipmentLevelLimit.Const_Value;
                if (level <= maximumLevel)
                {
                    DB_EquipLevelUp.Schema equipLevelUp = DB_EquipLevelUp.Query(DB_EquipLevelUp.Field.Goods_Type, goodsType);
                    if (equipLevelUp != null)
                    {
                        if (level < 11)
                        {
                            requiredTicket = equipLevelUp.AddCount_10;
                            requiredGold = requiredGold + equipLevelUp.Gold_10;
                        }
                        else if (level < 21 && level > 10)
                        {
                            requiredTicket = equipLevelUp.AddCount_20;
                            requiredGold = requiredGold + equipLevelUp.Gold_20;
                        }
                        else if (level < 31 && level > 20)
                        {
                            requiredTicket = equipLevelUp.AddCount_30;
                            requiredGold = requiredGold + equipLevelUp.Gold_30;
                        }
                        requiredGold = requiredGold + (((int)gradeType - 2) * 10);
                        requiredGold = requiredGold * level;
                        requiredGold = requiredGold + equipLevelUp.BaseGold;

                        return true;
                    }
                }
            }

            return false;
        }
    }

    public static class Card
    {
        public static bool TryGetStat(int cardIndex, byte level, out int hp, out int ap, out int dp)
        {
            DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardIndex);
            if (card != null)
            {
                float value = Mathf.Pow((1f + 0.1f), level - 1);

                hp = Mathf.RoundToInt(card.LvBase_Hp * value);
                ap = Mathf.RoundToInt(card.LvBase_Ap * value);
                dp = Mathf.RoundToInt(card.LvBase_Dp * value);

                return true;
            }

            hp = ap = dp = 0;

            return false;
        }

        public static bool TryGetBattlePower(int cardIndex, byte level, byte accessoryLevel, byte weaponLevel, byte armorLevel, byte skillLevel, out int battlePower)
        {
            int charHP, charAP, charDP;
            if (TryGetStat(cardIndex, level, out charHP, out charAP, out charDP))
            {
                int equipHP, equipAP, equipDP;
                if (Equipment.TryGetStat(cardIndex, accessoryLevel, weaponLevel, armorLevel, out equipHP, out equipAP, out equipDP))
                {
                    DB_Const.Schema battlePowerValueHP = DB_Const.Query(DB_Const.Field.Const_IndexID, Const_IndexID.Const_Battlepower_Value_Hp);
                    DB_Const.Schema battlePowerValueAP = DB_Const.Query(DB_Const.Field.Const_IndexID, Const_IndexID.Const_Battlepower_Value_Ap);
                    DB_Const.Schema battlePowerValueDP = DB_Const.Query(DB_Const.Field.Const_IndexID, Const_IndexID.Const_Battlepower_Value_Dp);
                    DB_Const.Schema battlePowerValueSkill = DB_Const.Query(DB_Const.Field.Const_IndexID, Const_IndexID.Const_Battlepower_Value_Skill);
                    if (battlePowerValueHP != null
                        && battlePowerValueAP != null
                        && battlePowerValueDP != null
                        && battlePowerValueSkill != null)
                    {
                        float value = ((charHP + equipHP) * battlePowerValueHP.Const_Value);
                        value = value + ((charAP + equipAP) * battlePowerValueAP.Const_Value);
                        value = value + ((charDP + equipDP) * battlePowerValueDP.Const_Value);
                        value = value + (skillLevel * battlePowerValueSkill.Const_Value);

                        battlePower = Mathf.RoundToInt(value);

                        return true;
                    }
                }
            }

            battlePower = 0;

            return false;
        }
    }


    public static class Util
    {
        //n자리수 만큼 반올림.
        public static float GetRound(float Value, int n)
        {
            return Mathf.Round(Value * Mathf.Pow(10, n)) / Mathf.Pow(10, n);
        }
    }
}
