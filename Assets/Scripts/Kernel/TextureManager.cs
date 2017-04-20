using System;
using System.Collections.Generic;
using UnityEngine;
using Common.Util;
using Common.Packet;

public enum SpritePackingTag
{
    // Match up enum with Assets/Resources/Textures/* folder name.
    Portrait,
    Characteristic,
    Chest,
    Extras,
    Guild,
    BuffIcon,
    Lobby,
    Treasure,
    Achieve,
    Franchise,
    Loading,
    Promotion
}

public sealed class TextureManager : Singleton<TextureManager>/*, IResourceManager*/
{
    Dictionary<SpritePackingTag, TextureDictionary> m_TextureDictionaries = new Dictionary<SpritePackingTag, TextureDictionary>();

    // Use this for initialization

    // Update is called once per frame

    #region
    public static Sprite GetEquipSprite(Goods_Type goodsType, byte level)
    {
        string spriteName = string.Empty;
        switch (goodsType)
        {
            case Goods_Type.EquipUpAccessory:
                spriteName = "HP_ring";
                break;
            case Goods_Type.EquipUpArmor:
                spriteName = "shield";
                break;
            case Goods_Type.EquipUpWeapon:
                spriteName = "sword";
                break;
        }
        if (!string.IsNullOrEmpty(spriteName))
        {
            if (level <= 10)
            {
                level = 1;
            }
            else if (level > 10 && level <= 20)
            {
                level = 2;
            }
            else level = 3;
            spriteName = string.Format("{0}{1:00}",
                                       spriteName,
                                       level);
        }

        return !string.IsNullOrEmpty(spriteName) ? GetSprite(SpritePackingTag.Characteristic, spriteName) : null;
    }

    public static Sprite GetGoodsTypeSprite(eGoodsType goodsType)
    {
        DB_Goods.Schema goods = DB_Goods.Query(DB_Goods.Field.Index, goodsType);
        if (goods != null)
        {
            return GetGoodsTypeSprite(goods.Goods_Type);
        }

        return null;
    }

    public static Sprite GetGoodsTypeSprite(Goods_Type goodsType, bool isDoubleCard = false)
    {
        switch (goodsType)
        {
            case Goods_Type.AccountExp:
                return GetSprite(SpritePackingTag.Extras, "Item_Icon_character_exp");
            case Goods_Type.EquipUpAccessory:
                return GetSprite(SpritePackingTag.Characteristic, "HP_ring_Card");
            case Goods_Type.EquipUpArmor:
                return GetSprite(SpritePackingTag.Characteristic, "shield_Card");
            case Goods_Type.EquipUpWeapon:
                return GetSprite(SpritePackingTag.Characteristic, "sword_Card");
            case Goods_Type.SkillUpHealer:
                return isDoubleCard ? GetClassTypeDoubleCardSprite(ClassType.ClassType_Healer) :
                                      GetClassTypeCardSprite(ClassType.ClassType_Healer);
            case Goods_Type.SkillUpHitter:
                return isDoubleCard ? GetClassTypeDoubleCardSprite(ClassType.ClassType_Hitter) :
                                      GetClassTypeCardSprite(ClassType.ClassType_Hitter);

            case Goods_Type.SkillUpKeeper:
                return isDoubleCard ? GetClassTypeDoubleCardSprite(ClassType.ClassType_Keeper) :
                                      GetClassTypeCardSprite(ClassType.ClassType_Keeper);
            case Goods_Type.SkillUpRanger:
                return isDoubleCard ? GetClassTypeDoubleCardSprite(ClassType.ClassType_Ranger) :
                                      GetClassTypeCardSprite(ClassType.ClassType_Ranger);
            case Goods_Type.SkillUpWizard:
                return isDoubleCard ? GetClassTypeDoubleCardSprite(ClassType.ClassType_Wizard) :
                                      GetClassTypeCardSprite(ClassType.ClassType_Wizard);
            case Goods_Type.Gold:
                return GetSprite(SpritePackingTag.Extras, "ui_gold_icon");
            case Goods_Type.Heart:
                return GetSprite(SpritePackingTag.Extras, "ui_heart_icon");
            case Goods_Type.Ruby:
                return GetSprite(SpritePackingTag.Extras, "ui_cash_icon");
            case Goods_Type.StarPoint:
                return GetSprite(SpritePackingTag.Extras, "ui_star_icon");
            case Goods_Type.RevengePoint:
                return GetSprite(SpritePackingTag.Extras, "ui_revenge_point");
            case Goods_Type.GuildPoint:
                return GetSprite(SpritePackingTag.Extras, "ui_gulid_point");
            case Goods_Type.SmilePoint:
                return GetSprite(SpritePackingTag.Extras, "ui_smile_point");
            case Goods_Type.TreasureDetectMap_Terrapin:
                return GetSprite(SpritePackingTag.Extras, "ui_treasure_map_1");
            case Goods_Type.TreasureDetectMap_Coconut:
                return GetSprite(SpritePackingTag.Extras, "ui_treasure_map_2");
            case Goods_Type.TreasureDetectMap_Ice:
                return GetSprite(SpritePackingTag.Extras, "ui_treasure_map_3");
            case Goods_Type.TreasureDetectMap_Lake:
                return GetSprite(SpritePackingTag.Extras, "ui_treasure_map_4");
            case Goods_Type.TreasureDetectMap_Black:
                return GetSprite(SpritePackingTag.Extras, "ui_treasure_map_5");
            case Goods_Type.SweepTicket:
                return GetSprite(SpritePackingTag.Extras, "ui_sweepticket_icon");
            case Goods_Type.GuildExp:
                return GetSprite(SpritePackingTag.Extras, "ui_guild_exp_icon");

        }

        Debug.LogError(goodsType);
        return null;
    }

    public static Sprite GetGuildPatternSprite(int index)
    {
        DB_EmblemPattern.Schema emblemPattern = DB_EmblemPattern.Query(DB_EmblemPattern.Field.Index, index);
        if (emblemPattern != null)
        {
            return GetSprite(SpritePackingTag.Guild, emblemPattern.Icon_Name);
        }

        return null;
    }

    public static Sprite GetPortraitSprite(string identificationName)
    {
        if (!string.IsNullOrEmpty(identificationName))
        {
            return GetSprite(SpritePackingTag.Portrait, string.Format("Icon_{0}", identificationName));
        }

        return null;
    }

    public static Sprite GetPortraitSprite(int cardIndex)
    {
        DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardIndex);
        if (card != null)
        {
            return GetPortraitSprite(card.IdentificationName);
        }

        return null;
    }

    public static Sprite GetGradeTypeBackgroundSprite(Grade_Type gradeType)
    {
        return GetSprite(SpritePackingTag.Characteristic, string.Format("ui_card_{0}_bg", gradeType));
    }

    public static Sprite GetGradeTypeFrameSprite(Grade_Type gradeType)
    {
        return GetSprite(SpritePackingTag.Characteristic, string.Format("ui_card_{0}_frame", gradeType));
    }

    public static Sprite GetGradeTypeSprite(Grade_Type gradeType)
    {
        return GetSprite(SpritePackingTag.Characteristic, gradeType.ToString());
    }

    public static Sprite GetClassTypeIconSprite(ClassType classType)
    {
        return GetSprite(SpritePackingTag.Characteristic, classType.ToString());
    }

    public static Sprite GetClassTypeCardSprite(ClassType classType)
    {
        return GetSprite(SpritePackingTag.Characteristic, string.Format("{0}_Card", classType));
    }

    public static Sprite GetClassTypeDoubleCardSprite(ClassType classType)
    {
        return GetSprite(SpritePackingTag.Characteristic, string.Format("{0}_Double", classType));
    }

    public static Sprite GetAchieveTypeSprite(Achieve_Type achieveType)
    {
        string spriteName = string.Empty;
        switch (achieveType)
        {
            case Achieve_Type.Battle_Type:
                spriteName = "achieve_combat";
                break;
            case Achieve_Type.Character_Type:
                spriteName = "achieve_card";
                break;
            case Achieve_Type.Normal_Type:
                spriteName = "achieve_normal";
                break;
        }

        if (!string.IsNullOrEmpty(spriteName))
        {
            return GetSprite(SpritePackingTag.Achieve, spriteName);
        }

        return null;
    }
    #endregion

    string GetPath(SpritePackingTag spritePackingTag)
    {
        return string.Format("Textures/{0}", spritePackingTag);
    }

    public static Sprite GetSprite(SpritePackingTag spritePackingTag, string spriteName, bool force = true, bool caching = true)
    {
        if (Instance)
        {
            TextureDictionary textureDictionary = Instance.GetTextureDictionary(spritePackingTag);
            if (textureDictionary == null && force)
            {
                textureDictionary = Instance.CreateTextureDictionary(spritePackingTag);
            }
            if (textureDictionary != null)
            {
                return textureDictionary.GetSprite(spriteName, force, caching);
            }
        }

        return null;
    }

    public static void ClearCache()
    {
        foreach (SpritePackingTag item in System.Enum.GetValues(typeof(SpritePackingTag)))
        {
            TextureManager.ClearCache(item);
        }
    }

    public static void ClearCache(SpritePackingTag spritePackingTag)
    {
        if (Instance)
        {
            TextureDictionary textureDictionary = Instance.GetTextureDictionary(spritePackingTag);
            if (textureDictionary != null)
            {
                textureDictionary.ClearCache();
            }
        }
    }

    public static void GenerateCache(SpritePackingTag spritePackingTag)
    {
        if (Instance)
        {
            TextureDictionary textureDictionary = Instance.GetTextureDictionary(spritePackingTag);
            if (textureDictionary == null)
            {
                textureDictionary = Instance.CreateTextureDictionary(spritePackingTag);
            }
            if (textureDictionary != null)
            {
                textureDictionary.GenerateCache();
            }
        }
    }

    TextureDictionary CreateTextureDictionary(SpritePackingTag spritePackingTag)
    {
        TextureDictionary textureDictionary = GetTextureDictionary(spritePackingTag);
        if (textureDictionary == null)
        {
            string path = GetPath(spritePackingTag);
            if (!string.IsNullOrEmpty(path))
            {
                textureDictionary = new TextureDictionary(spritePackingTag, path, StringComparer.Ordinal);

                m_TextureDictionaries.Add(spritePackingTag, textureDictionary);
            }
        }

        return textureDictionary;
    }

    TextureDictionary GetTextureDictionary(SpritePackingTag spritePackingTag)
    {
        if (m_TextureDictionaries.ContainsKey(spritePackingTag))
        {
            return m_TextureDictionaries[spritePackingTag];
        }

        return null;
    }

    sealed class TextureDictionary : Dictionary<string, Sprite>
    {
        string m_AssetPath;

        public TextureDictionary(SpritePackingTag spritePackingTag, string assetPath, IEqualityComparer<string> comparer)
            : base(comparer)
        {
            m_AssetPath = assetPath;
        }

        public Sprite GetSprite(string spriteName, bool force, bool caching)
        {
            Sprite item;
            if (!TryGetValue(spriteName, out item))
            {
                if (force)
                {
                    string combined = string.Format("{0}/{1}", m_AssetPath, spriteName);
                    item = Resources.Load<Sprite>(combined);
                    if (item)
                    {
                        if (caching)
                        {
                            Add(spriteName, item);
                        }
                    }
                    else
                    {
                        Debug.LogError(combined);
                    }
                }
            }

            return item;
        }

        public void GenerateCache()
        {
            ClearCache();

            Sprite[] resources = Resources.LoadAll<Sprite>(m_AssetPath);
            for (int i = 0; i < resources.Length; i++)
            {
                Sprite resource = resources[i];
                if (resource)
                {
                    Add(resource.name, resource);
                }
            }
        }

        public void ClearCache()
        {
            Clear();
            //Resources.UnloadUnusedAssets();
        }
    }
}
