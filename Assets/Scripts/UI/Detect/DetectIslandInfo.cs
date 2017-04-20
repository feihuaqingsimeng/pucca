using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Packet;

public class DetectIslandInfo : MonoBehaviour
{
    public  int             IslandIndexNumber;

    public  Text            IslandName;

    public  Image           IslandImage;

    public  GameObject      BubbleObject;
    public  Text            MapCount;
    
    public  GameObject      LockObject;
    public  Text            LockLevel;

    private Color           ColorSpriteGrey = Color.grey;

    public  Button          IslandButton;

    private string          FieldName;
    private Goods_Type      IslandGoodsType;
    private DB_TreasureDetectMap.Schema TreasureMapData;


    private UIDetectPopup   detectPopup;



    void OnEnable()
    {
        IslandButton.onClick.AddListener(OnPressIslandButton);

    }

    void OnDisable()
    {
        IslandButton.onClick.RemoveAllListeners();
    }


    public void InitIslandInfo()
    {
        switch (IslandIndexNumber)
        {
            case 0:
                IslandGoodsType = Goods_Type.TreasureDetectMap_Terrapin;
                FieldName = Languages.ToString(TEXT_UI.ISLAND_TERRAPIN);
                break;
            case 1:
                IslandGoodsType = Goods_Type.TreasureDetectMap_Coconut;
                FieldName = Languages.ToString(TEXT_UI.ISLAND_COCONUT);
                break;
            case 2:
                IslandGoodsType = Goods_Type.TreasureDetectMap_Ice;
                FieldName = Languages.ToString(TEXT_UI.ISLAND_ICE);
                break;
            case 3:
                IslandGoodsType = Goods_Type.TreasureDetectMap_Lake;
                FieldName = Languages.ToString(TEXT_UI.ISLAND_LAKE);
                break;
            case 4:
                IslandGoodsType = Goods_Type.TreasureDetectMap_Black;
                FieldName = Languages.ToString(TEXT_UI.ISLAND_BLACK);
                break;
        }

        IslandName.text = FieldName;

        TreasureMapData = DB_TreasureDetectMap.Query(DB_TreasureDetectMap.Field.Index, IslandIndexNumber + 1);

        if (Kernel.entry.account.level < TreasureMapData.UnlockAccountLevel)
        {
            IslandImage.color = ColorSpriteGrey;
    
            LockObject.SetActive(true);
            LockLevel.text = Languages.ToString(TEXT_UI.ACCOUNT_LEVEL_OPEN, TreasureMapData.UnlockAccountLevel);
        }
        else
        {
            IslandImage.color = Color.white;

            LockObject.SetActive(false);
        }


        BubbleObject.SetActive(true);
        MapCount.text = Kernel.entry.account.GetValue(IslandGoodsType).ToString();

    }




    public void OnPressIslandButton()
    {
        if (Kernel.entry == null || TreasureMapData == null)
            return;

        if (Kernel.entry.account.level < TreasureMapData.UnlockAccountLevel)
            UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.ACCOUNT_LEVEL_INFO, TreasureMapData.UnlockAccountLevel));
        else if (Kernel.entry.account.GetValue(IslandGoodsType) < 1)
            UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.TREASURE_MAP_NONE, FieldName));
        else
        {
            if(detectPopup == null)
                detectPopup = Kernel.uiManager.Get<UIDetectPopup>(UI.DetectPopup, true, false);

            detectPopup.InitDetectPopup(FieldName, Languages.ToString(TEXT_UI.TREASURE_DETECT_INFO, FieldName, 1), TreasureMapData.IslandNum);
            Kernel.uiManager.Open(UI.DetectPopup);
        }
    }







}
