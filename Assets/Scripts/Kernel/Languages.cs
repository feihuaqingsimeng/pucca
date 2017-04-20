using Common.Packet;
using System;
using System.Text;
using System.Text.RegularExpressions;

public static class Languages
{
    static StringBuilder m_StringBuilder = new StringBuilder(64);

    public static bool IsAvailableName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            if (name.Length >= 2 && name.Length <= 8)
            {
                if (!Regex.IsMatch(name, @"[^0-9a-zA-z가-힣]"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static string AreaString(byte currentPvPArea)
    {
        m_StringBuilder.Remove(0, m_StringBuilder.Length);

        m_StringBuilder.Append(Languages.ToString(TEXT_UI.CARD_DECK_AREA));
        m_StringBuilder.Append(" ");
        m_StringBuilder.Append(currentPvPArea);

        return m_StringBuilder.ToString();
    }

    public static string LevelString(byte level)
    {
        m_StringBuilder.Remove(0, m_StringBuilder.Length);

        m_StringBuilder.Append(Languages.ToString(TEXT_UI.LV));
        m_StringBuilder.Append(level);

        return m_StringBuilder.ToString();
    }

    public static string MillionFormatter(int value)
    {
        // miilion
        if (value >= 1000000)
        {
            return string.Format("{0:0.##}m", ((float)value / (float)1000000));
        }
        else
        {
            return value.ToString("#,0");
        }
    }

    public static string ToString<T>(T value) where T : IFormattable
    {
        return value.ToString("#,0", null);
    }

    public static string Ordinal(long value)
    {
        if (value > 0)
        {
            switch (value % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }

            switch (value % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        return string.Empty;
    }

    public static string TimerString(TimeSpan value)
    {
        m_StringBuilder.Remove(0, m_StringBuilder.Length);

        if (value.Days > 0)
        {
            return TimeSpanToString(value);
        }
        else
        {
            if (value.TotalSeconds > 0)
            {
                m_StringBuilder.AppendFormat("{0}:{1}:{2}", value.Hours, value.Minutes, value.Seconds);
            }
            else
            {
                m_StringBuilder.Append("00:00:00");
            }
        }

        return m_StringBuilder.ToString();
    }

    public static string TimeSpanToString(TimeSpan value, bool isRemainTime = false)
    {
        m_StringBuilder.Remove(0, m_StringBuilder.Length);

        if (value.Days > 0)
        {
            // *일 *시간
            m_StringBuilder.Append(value.Days);
            m_StringBuilder.Append(Languages.ToString(TEXT_UI.DAY));

            if (value.Hours > 0)
            {
                m_StringBuilder.AppendFormat(" {0}{1}", value.Hours, Languages.ToString(TEXT_UI.HOUR));
            }
        }
        else if (value.Hours > 0)
        {
            // *시간
            m_StringBuilder.AppendFormat("{0}{1}", value.Hours, Languages.ToString(TEXT_UI.HOUR));
        }
        else
        {
            // *분
            if (value.Minutes > 0)
            {
                m_StringBuilder.AppendFormat("{0}{1}", value.Minutes, Languages.ToString(TEXT_UI.MINUTE));
            }
            else
            {
                m_StringBuilder.Append(Languages.ToString(TEXT_UI.JUST));
            }
        }

        // 전
        if (!isRemainTime && value.TotalMinutes >= 1)
        {
            m_StringBuilder.AppendFormat(" {0}", Languages.ToString(TEXT_UI.BEFORE));
        }

        return m_StringBuilder.ToString();
    }

    public static string StringToTEXT_UI(string value, params object[] args)
    {
        try
        {
            TEXT_UI textUI = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), value);
            return ToString(textUI, args);
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }

    public static string ToString(TEXT_UI value, params object[] args)
    {
        DBStr_UI.Schema schema = DBStr_UI.Query(DBStr_UI.Field.TEXT_UI, value);

        if (schema != null)
        {
            return string.Format(schema.StringData, args);
        }

        return value.ToString();
    }

    public static string ToStringBuiltIn(TEXT_UI textUI, params object[] args)
    {
        DBStr_UI_BuiltIn.Schema strUIBuiltIn = DBStr_UI_BuiltIn.Query(DBStr_UI_BuiltIn.Field.TEXT_UI, textUI,
                                                                      DBStr_UI_BuiltIn.Field.LanguageCode, Kernel.languageCode);
        if (strUIBuiltIn != null)
        {
            return string.Format(strUIBuiltIn.String_Data, args);
        }

        return textUI.ToString();
    }

    public static string ToString(LanguageCode value)
    {
        switch (value)
        {
            case LanguageCode.Chinese: return "中文(简体)";
            case LanguageCode.ChineseTraditional: return "中文(繁體)";
            case LanguageCode.English: return "English";
            case LanguageCode.French: return "Français";
            case LanguageCode.German: return "Deutsch";
            case LanguageCode.Indonesian: return "BahasaIndonesia";
            case LanguageCode.Japanese: return "日本語";
            case LanguageCode.Korean: return "한국어";
            case LanguageCode.Portuguese: return "Português";
            case LanguageCode.Russian: return "Русский";
            case LanguageCode.Spanish: return "Español";
            case LanguageCode.Thai: return "ภาษาไทย";
            case LanguageCode.Turkish: return "Türkçe";
            case LanguageCode.Vietnamese: return "TiếngViệt";
            case LanguageCode.Unknown: return "Unknown";
            default: return "";
        }
    }

    public static string ToString(Common.Util.eLoginType loginType)
    {
        switch (loginType)
        {
            case Common.Util.eLoginType.Guest: return Languages.ToString(TEXT_UI.ACCOUNT_GUEST);
            case Common.Util.eLoginType.Google: return Languages.ToString(TEXT_UI.ACCOUNT_GOOGLE_PLUS);
            case Common.Util.eLoginType.Facebook: return Languages.ToString(TEXT_UI.ACCOUNT_SP_FACEBOOK); // Languages.ToString(TEXT_UI.ACCOUNT_FASEBOOK); 스트링 필요
            case Common.Util.eLoginType.Apple: return Languages.ToString(TEXT_UI.ACCOUNT_SP_APPLE); // Languages.ToString(TEXT_UI.ACCOUNT_APPLE); 스트링 필요
            case Common.Util.eLoginType.GuestEditer: return Languages.ToString(TEXT_UI.ACCOUNT_GUESTEDITER); // Languages.ToString(TEXT_UI.GuestEditer); 스트링 필요
            default: return "No Login Type";
        }
    }

    public static string ToString(Goods_Type value)
    {
        switch (value)
        {
            case Goods_Type.AccountExp:
                return ToString(TEXT_UI.GOODS_ACCOUNT_EXP);
            case Goods_Type.EquipUpAccessory:
                return ToString(TEXT_UI.GOODS_EQUIPUP_ACCESSORY);
            case Goods_Type.EquipUpArmor:
                return ToString(TEXT_UI.GOODS_EQUIPUP_ARMOR);
            case Goods_Type.EquipUpWeapon:
                return ToString(TEXT_UI.GOODS_EQUIPUP_WEAPON);
            case Goods_Type.FriendPoint:
                return ToString(TEXT_UI.GOODS_FRIEND_POINT);
            case Goods_Type.Gold:
                return ToString(TEXT_UI.GOODS_GOLD);
            case Goods_Type.GuildExp:
                return ToString(TEXT_UI.GOODS_GUILD_EXP);
            case Goods_Type.GuildPoint:
                return ToString(TEXT_UI.GOODS_GUILD_POINT);
            case Goods_Type.Heart:
                return ToString(TEXT_UI.GOODS_HEART);
            case Goods_Type.RankingPoint:
                return ToString(TEXT_UI.GOODS_RANKING_POINT);
            case Goods_Type.Ruby:
                return ToString(TEXT_UI.GOODS_RUBY);
            case Goods_Type.SkillUpHealer:
                return ToString(TEXT_UI.GOODS_SKILLUP_HEALER);
            case Goods_Type.SkillUpHitter:
                return ToString(TEXT_UI.GOODS_SKILLUP_HITTER);
            case Goods_Type.SkillUpKeeper:
                return ToString(TEXT_UI.GOODS_SKILLUP_KEEPER);
            case Goods_Type.SkillUpRanger:
                return ToString(TEXT_UI.GOODS_SKILLUP_RANGER);
            case Goods_Type.SkillUpWizard:
                return ToString(TEXT_UI.GOODS_SKILLUP_WIZARD);
            case Goods_Type.StarPoint:
                return ToString(TEXT_UI.GOODS_STAR_POINT);
            case Goods_Type.RevengePoint:
                return ToString(TEXT_UI.REVENGE_POINT);
            case Goods_Type.SweepTicket:
                return ToString(TEXT_UI.GOODS_SWEEP_TICKET);
            case Goods_Type.SmilePoint:
                return ToString(TEXT_UI.GOODS_SMILE_POINT);
        }

        return value.ToString();
    }

    public static string ToString(Result_Define.eResult value)
    {
        DBStr_Network.Schema schema = DBStr_Network.Query(DBStr_Network.Field.IndexID, value);
        if (schema != null)
        {
            return schema.StringData;
        }

        return value.ToString();
    }

    public static string FindCharName(int cardIndex)
    {
        DBStr_Character.Schema schema = DBStr_Character.Query(DBStr_Character.Field.Char_Index, cardIndex);
        if (schema != null)
        {
            return schema.StringData;
        }

        return cardIndex.ToString();
    }

    public static string FindSkillName(int skillIndex, SkillType skillType)
    {
        DBStr_Skill.Schema schema = DBStr_Skill.Query(DBStr_Skill.Field.Skill_Index, skillIndex, DBStr_Skill.Field.SkillType, skillType);
        if (schema != null)
        {
            return schema.Skill_Name;
        }

        return skillIndex.ToString();
    }

    public static string FindSkillDesc(int skillIndex)
    {
        DBStr_Skill.Schema schema = DBStr_Skill.Query(DBStr_Skill.Field.Skill_Index, skillIndex);
        if (schema != null)
        {
            return schema.Skill_Desc;
        }

        return skillIndex.ToString();
    }


    public static string GetNumberComma(int Number)
    {
        return Number.ToString("#,0");
    }





    //스킬 설명 받아오기.
    public static string GetSkillExplain(int CardIndex, int SkillLevel, SkillType eSkillType)
    {
        DB_Skill.Schema DBSkill = DB_Skill.Query(DB_Skill.Field.Index, CardIndex, DB_Skill.Field.SkillType, eSkillType);
        if (DBSkill == null)
            return "";

        float SkillValue = (DBSkill.BaseValue + (DBSkill.LvAddValue * (SkillLevel - 1))) * 100.0f;

        DB_Buff.Schema BuffData_1 = null;
        DB_Buff.Schema BuffData_2 = null;
        float BuffValue_1 = 0.0f;
        float BuffValue_2 = 0.0f;

        if (DBSkill.BuffNumber_1 != 0)
        {
            BuffData_1 = DB_Buff.Query(DB_Buff.Field.Index, DBSkill.BuffNumber_1);
            BuffValue_1 = Settings.Util.GetRound((BuffData_1.BaseValue + (BuffData_1.LvAddValue * (SkillLevel - 1))) * 100.0f, 2);
            if (BuffData_1.BUFF_KIND == BUFF_KIND.STATIC_DAMAGE)
                BuffValue_1 *= 0.01f;
        }

        if (DBSkill.BuffNumber_2 != 0)
        {
            BuffData_2 = DB_Buff.Query(DB_Buff.Field.Index, DBSkill.BuffNumber_2);
            BuffValue_2 = Settings.Util.GetRound((BuffData_2.BaseValue + (BuffData_2.LvAddValue * (SkillLevel - 1))) * 100.0f, 2);
            if (BuffData_2.BUFF_KIND == BUFF_KIND.STATIC_DAMAGE)
                BuffValue_2 *= 0.01f;
        }


        DBStr_Skill.Schema SkillStringData = DBStr_Skill.Query(DBStr_Skill.Field.Skill_Index, CardIndex, DBStr_Skill.Field.SkillType, eSkillType);
        if (SkillStringData == null)
            return "";

        return string.Format(SkillStringData.Skill_Desc, (int)SkillValue, BuffValue_1, BuffValue_2);
    }


    //스킬 툴팁 받아오기.
    public static string GetSkillToolTip(int CardIndex, int SkillLevel, SkillType eSkillType)
    {
        DB_Skill.Schema DBSkill = DB_Skill.Query(DB_Skill.Field.Index, CardIndex, DB_Skill.Field.SkillType, eSkillType);
        if (DBSkill == null)
            return "";

        float SkillValue = (DBSkill.BaseValue + (DBSkill.LvAddValue * (SkillLevel - 1))) * 100.0f;

        DB_Buff.Schema BuffData_1 = null;
        DB_Buff.Schema BuffData_2 = null;
        float BuffValue_1 = 0.0f;
        float BuffValue_2 = 0.0f;

        if (DBSkill.BuffNumber_1 != 0)
        {
            BuffData_1 = DB_Buff.Query(DB_Buff.Field.Index, DBSkill.BuffNumber_1);
            BuffValue_1 = Settings.Util.GetRound((BuffData_1.BaseValue + (BuffData_1.LvAddValue * (SkillLevel - 1))) * 100.0f, 2);
        }

        if (DBSkill.BuffNumber_2 != 0)
        {
            BuffData_2 = DB_Buff.Query(DB_Buff.Field.Index, DBSkill.BuffNumber_2);
            BuffValue_2 = Settings.Util.GetRound((BuffData_2.BaseValue + (BuffData_2.LvAddValue * (SkillLevel - 1))) * 100.0f, 2);
        }


        DBStr_Skill.Schema SkillStringData = DBStr_Skill.Query(DBStr_Skill.Field.Skill_Index, CardIndex, DBStr_Skill.Field.SkillType, eSkillType);
        if (SkillStringData == null)
            return "";

        return string.Format(SkillStringData.Skill_Tooltip, (int)SkillValue, BuffValue_1, BuffValue_2);
    }
}
