using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPackageInfoItems : MonoBehaviour 
{
    public enum PackageItemType
    {
        PIT_NONE,
        PIT_CARD,
        PIT_GOODS,
        PIT_BOX,
    }

    private PackageItemType m_ePackageItemType;

    public Image    m_BackGround;
    public Image    m_ItemImg;
    public Text     m_ItemName;
    public Button   m_InfoButton;

    public Sprite   m_BaseBackGround;

    [HideInInspector]
    public RectTransform m_thisRecttrans;

    DB_Package_BoxGet.Schema m_BoxData;
    DB_Card.Schema      m_CardData;


    private void Awake()
    {
        if (m_InfoButton != null)
            m_InfoButton.onClick.AddListener(OnClickInfo);

        if (m_thisRecttrans != null)
            return;

        m_thisRecttrans = this.GetComponent<RectTransform>();
    }

    public void SetUI(DB_ProductPackage.Schema packageItem)
    {
        if (packageItem.Goods_Type == Goods_Type.None)
            return;

        //GoodsType
        m_ePackageItemType  = PackageItemType.PIT_GOODS;
        m_ItemImg.sprite    = packageItem.Goods_Type == Goods_Type.Box || packageItem.Goods_Type == Goods_Type.Card ? null  : TextureManager.GetGoodsTypeSprite(packageItem.Goods_Type);
        m_ItemName.text     = packageItem.Goods_Type == Goods_Type.Box || packageItem.Goods_Type == Goods_Type.Card ? ""    :Languages.ToString(packageItem.Goods_Type) + "X" + packageItem.GoodsValue;
        m_BackGround.sprite = m_BaseBackGround;
        m_ItemImg.SetNativeSize();

        Transform trsIcon = m_ItemImg.GetComponent<Transform>();

        if (trsIcon != null)
            trsIcon.localScale = Vector3.one;

        if (packageItem.Goods_Type == Goods_Type.EquipUpAccessory || packageItem.Goods_Type == Goods_Type.EquipUpArmor || packageItem.Goods_Type == Goods_Type.EquipUpWeapon 
            || packageItem.Goods_Type == Goods_Type.SkillUpHealer || packageItem.Goods_Type == Goods_Type.SkillUpHitter|| packageItem.Goods_Type == Goods_Type.SkillUpKeeper
            || packageItem.Goods_Type == Goods_Type.SkillUpRanger|| packageItem.Goods_Type == Goods_Type.SkillUpWizard)
        {
            if (trsIcon != null)
                trsIcon.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        }

        if (packageItem.Goods_Type == Goods_Type.Gold || packageItem.Goods_Type == Goods_Type.Ruby || packageItem.Goods_Type == Goods_Type.Heart 
            || packageItem.Goods_Type == Goods_Type.StarPoint || packageItem.Goods_Type == Goods_Type.SmilePoint || packageItem.Goods_Type == Goods_Type.RankingPoint 
            || packageItem.Goods_Type == Goods_Type.RevengePoint || packageItem.Goods_Type == Goods_Type.GuildPoint)
        {
            if (trsIcon != null)
                trsIcon.localScale = new Vector3(1.2f, 1.2f, 1.0f);
        }
        if (packageItem.Goods_Type == Goods_Type.Box)
        {
            m_ePackageItemType = PackageItemType.PIT_BOX;

            DB_Package_BoxGet.Schema boxGetData = DB_Package_BoxGet.Query(DB_Package_BoxGet.Field.Index, packageItem.GoodsValue);

            if(boxGetData == null)
                return;

            m_BoxData = boxGetData;

            m_ItemImg.sprite = TextureManager.GetSprite(SpritePackingTag.Chest, boxGetData.Box_IdentificationName);
            m_ItemImg.SetNativeSize();
            m_ItemName.text = Languages.ToString(m_BoxData.TEXT_UI);

            if (trsIcon != null)
                trsIcon.localScale = new Vector3(0.3f, 0.3f, 1.0f);
        }

        if (packageItem.Goods_Type == Goods_Type.Card)
        {
            m_ePackageItemType = PackageItemType.PIT_CARD;

            DB_Card.Schema cardData = DB_Card.Query(DB_Card.Field.Index, packageItem.GoodsValue);

            if (cardData == null)
                return;

            m_CardData = cardData;

            m_ItemImg.sprite = TextureManager.GetPortraitSprite(cardData.Index);
            m_BackGround.sprite = TextureManager.GetGradeTypeBackgroundSprite(cardData.Grade_Type);
            m_ItemImg.SetNativeSize();
            m_ItemName.text = Languages.FindCharName(cardData.Index);

            if (trsIcon != null)
                trsIcon.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        }

        m_InfoButton.gameObject.SetActive(m_ePackageItemType != PackageItemType.PIT_GOODS && m_ePackageItemType != PackageItemType.PIT_NONE);
    }

    private void OnClickInfo()
    {
        if (m_ePackageItemType == PackageItemType.PIT_BOX)
        {
            UIChestInfo chestInfo = Kernel.uiManager.Get<UIChestInfo>(UI.ChestInfo, true, false);

            if (chestInfo == null)
                return;

            if (m_BoxData != null)
                chestInfo.SetUI(m_BoxData.Index, 0, false, true);

            Kernel.uiManager.Open(UI.ChestInfo);
        }

        if (m_ePackageItemType == PackageItemType.PIT_CARD)
        {
            UISecretCardInfo cardInfo = Kernel.uiManager.Get<UISecretCardInfo>(UI.SecretCardInfo, true, false);
            
            if (cardInfo == null)
                return;

            if(m_CardData != null)
                cardInfo.SetCharBaseData(m_CardData);

            Kernel.uiManager.Open(UI.SecretCardInfo);
        }
    }
}
