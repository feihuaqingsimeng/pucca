using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISubBattle_PVE : MonoBehaviour
{
    public Text             Hero_Name;

    public GameObject       Wave_UIObject;
    public float            WaveInfo_PosX_Min;
    public float            WaveInfo_PosX_Max;
    public RectTransform    MovePointer_Battle;
    public RectTransform    MovePointer_Move;
    public RectTransform    MoveFace_Obj;
    public Image            MoveFace_Portrait;

    public  GameObject      BossHP_UIObject;
    public  float           BossHPGaugeWidth;
    public  RectTransform   BossHP_RectTransform;
    public  Text            BossHP_CurValue;

    private BattleManager   pBattleMng;

    private float           MaxMoveLength;

    private bool            BossBattle;
    private BattlePawn      BossPawn;
    private bool            BossEmpty;





    public void InitSubBattleUI_PVE(BattleManager BattleMng, int BossCardIndex)
    {
        pBattleMng = BattleMng;

        if(BossCardIndex != 0)
            MoveFace_Portrait.sprite = TextureManager.GetPortraitSprite(BossCardIndex);
        MovePointer_Battle.gameObject.SetActive(false);
        MovePointer_Move.gameObject.SetActive(false);
        MoveFace_Obj.gameObject.SetActive(false);

        MaxMoveLength = WaveInfo_PosX_Max - WaveInfo_PosX_Min;


        BossBattle = false;
        if (BossCardIndex != 0)
            BossBattle = true;

        Wave_UIObject.SetActive(true);
        BossHP_UIObject.SetActive(false);

        BossPawn = null;
        BossEmpty = false;
    }


    void Update()
    {
        if (pBattleMng.CurBattleKind != BATTLE_KIND.PVE_BATTLE)
            return;

        int CurGroupIndex = pBattleMng.PVE_BattleGroupIndex;

        if (CurGroupIndex >= pBattleMng.BattleGroupArray.Length)     //마지막 웨이브면 전투중 표기와 이동 표기 끄기.
        {
            if (BossBattle)
            {
                Wave_UIObject.SetActive(false);
                BossHP_UIObject.SetActive(true);

                if (BossPawn == null && BossEmpty == false)
                {
                    BossPawn = pBattleMng.FindBattlePawn_Boss();
                    if (BossPawn == null)
                    {
                        BossEmpty = true;
                    }
                }

                if (BossPawn != null)
                {
                    float CurWidth = (float)BossPawn.CurHP * BossHPGaugeWidth / (float)BossPawn.MaxHP;
                    if (CurWidth >= BossHPGaugeWidth)
                        CurWidth = BossHPGaugeWidth;
                    if (CurWidth <= 0.0f)
                        CurWidth = 0.0f;
                    BossHP_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurWidth);
                    BossHP_CurValue.text = Languages.GetNumberComma(BossPawn.CurHP);
                }
            }
            else
            {
                MovePointer_Battle.gameObject.SetActive(true);
                MovePointer_Move.gameObject.SetActive(false);
                MoveFace_Obj.gameObject.SetActive(false);
            }
        }
        else
        {
            bool bCurBattle = pBattleMng.CheckAliveEnemyGroup();        //전투중이면 전투중 표기.
            MovePointer_Battle.gameObject.SetActive(bCurBattle);

            float MoveGap = MaxMoveLength - (pBattleMng.BattleGroupArray[CurGroupIndex].SpawnTime_Cur * MaxMoveLength / pBattleMng.BattleGroupArray[CurGroupIndex].SpawnTime_Max);

            if (CurGroupIndex == pBattleMng.BattleGroupArray.Length - 1)     //마지막 웨이브일땐 보스얼굴이 이동.
            {
                if (BossBattle)
                {
                    MovePointer_Move.gameObject.SetActive(false);
                    MoveFace_Obj.gameObject.SetActive(true);
                    MoveFace_Obj.localPosition = new Vector3(WaveInfo_PosX_Min + MoveGap, MoveFace_Obj.localPosition.y, MoveFace_Obj.localPosition.z);
                }
                else
                {
                    MovePointer_Move.gameObject.SetActive(true);
                    MoveFace_Obj.gameObject.SetActive(false);

                    MovePointer_Move.localPosition = new Vector3(WaveInfo_PosX_Min + MoveGap, MovePointer_Move.localPosition.y, MovePointer_Move.localPosition.z);
                }
            }
            else                                                            //일반 웨이브일땐 일반포인터가 이동.
            {
                MovePointer_Move.gameObject.SetActive(true);
                MoveFace_Obj.gameObject.SetActive(false);

                MovePointer_Move.localPosition = new Vector3(WaveInfo_PosX_Min + MoveGap, MovePointer_Move.localPosition.y, MovePointer_Move.localPosition.z);
            }
        }
    }




}
