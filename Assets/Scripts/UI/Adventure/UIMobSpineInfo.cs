using UnityEngine;
using System.Collections;

public class UIMobSpineInfo : MonoBehaviour
{
    public SkeletonAnimation    SpineAnimation;

    private bool                bActiveSpine = false;
    private int                 BattlePower;    //전투력.
    private ClassType           eClassType;     //클래스.
    private string              MobName;        //이름.

    public float                MoveLength;
    public float                MoveSpeed;

    public UITooltipObject      tooltip; 


    public void InitUIMobSpineInfo(int CardIndex, int nBattlePower)
    {
        BattlePower = nBattlePower;

        SetMobSpine(CardIndex);

        if (tooltip != null)
        {
            string strClass = "";

            switch (eClassType)
            {
                case ClassType.ClassType_Healer: strClass = Languages.ToString(TEXT_UI.CLASS_HEALER); break;
                case ClassType.ClassType_Hitter: strClass = Languages.ToString(TEXT_UI.CLASS_HITTER); break;
                case ClassType.ClassType_Keeper: strClass = Languages.ToString(TEXT_UI.CLASS_KEEPER); break;
                case ClassType.ClassType_Ranger: strClass = Languages.ToString(TEXT_UI.CLASS_RANGER); break;
                case ClassType.ClassType_Wizard: strClass = Languages.ToString(TEXT_UI.CLASS_WIZARD); break;
            }

            string strPower = Languages.ToString(TEXT_UI.BATTLE_POWER) + ":" + BattlePower;
            tooltip.content = "<color=#FFFFFFFF>" + MobName + "</color>" + "\n" + strClass + "\n" + strPower;
        }
        tooltip.gameObject.SetActive(false);
        
        bActiveSpine = true;
        SetMove();
    }





    public void SetMobSpine(int CardIndex)
    {
        DB_Card.Schema DBData_Base = DB_Card.Query(DB_Card.Field.Index, CardIndex);
        string CharDir = "Spines/Character/" + DBData_Base.IdentificationName + "/" + DBData_Base.IdentificationName + "_SkeletonData";
        try
        {
            SpineAnimation.skeletonDataAsset = (SkeletonDataAsset)Resources.Load(CharDir);
        }
        catch (System.Exception error)
        {
            Debug.LogWarning(error);
            Debug.LogError("File Not Find : " + CharDir);
            return;
        }

        SpineAnimation.initialSkinName = DBData_Base.IdentificationName;

        SpineAnimation.transform.localPosition = new Vector3(MoveLength, SpineAnimation.transform.localPosition.y, SpineAnimation.transform.localPosition.z);

        eClassType = DBData_Base.ClassType;

        MobName = Languages.FindCharName(CardIndex);

        float SizeRate = DBData_Base.SizeRate;
        transform.localScale = Vector3.one * SizeRate;
    }


    public void SetMove()
    {
        SpineAnimation.AnimationName = "run";
        SpineAnimation.Reset();
        Spine.TrackEntry pCurAniData = SpineAnimation.state.GetCurrent(0);
        pCurAniData.Loop = true;
        SpineAnimation.timeScale = 1.0f;
    }


    public void SetIdle()
    {
        SpineAnimation.AnimationName = "wait";
        SpineAnimation.Reset();
        Spine.TrackEntry pCurAniData = SpineAnimation.state.GetCurrent(0);
        pCurAniData.Loop = true;
        SpineAnimation.timeScale = 1.0f;
    }






	void Update ()
    {
        if (!bActiveSpine)
            return;

        float fPosX = SpineAnimation.transform.localPosition.x;
        fPosX -= Time.deltaTime * MoveSpeed;
        if(fPosX <= 0.0f)
        {
            fPosX = 0.0f;
            SetIdle();
            tooltip.gameObject.SetActive(true);
            bActiveSpine = false;
        }

        SpineAnimation.transform.localPosition = new Vector3(fPosX, SpineAnimation.transform.localPosition.y, SpineAnimation.transform.localPosition.z);


	}
}
