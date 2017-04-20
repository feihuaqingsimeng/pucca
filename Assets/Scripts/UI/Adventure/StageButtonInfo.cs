using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum STAGEBUTTON_OPEN_STATE
{
    OPEN = 0,
    SELECT,
    CLOSE
}

public class StageButtonInfo : MonoBehaviour
{
    private UIAdventure         AdventureManager;



    public  bool                SpineButton;
    public  SkeletonAnimation   SpineAnimation;
    private Renderer            SpineRenderer;

    public  Button      PressButtonObject;
    public  Text        StageNumber;
    public  GameObject  ArrowObject;
    public  GameObject  EffectBottomObject;


    //데이터.
    [HideInInspector]
    public  int         StageIndex;

    private bool                    CheckForceChangeColor;
    private STAGEBUTTON_OPEN_STATE  eCurState;


    //초기화.
    public void InitStageButton(UIAdventure AdventureMng, int _StageID, STAGEBUTTON_OPEN_STATE state)
    {
        eCurState = state;


        //객체 호출.
        PressButtonObject = transform.FindChild("Button").GetComponent<Button>();
        EffectBottomObject = transform.FindChild("EffectBottom").gameObject;

        Transform UICanvasObj = PressButtonObject.transform.FindChild("UI_Canvas");
        ArrowObject = UICanvasObj.FindChild("EffectArrow").gameObject;
        StageNumber = UICanvasObj.FindChild("Text").GetComponent<Text>();

        PressButtonObject.onClick.AddListener(PressButton);


        //초기화.
        AdventureManager = AdventureMng;
        StageIndex = _StageID;

        StageNumber.text = (_StageID % 100).ToString();

        Color Color_Outline, Color_Shadow;
        if (eCurState == STAGEBUTTON_OPEN_STATE.OPEN || eCurState == STAGEBUTTON_OPEN_STATE.SELECT)
        {
            Kernel.colorManager.TryGetColor("ui_button_02_outline", out Color_Outline);
            Kernel.colorManager.TryGetColor("ui_button_02_shadow", out Color_Shadow);

            StageNumber.color = Color.white;
            SetColor(StageNumber, Color_Outline, Color_Shadow);
        }
        else
        {
            Kernel.colorManager.TryGetColor("ui_button_04_outline", out Color_Outline);
            Kernel.colorManager.TryGetColor("ui_button_04_shadow", out Color_Shadow);

            StageNumber.color = Color.gray;
            SetColor(StageNumber, Color.black, Color.black);
        }

        if(SpineButton)
            StageNumber.gameObject.SetActive(false);

        switch (eCurState)
        {
            case STAGEBUTTON_OPEN_STATE.OPEN:
                EffectBottomObject.gameObject.SetActive(false);
                ArrowObject.SetActive(false);
                break;

            case STAGEBUTTON_OPEN_STATE.SELECT:
                EffectBottomObject.gameObject.SetActive(true);
                ArrowObject.SetActive(true);
                break;

            case STAGEBUTTON_OPEN_STATE.CLOSE:
                EffectBottomObject.gameObject.SetActive(false);
                ArrowObject.SetActive(false);
                break;
        }

        CheckForceChangeColor = false;
        if (SpineButton && SpineAnimation != null)
        {
            SpineRenderer = SpineAnimation.GetComponent<Renderer>();
            switch (eCurState)
            {
                case STAGEBUTTON_OPEN_STATE.OPEN:
                case STAGEBUTTON_OPEN_STATE.SELECT:
                    SpineRenderer.material.color = Color.white;
                    SpineAnimation.timeScale = 1.0f;
                    PressButtonObject.enabled = true;
                    break;

                case STAGEBUTTON_OPEN_STATE.CLOSE:
                    SpineRenderer.material.color = Color.gray;
                    SpineAnimation.timeScale = 0.0f;
                    PressButtonObject.enabled = false;
                    break;
            }
            CheckForceChangeColor = true;
        }
        else
        {
            SpineRenderer = null;
            switch (eCurState)
            {
                case STAGEBUTTON_OPEN_STATE.OPEN:
                case STAGEBUTTON_OPEN_STATE.SELECT:
                    PressButtonObject.GetComponent<Image>().color = Color.white;
                    PressButtonObject.enabled = true;
                    break;

                case STAGEBUTTON_OPEN_STATE.CLOSE:
                    PressButtonObject.GetComponent<Image>().color = Color.gray;
                    PressButtonObject.enabled = false;
                    break;
            }
        }
    }



	void Update ()
    {
        if(CheckForceChangeColor && SpineRenderer != null)
        {
            if (eCurState == STAGEBUTTON_OPEN_STATE.CLOSE && SpineRenderer.material.color == Color.white)
            {
                SpineRenderer.material.color = Color.gray;
//                CheckForceChangeColor = false;
            }
        }
	}


    public void PressButton()
    {
        if (AdventureManager == null)
            return;

        AdventureManager.ShowStageInfo(StageIndex);

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.GroupNumber == 30 && StageIndex == 101)  //첫번째 지역 첫번째 버튼.
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }
    }






    void SetColor(Text text, Color outlineColor, Color shadowColor)
    {
        if (text != null)
        {
            foreach (var item in text.GetComponentsInChildren<Shadow>(true))
            {
                if (item is Outline)
                {
                    item.effectColor = outlineColor;
                }
                else if (item is Shadow)
                {
                    item.effectColor = shadowColor;
                }
            }
        }
    }
}
