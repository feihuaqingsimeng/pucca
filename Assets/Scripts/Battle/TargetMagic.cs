using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetMagic : MonoBehaviour
{
    private BattleManager   BattleMng;
    private BattleBullet    BulletManager;
    private BattlePawn      AttackerPawn;
    private BattlePawn      TargetPawn;
    private BattleSkillManager SkillMng;

    private bool                ListHitMode;
    private ArrayList           HitKeyArrList = new ArrayList();
    

    public  float ThrowDelayTime; //활성화 후 발사 대기시간.
    private float curDelayTime;

    public  float HitDelayTime; //활성화 후 발사 대기시간.
    private float curHitDelayTime;


    public  Vector2 MagicOffset;    //투사체 생성 위치 오프셋.

    public  GameObject Prefab_Eff_Magic;      //마법 이펙트 프리팹.
    private int EffID_Magic;              //이펙트 풀에 넣어둔 마법 이펙트의 ID.

    public GameObject Prefab_Eff_Hit;         //마법 피격 이펙트 프리팹.
    private int EffID_Hit;              //이펙트 풀에 넣어둔 피격 이펙트의 ID.

    private bool ActiveMode;        //활성화여부.
    private bool WaitDelayEnd;
    private bool HealMagic;         //힐마법.

    private SkillType   MagicSkillType;

    //투사체 오브젝트 초기화.
    public void InitTargetMagic(BattleManager pBattleMng, BattleBullet BulletMng, BattlePawn pBasePawn, BATTLE_BULLET_TYPE eType)
    {
        BattleMng = pBattleMng;
        BulletManager = BulletMng;
        AttackerPawn = pBasePawn;
        SkillMng = null;

        DB_BattleBullet.Schema MagicBulletData = DB_BattleBullet.Query(DB_BattleBullet.Field.Bullet_ID, AttackerPawn.DBData_Base.Index,
            DB_BattleBullet.Field.BATTLE_BULLET_TYPE, eType);

        //마법 이펙트 오브젝트.
        if (MagicBulletData.BulletEffectName.Equals("NONE") == true)
            EffID_Magic = -1;
        else
        {
            Prefab_Eff_Magic = Resources.Load("Effects/" + MagicBulletData.BulletEffectName) as GameObject;
            if(Prefab_Eff_Magic != null)
                EffID_Magic = BattleMng.pEffectPoolMng.AddEffectPool(Prefab_Eff_Magic, false, 1);
        }

        //히트 이펙트는 최대 타격 수 만큼 만들어두자.
        Prefab_Eff_Hit = Resources.Load("Effects/" + MagicBulletData.HitEffectName) as GameObject;
        if(Prefab_Eff_Hit != null)
            EffID_Hit = BattleMng.pEffectPoolMng.AddEffectPool(Prefab_Eff_Hit, false, 1);

        ActiveMode = false;
        HealMagic = false;
        WaitDelayEnd = false;
        ListHitMode = false;
    }



    public void InitTargetMagic_Skill(BattleManager pBattleMng, BattlePawn pBasePawn, BattleSkillManager pSkillMng, SkillType eSkillType)
    {
        BattleMng = pBattleMng;
        BulletManager = null;

        AttackerPawn = pBasePawn;
        SkillMng = pSkillMng;

        int SkillIndex = 0;
        SKILL_KIND eSkillKind = SKILL_KIND.DAMAGE;

        //마법 이펙트 오브젝트.
        if (eSkillType == SkillType.Max)
        {
            EffID_Magic = SkillMng.SkillEffData_Max.EffID_Active;
            EffID_Hit = SkillMng.SkillEffData_Max.EffID_Hit;
            SkillIndex = SkillMng.DB_MaxSkill.Index;
            eSkillKind = SkillMng.DB_MaxSkill.SKILL_KIND;
        }
        else
        {
            EffID_Magic = SkillMng.SkillEffData_Active.EffID_Active;
            EffID_Hit = SkillMng.SkillEffData_Active.EffID_Hit;
            SkillIndex = SkillMng.DB_ActiveSkill.Index;
            eSkillKind = SkillMng.DB_ActiveSkill.SKILL_KIND;
        }


        ActiveMode = false;
        if(eSkillKind == SKILL_KIND.HEAL)
            HealMagic = true;
        else
            HealMagic = false;
        WaitDelayEnd = false;

        ThrowDelayTime = 0.2f;
        HitDelayTime = 0.0f;

        switch(SkillIndex)
        {
            case 55:    //지옥풀.
                HitDelayTime = 0.5f;
                break;
        }
        ListHitMode = false;
    }




    //발사!
    public void SetCastingMagic(BattlePawn Target, bool healMagic, SkillType eSkillType)
    {
        MagicSkillType = eSkillType;

        ActiveMode = true;
        WaitDelayEnd = false;
        HealMagic = healMagic;
        curDelayTime = 0.0f;
        curHitDelayTime = 0.0f;
        TargetPawn = Target;
        ListHitMode = false;
        transform.position = TargetPawn.PawnTransform.position;
    }


    public void SetCastingMagicToList(ArrayList pTargetList, SkillType eSkillType)
    {
        MagicSkillType = eSkillType;

        ActiveMode = true;
        WaitDelayEnd = false;
        curDelayTime = 0.0f;
        curHitDelayTime = 0.0f;

        HitKeyArrList.Clear();
        for (int idx = 0; idx < pTargetList.Count; idx++)
        {
            HitKeyArrList.Add((int)pTargetList[idx]);
        }
        ListHitMode = true;
    }


    void Update()
    {
        if (!ActiveMode)
            return;

        if (BattleMng.GamePauseMode)
            return;


        //활성화 후 대기.
        if(WaitDelayEnd == false)
        {
            curDelayTime += Time.deltaTime;
            if (curDelayTime >= ThrowDelayTime)
            {
                curDelayTime = 0.0f;

                if (EffID_Magic != -1)
                {
                    if (ListHitMode)
                    {
                        for (int idx = 0; idx < HitKeyArrList.Count; idx++)
                        {
                            BattlePawn pTempPawn = BattleMng.GetPawnOnKey((int)HitKeyArrList[idx]);

                            if (pTempPawn == null)
                            {
                                WaitDelayEnd = true;
                                return;
                            }

                            BattleMng.pEffectPoolMng.SetBattleEffect_Follow(pTempPawn.transform, true, EffID_Magic);
                        }
                    }
                    else
                    {
                        BattleMng.pEffectPoolMng.SetBattleEffect(TargetPawn.transform.position, EffID_Magic);
                    }
                }
                WaitDelayEnd = true;
            }
            return;
        }

        curHitDelayTime += Time.deltaTime;
        if(curHitDelayTime >= HitDelayTime)
        {
            ActiveMode = false;
            curHitDelayTime = 0.0f;

            //사운드.
            if (SkillMng != null)
            {
                switch (SkillMng.GetDB_Skill(MagicSkillType).Index)
                {
                    case 55:    //지옥풀.
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HELLGRASS);
                        break;

                    case 13:
                    case 1004:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_SCRATCH);
                        break;

                    case 34:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_LIGHTNING);
                        break;
                }
            }


            if(BulletManager != null)
                BulletManager.ActiveBullet = false;

            if (AttackerPawn == null)
                return;

            if (ListHitMode)
            {
                for (int idx = 0; idx < HitKeyArrList.Count; idx++)
                {
                    BattlePawn pTempPawn = BattleMng.GetPawnOnKey((int)HitKeyArrList[idx]);

                    if (pTempPawn == null)
                        continue;

                    if (HealMagic)
                    {
                        int HealValue = 0;
                        bool isCriticalHeal = false;
                        AttackerPawn.MakeBattleHealCount(AttackerPawn, pTempPawn, ref HealValue, ref isCriticalHeal);
                        pTempPawn.GetHeal_Normal(HealValue, isCriticalHeal, EffID_Hit);
                    }
                    else
                    {
                        pTempPawn.GetDamage_Skill(AttackerPawn, MagicSkillType, EffID_Hit);

                        switch (SkillMng.GetDB_Skill(MagicSkillType).Index)
                        {
                            case 46: //텔레포트.
                                pTempPawn.SetTeleport(AttackerPawn);
                                Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_TELEPORT);
                                break;
                        }
                    }
                    pTempPawn.AddBuff(AttackerPawn, MagicSkillType);
                }
            }
            else
            {
                if (HealMagic)
                {
                    int HealValue = 0;
                    bool isCriticalHeal = false;
                    AttackerPawn.MakeBattleHealCount(AttackerPawn, TargetPawn, ref HealValue, ref isCriticalHeal);
                    TargetPawn.GetHeal_Normal(HealValue, isCriticalHeal, EffID_Hit);
                }
                else
                {
                    int DamageValue = 0;
                    bool isCriticalDamage = false;
                    AttackerPawn.MakeBattleDamage(AttackerPawn, TargetPawn, ref DamageValue, ref isCriticalDamage);
                    TargetPawn.GetDamage_Normal(DamageValue, isCriticalDamage, EffID_Hit);

                    //데미지반사.
                    if (TargetPawn.ReflectDmgMode)
                        AttackerPawn.GetDamage_Reflect(DamageValue, TargetPawn.ReflectAddValue);
                }
            }
        }
    }

}
