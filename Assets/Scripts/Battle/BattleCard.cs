using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleCard : MonoBehaviour
{
    public  int             BattleCardIndex;

    private UIBattle        pBattleUI;

    public  Animator        ButtonAnimation;

    public  GameObject      EmptySlot;
    public  GameObject      BattleCardObj;
    public  GameObject      ButtonEffect;

    //콤보.
    public  GameObject      ComboEffect;

    public  Image           PawnPortrait;

    public  Image           CardFrame_Back;
    public  Image           CardFrame_Out;

    public  Image           NotEnoughCostGauge;
    public  GameObject      NotUseSkill;
    public  Image           UseGauge;

    public  GameObject      CostUI;
    public  Text            CostCount;

    public  GameObject      DieUI;
    public  GameObject      WarningUI;

    public  Button          UseSkillButton;

    private BattleManager   BattleMng;
    private BattlePawn      BasePawnData;


    private bool            PreHideButton;


    private GameObject      PressButtonEffect;

    public  Text            TextCoolTime;


    public  BattleGauge_HP  HPGauge;


    void Awake()
    {
        UseSkillButton.onClick.AddListener(PressBattleCard);
    }



    public void InitBattleCard(UIBattle BattleUI, BattlePawn pBasePawn)
    {
        pBattleUI = BattleUI;

        BasePawnData = pBasePawn;
        BattleMng = BasePawnData.BattleMng;
        DB_Card.Schema CardInfo = DB_Card.Query(DB_Card.Field.Index, BasePawnData.CardIndex);

        if (CardInfo == null)
        {
            EmptySlot.SetActive(true);
            BattleCardObj.SetActive(false);
            ButtonEffect.SetActive(false);
        }
        else
        {
            EmptySlot.SetActive(false);
            BattleCardObj.SetActive(true);
            ButtonEffect.SetActive(false);
            DieUI.SetActive(false);
            WarningUI.SetActive(false);

            PawnPortrait.sprite = TextureManager.GetPortraitSprite(BasePawnData.CardIndex);
            CardFrame_Back.sprite = TextureManager.GetGradeTypeBackgroundSprite(BasePawnData.GetGradeType());
            CardFrame_Out.sprite = TextureManager.GetGradeTypeFrameSprite(BasePawnData.GetGradeType());
            UseGauge.fillAmount = 0.0f;

            if(BattleMng.UseExSkill_H)
                CostCount.text = BasePawnData.SkillCost_Max.ToString();
            else
                CostCount.text = BasePawnData.SkillCost.ToString();
        }

        PressButtonEffect = Instantiate(Resources.Load("Effects/EF_UI_CardSlot_fx")) as GameObject;
        PressButtonEffect.transform.SetParent(transform);
        PressButtonEffect.transform.localPosition = Vector3.zero;
        PressButtonEffect.transform.localScale = Vector3.one;
        PressButtonEffect.SetActive(false);

        BasePawnData.HPGaugeObject = HPGauge;
    }


    public void SetEmptyCard()
    {
        EmptySlot.SetActive(true);
        BattleCardObj.SetActive(false);
        ButtonEffect.SetActive(false);

        ComboEffect.SetActive(false);
    }



    void Update()
    {
        UpdateBattleCard();
        UpdatePawnHP();
    }


    public void UpdateBattleCard()
    {
        if (BasePawnData == null)
            return;

        if (BasePawnData.IsDeath())
        {
            DieUI.SetActive(true);
            WarningUI.SetActive(false);
            CostUI.SetActive(false);
            UseGauge.gameObject.SetActive(false);
            ButtonEffect.SetActive(false);
            TextCoolTime.gameObject.SetActive(false);

            //콤보.
            ComboEffect.SetActive(false);
        }
        else
        {
            DieUI.SetActive(false);
            CostUI.SetActive(true);
            UseGauge.gameObject.SetActive(true);


            bool CheckSkillTimeActive = false;
            bool CheckSkillCostActive = false;
            bool CheckPawnActive = true;

            if (BattleMng.SkillUseDelay)    //스킬 사용 딜레이 중이면... 또는 특수스킬중일때.
            {
                float Value = BattleMng.CurSkillUseDelayTime * 1.0f / BattleMng.MaxSkillUseDelayTime;
                UseGauge.fillAmount = 1.0f - Value;
                PreHideButton = true;
            }
            else
            {
                UseGauge.fillAmount = 0.0f;
                CheckSkillTimeActive = true;
            }

            if(BasePawnData.SkillCoolTime_Cur < BasePawnData.SkillCoolTime_Max)
            {
                if(!NotEnoughCostGauge.gameObject.activeInHierarchy)
                    NotEnoughCostGauge.gameObject.SetActive(true);
                NotEnoughCostGauge.fillAmount = 1.0f - (BasePawnData.SkillCoolTime_Cur * 1.0f / BasePawnData.SkillCoolTime_Max);
                PreHideButton = true;

                if(!TextCoolTime.gameObject.activeInHierarchy)
                    TextCoolTime.gameObject.SetActive(true);
                TextCoolTime.text = (BasePawnData.SkillCoolTime_Max - BasePawnData.SkillCoolTime_Cur).ToString("f1") + "s";
            }
            else
            {
                NotEnoughCostGauge.gameObject.SetActive(false);
                TextCoolTime.gameObject.SetActive(false);
                CheckSkillCostActive = true;
            }




            //폰 상태.
            if (BasePawnData.FreezeMode || BasePawnData.StunMode || BasePawnData.AirborneMode || BasePawnData.SilenceMode || BasePawnData.bModule_Jump || BasePawnData.bModule_Push || BasePawnData.SpecialActionMode)
            {
                NotUseSkill.SetActive(true);
                PreHideButton = true;
                CheckPawnActive = false;
            }
            else
            {
                NotUseSkill.SetActive(false);
            }


            if (BasePawnData.FreezeMode || BasePawnData.StunMode || BasePawnData.AirborneMode || BasePawnData.SilenceMode || BasePawnData.bModule_Jump || BasePawnData.bModule_Push)
                WarningUI.SetActive(true);
            else
                WarningUI.SetActive(false);



            if (PreHideButton && (CheckSkillTimeActive && CheckSkillCostActive && CheckPawnActive))
            {
                if(Kernel.entry.tutorial.TutorialActive)
                {
                    if(Kernel.entry.tutorial.GroupNumber == 10)
                    {
                        switch (Kernel.entry.tutorial.WaitSeq)
                        {
                            case 101:
                                if (BattleCardIndex == 1)
                                {
                                    BattleMng.BattlePause();
                                    Kernel.entry.tutorial.onSetNextTutorial();
                                }
                                break;

                            case 102:
                                if (BattleCardIndex == 2)
                                {
                                    BattleMng.BattlePause();
                                    Kernel.entry.tutorial.onSetNextTutorial();
                                }
                                break;

                            case 103:
                                if (BattleCardIndex == 3)
                                {
                                    BattleMng.BattlePause();
                                    Kernel.entry.tutorial.onSetNextTutorial();
                                }
                                break;
                        }

                    }
                }



                PreHideButton = false;
                ActiveButtonAni();
            }


            if (PreHideButton == false)
            {
                ButtonEffect.SetActive(true);

                //콤보.
                if (BattleMng.CurComboCount_H >= BattleMng.MaxComboCount)
                    ComboEffect.SetActive(true);
                else
                    ComboEffect.SetActive(false);
            }
            else
            {
                ButtonEffect.SetActive(false);

                //콤보.
                ComboEffect.SetActive(false);
            }

        }


    }




    private void UpdatePawnHP()
    {
        if (HPGauge == null)
            return;

        if (BasePawnData == null)
        {
            HPGauge.UpdateBattleGauge_HP(Vector3.zero, 1.0f);
            return;
        }

        HPGauge.UpdateBattleGauge_HP(Vector3.zero, (float)BasePawnData.CurHP * 1.0f / (float)BasePawnData.MaxHP);
    }


    public void PressBattleCard()
    {
        int nSelectButtonIdx = 0;
        for (int idx = 0; idx < pBattleUI.BattleCardList.Length; idx++)
        {
            if (pBattleUI.BattleCardList[idx] != this)
                gameObject.GetComponent<RectTransform>().SetSiblingIndex(0);
            else
                nSelectButtonIdx = idx;
        }

        pBattleUI.BattleCardList[nSelectButtonIdx].gameObject.GetComponent<RectTransform>().SetSiblingIndex(4);




        bool bNotPressMode = false;

        if (BattleMng.eBattleStateKind != BATTLE_STATE_KIND.BATTLE)
            bNotPressMode = true;

        if (BasePawnData == null || BasePawnData.IsDeath())
            bNotPressMode = true;

        if (BasePawnData.FreezeMode || BasePawnData.StunMode || BasePawnData.AirborneMode || BasePawnData.SilenceMode || BasePawnData.bModule_Jump || BasePawnData.bModule_Push || BasePawnData.SpecialActionMode)
            bNotPressMode = true;

        if (BattleMng.SkillUseDelay)
            bNotPressMode = true;


        if (bNotPressMode)
        {
            BattleMng.pBattleUI.ShowBattleMsg(Languages.ToString(TEXT_UI.NOT_YET_AVAILABLE)); // TID 필요.
            return;
        }

        bool CheckUse = false;
        if(BattleMng.UseExSkill_H)
            BasePawnData.SetUseSkill(SkillType.Max, ref CheckUse);
        else
            BasePawnData.SetUseSkill(SkillType.Active, ref CheckUse);

        if (CheckUse)
        {
            PressButtonEffect.SetActive(false);
            PressButtonEffect.SetActive(true);
        }

    }


    void HidePressButtonEffect()
    {
        PressButtonEffect.SetActive(false);
    }

    void ReturnRenderPos()
    {
        gameObject.GetComponent<RectTransform>().SetSiblingIndex(0);
    }




    public void ActiveButtonAni()
    {
        ButtonAnimation.SetTrigger("Pressed");
    }

    public void EndButtonAni()
    {
        ButtonAnimation.SetTrigger("Normal");
    }


}
