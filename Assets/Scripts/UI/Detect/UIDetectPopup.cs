using Common.Packet;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class UIDetectPopup : UIObject
{
    public  Button      Detect_AR_Button;
    public  Image       ButtonImg_AR;
    public  Text        ButtonText_AR;

    public  Button      Detect_Manual_Button;

    public  Text        ExplainText;



    private int         IslandNumber;
    private string      IslandName;


    protected override void Awake()
    {
        Color shadowEffectColor, outlineEffectColor;
        string ShadowColorName, OutlineColorName;

        if (SystemInfo.supportsGyroscope)    //자이로 지원하면.
        {
            ButtonImg_AR.sprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_03");
            ShadowColorName = "ui_button_03_shadow";
            OutlineColorName = "ui_button_03_outline";
        }
        else
        {
            ButtonImg_AR.sprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_disable");
            ShadowColorName = "ui_button_05_shadow";
            OutlineColorName = "ui_button_05_outline";
        }

        Kernel.colorManager.TryGetColor(ShadowColorName, out shadowEffectColor);
        Kernel.colorManager.TryGetColor(OutlineColorName, out outlineEffectColor);
        UIUtility.SetBaseMeshEffectColor(Detect_AR_Button.gameObject, true, shadowEffectColor, outlineEffectColor);

        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Detect_AR_Button.onClick.AddListener(PressDetect_ARMode);
        Detect_Manual_Button.onClick.AddListener(PressDetect_Manual);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Detect_AR_Button.onClick.RemoveAllListeners();
        Detect_Manual_Button.onClick.RemoveAllListeners();
    }



    public void InitDetectPopup(string FieldName, string szExplain, int IslandNum)
    {
        ExplainText.text = szExplain;
        IslandName = FieldName;
        IslandNumber = IslandNum;
    }




    //AR모드.
    private void PressDetect_ARMode()
    {
        if (SystemInfo.supportsGyroscope)    //자이로 지원하면.
        {
            Kernel.uiManager.Close(UI.DetectPopup);

            Kernel.entry.detect.onDetectStartAR += StartAR;
            Kernel.entry.detect.REQ_PACKET_CG_GAME_START_TREASURE_DETECT_SYN(IslandNumber);
        }
        else
            UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.NOT_SUPPORT_DEVICE));

    }

    private void StartAR()
    {
        Kernel.entry.detect.onDetectStartAR -= StartAR;

        Kernel.entry.detect.DetectFieldName = IslandName;
        Kernel.sceneManager.LoadScene(Scene.DetectAR);
    }






    //수동.
    private void PressDetect_Manual()
    {
        Kernel.uiManager.Close(UI.DetectPopup);

        Kernel.entry.detect.onDetectStartAR += StartManual;
        Kernel.entry.detect.REQ_PACKET_CG_GAME_START_TREASURE_DETECT_SYN(IslandNumber);
    }




    private void StartManual()
    {
        Kernel.entry.detect.onDetectStartAR -= StartManual;

        Kernel.entry.detect.DetectFieldName = IslandName;

        UIDetectManual DetectManualData = Kernel.uiManager.Open<UIDetectManual>(UI.DetectManual);
        DetectManualData.InitDetectManual(Kernel.entry.detect.DetectBoxIndex, IslandName, IslandNumber);
    }





}
