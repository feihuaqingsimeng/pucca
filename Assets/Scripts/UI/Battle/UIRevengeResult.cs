using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIRevengeResult : UIObject
{
    public  GameObject[]    WinText;
    public  Text            txtHaveRevengePoint;
    public  Text            txtGetRevengePoint;

    public  Text            RevengeResultText;

    public  SkeletonAnimation   PuccaSpineAni_Win;
    public  SkeletonAnimation   PuccaSpineAni_Lose;

    public  Button          ExitButton;
    public  Button          GoShopButton;


    private bool            CheckEndMotion;

    protected override void Awake()
    {
        ExitButton.onClick.AddListener(PressExitButton);
        GoShopButton.onClick.AddListener(PressGoShopButton);
        CheckEndMotion = false;
    }

    public void InitRevengeResult(BattleManager pBattleMng, BATTLE_RESULT_STATE eResultType, int CurRevengePoint, int GetRevengePoint)
    {
        CheckEndMotion = false;

        for(int idx = 0; idx < WinText.Length; idx++)
        {
            WinText[idx].SetActive(false);
        }


        if (eResultType == BATTLE_RESULT_STATE.WIN)
        {
            PuccaSpineAni_Win.gameObject.SetActive(true);
            PuccaSpineAni_Lose.gameObject.SetActive(false);
            ResetVictoryMotion();
        }
        else
        {
            PuccaSpineAni_Win.gameObject.SetActive(false);
            PuccaSpineAni_Lose.gameObject.SetActive(true);
            ResetDefeatMotion();
        }


        switch (eResultType)
        {
            case BATTLE_RESULT_STATE.WIN:
                WinText[0].SetActive(true);
                RevengeResultText.text = Languages.ToString(TEXT_UI.REVENG_SUCCESS);

                if (GetRevengePoint < 0)
                    txtGetRevengePoint.gameObject.SetActive(false);
                else
                {
                    txtGetRevengePoint.gameObject.SetActive(true);
                }
                txtGetRevengePoint.text = "(+" + Languages.GetNumberComma(GetRevengePoint) + ")";
                break;

            case BATTLE_RESULT_STATE.DRAW:
                WinText[1].SetActive(true);

                RevengeResultText.text = Languages.ToString(TEXT_UI.REVENG_DRAW);
                txtGetRevengePoint.gameObject.SetActive(false);
                break;

            case BATTLE_RESULT_STATE.LOSE:
                WinText[2].SetActive(true);

                RevengeResultText.text = Languages.ToString(TEXT_UI.REVENG_FAIL);
                txtGetRevengePoint.gameObject.SetActive(false);
                break;
        }


        txtHaveRevengePoint.text = Languages.GetNumberComma(CurRevengePoint);

    }


    void PressExitButton()
    {
        Kernel.uiManager.Close(UI.RevengeResult);
        Kernel.uiManager.Open(UI.HUD);
        
        Kernel.sceneManager.LoadScene(Scene.RevengeBattle);
    }

    void PressGoShopButton()
    {
        Kernel.uiManager.Close(UI.BattleResult);
        Kernel.uiManager.Open(UI.HUD);

        Kernel.sceneManager.LoadScene(Scene.StrangeShop);
    }


    protected override void Update()
    {
        base.Update();

        if (CheckEndMotion)
        {
            if (IsPlaying(PuccaSpineAni_Lose) == false)
            {
                ResetDefeatLoopMotion();
                CheckEndMotion = false;
            }
        }
    }






    //스파인제어.
    private void SetSpineMotion(SkeletonAnimation pAnimation, string MotionName, bool loop)
    {
        pAnimation.state.SetAnimation(0, MotionName, loop);
    }

    private bool IsPlaying(SkeletonAnimation pAnimation)
    {
        if (pAnimation.state.GetCurrent(0) == null)
            return false;

        return true;
    }


    private void ResetVictoryMotion()
    {
        SetSpineMotion(PuccaSpineAni_Win, "victory", true);
    }


    private void ResetDefeatMotion()
    {
        PuccaSpineAni_Lose.Reset();
        SetSpineMotion(PuccaSpineAni_Lose, "defeat", false);
        CheckEndMotion = true;
    }


    private void ResetDefeatLoopMotion()
    {
        SetSpineMotion(PuccaSpineAni_Lose, "defeat_loop", true);
    }

}
