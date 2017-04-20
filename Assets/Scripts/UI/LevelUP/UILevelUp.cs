using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.Packet;

public class UILevelUp : UIObject
{
    public RectTransform    ShineEffect;

    public Text             LevelValue;

    public Text             RewardCount_Heart;
    public Text             RewardCount_Ruby;
    public Text             RewardCount_Gold;

    public Text             OKButtonText;
    

    private float fRotateZ;

    protected override void Awake()
    {
        OKButtonText.text = Languages.ToString(TEXT_UI.OK);

        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        fRotateZ = 0.0f;
        InitLevelUp();
    }




    public void InitLevelUp()
    {
        int nLevel = Kernel.entry.account.level;
        LevelValue.text = nLevel.ToString();
        //TEXT_UI.GOODS_RECEIVE
        DB_AccountLevel.Schema LevelData = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, nLevel - 1);
        if (LevelData != null)
        {
            RewardCount_Heart.text  = Languages.ToString(TEXT_UI.GOODS_RECEIVE, Languages.ToString(TEXT_UI.HEART)   , Languages.GetNumberComma(LevelData.Reward_Heart));
            RewardCount_Ruby.text   = Languages.ToString(TEXT_UI.GOODS_RECEIVE, Languages.ToString(TEXT_UI.CASH)    , Languages.GetNumberComma(LevelData.Reward_Ruby));
            RewardCount_Gold.text   = Languages.ToString(TEXT_UI.GOODS_RECEIVE, Languages.ToString(TEXT_UI.GOLD)    , Languages.GetNumberComma(LevelData.Reward_TrainingPoint));
        }
        
        //레벨업 처리.
        //Kernel.entry.account.level++;
    }



    protected override void Update()
    {
        base.Update();


        fRotateZ -= 20.0f * Time.deltaTime;
        if(fRotateZ <= 360.0f)
            fRotateZ += 360.0f;
        ShineEffect.localRotation = Quaternion.Euler(0.0f, 0.0f, fRotateZ);
    }


}
