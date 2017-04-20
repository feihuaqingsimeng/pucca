using Common.Packet;
using Common.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardEquipInfo : UICardInfoComponent
{
    public List<UICardEquipObject> m_CardEquipObjectList;
    public GameObject m_EquipLevelUpFX;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_CardEquipObjectList.Count; i++)
        {
            m_CardEquipObjectList[i].onToggleValueChangeCallback += OnToggleValueChange;
            m_CardEquipObjectList[i].onAnimationEventCallback += OnAnimationEvent;
        }
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.entry != null)
        {
            Kernel.entry.character.onEquipmentLevelUpCallback += OnEquipmentLevelUp;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Kernel.entry != null)
        {
            Kernel.entry.character.onEquipmentLevelUpCallback -= OnEquipmentLevelUp;
        }
    }

    #region Properties
    public override long cid
    {
        get
        {
            return base.cid;
        }

        set
        {
            base.cid = value;

            CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(m_CID);
            if (cardInfo != null)
            {
                int hp, ap, dp;
                if (Settings.Equipment.TryGetStat(cardInfo.m_iCardIndex,
                                              cardInfo.m_byAccessoryLV,
                                              cardInfo.m_byWeaponLV,
                                              cardInfo.m_byArmorLV,
                                              out hp,
                                              out ap,
                                              out dp))
                {
                    DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardInfo.m_iCardIndex);
                    if (card != null)
                    {
                        for (int i = 0; i < m_CardEquipObjectList.Count; i++)
                        {
                            UICardEquipObject item = m_CardEquipObjectList[i];
                            byte level = 0;
                            int stat = 0, add = 0;
                            switch (item.m_Equipment)
                            {
                                case Goods_Type.EquipUpAccessory:
                                    level = cardInfo.m_byAccessoryLV;
                                    stat = hp;
                                    add = card.Acc_HP;
                                    break;
                                case Goods_Type.EquipUpArmor:
                                    level = cardInfo.m_byArmorLV;
                                    stat = dp;
                                    add = card.Armor_DP;
                                    break;
                                case Goods_Type.EquipUpWeapon:
                                    level = cardInfo.m_byWeaponLV;
                                    stat = ap;
                                    add = card.Weapon_AP;
                                    break;
                            }

                            item.SetValue(cardInfo.m_iCardIndex, level, stat, add);
                        }
                    }
                }

                for (int i = 0; i < m_CardEquipObjectList.Count; i++)
                {
                    if (m_CardEquipObjectList[i].m_Toggle.isOn)
                    {
                        byte level;
                        if (Kernel.entry.character.TryGetEquipmentLevel(cardInfo.m_iCardIndex,
                                                                        m_CardEquipObjectList[i].m_Equipment,
                                                                        out level))
                        {
                            OnToggleValueChange(m_CardEquipObjectList[i].m_Equipment, level);
                        }
                        break;
                    }
                }
            }
        }
    }

    protected override byte maxLevel
    {
        get
        {
            return Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Equipment_Level_Limit);
        }
    }

    protected override bool interactable
    {
        get
        {
            return base.interactable;
        }
        set
        {
            base.interactable = value;

            for (int i = 0; i < m_CardEquipObjectList.Count; i++)
            {
                m_CardEquipObjectList[i].m_Toggle.interactable = value;
            }
        }
    }
    #endregion

    public bool CheckAllEquipMaxLevel()
    {
        for (int i = 0; i < m_CardEquipObjectList.Count; i++)
        {
            UICardEquipObject equip = m_CardEquipObjectList[i];

            if (maxLevel > equip.m_Level)
                return false;
        }

        return true;
    }

    protected override void LevelUp()
    {
        Goods_Type equipment = Goods_Type.None;
        for (int i = 0; i < m_CardEquipObjectList.Count; i++)
        {
            if (m_CardEquipObjectList[i].m_Toggle.isOn)
            {
                equipment = m_CardEquipObjectList[i].m_Equipment;
                break;
            }
        }

        if (equipment != Goods_Type.None)
        {
            eItemType itemType = eItemType.Accessory;
            switch (equipment)
            {
                case Goods_Type.EquipUpAccessory:
                    itemType = eItemType.Accessory;
                    break;
                case Goods_Type.EquipUpArmor:
                    itemType = eItemType.Armor;
                    break;
                case Goods_Type.EquipUpWeapon:
                    itemType = eItemType.Weapon;
                    break;
            }

            Kernel.entry.character.REQ_PACKET_CG_CARD_ITEM_LEVEL_UP_SYN(m_CID, itemType);
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        m_EquipLevelUpFX.SetActive(false);
    }

    void OnEquipmentLevelUp(long cid, eGoodsType goodsType)
    {
        if (m_CID != cid)
        {
            return;
        }

        this.cid = cid;
        /*
        interactable = true;
        return;
        */
        StartCoroutine(Animation());

        for (int i = 0; i < m_CardEquipObjectList.Count; i++)
        {
            DB_Goods.Schema goods = DB_Goods.Query(DB_Goods.Field.Goods_Type, m_CardEquipObjectList[i].m_Equipment);
            if (goods != null)
            {
                if (int.Equals(goods.Index, (int)goodsType))
                {
                    m_CardEquipObjectList[i].Direction();
                    break;
                }
            }
        }
    }

    void OnAnimationEvent(string value)
    {
        if (string.Equals("FX", value))
        {
            for (int i = 0; i < m_CardEquipObjectList.Count; i++)
            {
                if (m_CardEquipObjectList[i].m_Toggle.isOn)
                {
                    UIUtility.SetParent(m_EquipLevelUpFX.transform, m_CardEquipObjectList[i].transform);
                    m_EquipLevelUpFX.SetActive(true);
                }
            }
        }
        else
        {
            m_EquipLevelUpFX.SetActive(false);
            interactable = true;
        }
    }

    IEnumerator Animation()
    {
        interactable = false;

        float deltaTime = 0f, normalizedValue = 0f;
        while (deltaTime <= m_SliderAnimationDuration)
        {
            normalizedValue = Mathf.Clamp01(1f - (float)PennerDoubleAnimation.QuintEaseOut(deltaTime, 0f, 1f, m_SliderAnimationDuration));
            if (float.IsNaN(normalizedValue))
            {
                normalizedValue = 0f;
            }
            m_Slider.normalizedValue = normalizedValue;

            deltaTime = deltaTime + Time.deltaTime;

            yield return 0;
        }

        this.cid = m_CID;
        // To OnAnimationEvent(string)
        //interactable = true;

        yield break;
    }

    void OnToggleValueChange(Goods_Type goodsType, byte level)
    {
        int ticket = Kernel.entry.account.GetValue(goodsType);
        int requiredTicket = 0, requiredGold = 0;
        CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(m_CID);
        if (cardInfo != null)
        {
            DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardInfo.m_iCardIndex);
            if (card != null)
            {
                Settings.Equipment.TryGetLevelUpCondition(card.Grade_Type, goodsType, level, out requiredTicket, out requiredGold);
            }
        }

        this.level = level;
        this.ticket = ticket;
        this.requiredGold = requiredGold;
        this.requiredTicket = requiredTicket;

        m_SliderIconImage.sprite = TextureManager.GetSprite(SpritePackingTag.Characteristic, GetSpriteName(goodsType));
        m_Slider.maxValue = requiredTicket;
        m_Slider.value = ticket;
        UpdateLevelUpButton();
    }

    string GetSpriteName(Goods_Type goodsType)
    {
        switch (goodsType)
        {
            case Goods_Type.EquipUpAccessory:
                return "HP_ring_Card_icon";
            case Goods_Type.EquipUpArmor:
                return "shield_Card_icon";
            case Goods_Type.EquipUpWeapon:
                return "sword_Card_icon";
        }

        return string.Empty;
    }
}
