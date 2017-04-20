using UnityEngine;
using System.Collections;

public class BuffElement
{
    private     BattleManager       BattleMng;
    private     BattlePawn          BasePawnData;


    public      DB_Buff.Schema      BuffBaseData;

    private     EffectPoolElement   AttachEffectElement;

    public      SkillType   BuffSkillType;
    public      bool    LeaderSkill;

    public      int     Index;

    private     int     Level;
    public      float   CurValue;
    private     float   CurTickTime;
    [HideInInspector]
    public      float   CurLifeTime;
    [HideInInspector]
    public      float   MaxLifeTime;


    private     int     AttackerValue_AP;



    private     int     HitEffect_ID;

    [HideInInspector]
    public      bool    BuffActive;



    [HideInInspector]
    public      int     Attacker_Key;



    public void InitBuffElement()
    {
        BuffActive = false;
    }


    //세팅.
    public void AddBuffElement(BattlePawn pAttacker, BattlePawn pBasePawn, bool Overlap, int BuffIndex, int nLevel, int nActiveEffect_ID, int nHitEffect_ID, SkillType eSkillType, bool bLeader = false)
    {
        if (BuffIndex == 0)
            return;

        LeaderSkill = bLeader;
        if (LeaderSkill)    //강제 오버랩 금지.
            Overlap = false;

        if (!Overlap && BuffActive)
            return;

        if (Overlap)
        {
            CurTickTime = 0.0f;
            CurLifeTime = 0.0f;
            return;
        }

        BuffBaseData = DB_Buff.Query(DB_Buff.Field.Index, BuffIndex);
        if (BuffBaseData == null)
            return;

        BuffSkillType = eSkillType;
        Index = BuffIndex;
        BasePawnData = pBasePawn;
        BattleMng = pBasePawn.BattleMng;

        Level = nLevel-1;
        CurValue = BuffBaseData.BaseValue + (BuffBaseData.LvAddValue * Level);


        if (pAttacker == null)
            Attacker_Key = -1;
        else
            Attacker_Key = pAttacker.GetBattlePawnKey();

        //즉시발동 버프들 처리하고...
        switch(BuffBaseData.BUFF_KIND)
        {
            case BUFF_KIND.KNOCKBACK:
//                pBasePawn.SetPushHit(pAttacker);
                pBasePawn.SetKnockBack(pAttacker, KNOCKBACK_TYPE.KNOCKBACK_SKILL);
                break;

            case BUFF_KIND.ANTI_DEBUFF:
                break;

            case BUFF_KIND.REMOVE_ICE:
                BasePawnData.RemoveBuff_Manual(BUFF_KIND.FREEZE);
                break;

            case BUFF_KIND.REMOVE_SUMMON:
                break;

            case BUFF_KIND.REMOVE_BUFF:
                BasePawnData.RemoveGoodBuff();
                break;
            
            case BUFF_KIND.HEALING_ONE:
                int HealValue = 0;
                bool CriticalHeal = false;
                BasePawnData.MakeBattleSkillHealCount(pAttacker, BasePawnData, BuffSkillType, ref HealValue, ref CriticalHeal);
                BasePawnData.GetHeal_SkillManual((int)((float)HealValue * CurValue), BuffSkillType, HitEffect_ID);
                break;
            
            case BUFF_KIND.GET_COST:
                break;

            case BUFF_KIND.COOLTIME_DOWN:
                BasePawnData.SkillCoolTime_Cur += (BuffBaseData.MaxTime + (Level * BuffBaseData.AddMaxTime));
                if (BasePawnData.SkillCoolTime_Cur >= BasePawnData.SkillCoolTime_Max)
                    BasePawnData.SkillCoolTime_Cur = BasePawnData.SkillCoolTime_Max;
                break;
            
            case BUFF_KIND.LOST_HP:
                int LostCount = (int)((float)BasePawnData.MaxHP * (BuffBaseData.BaseValue + (Level * BuffBaseData.LvAddValue)));
                int LimitCount = (int)((float)BasePawnData.MaxHP * 0.2f);

                if (BasePawnData.CurHP > LimitCount)
                {
                    BasePawnData.CurHP -= Mathf.Abs(LostCount);
                    if(BasePawnData.CurHP <= LimitCount)
                        BasePawnData.CurHP = LimitCount;

                    if(BasePawnData.AirborneMode)
                        BasePawnData.RemoveBuffElement(BUFF_KIND.AIRBORN);

                }  
                break;

            case BUFF_KIND.STATIC_DAMAGE:
                BasePawnData.GetDamage_Skill_Manual((int)(BuffBaseData.BaseValue + (Level * BuffBaseData.LvAddValue)), BuffSkillType, HitEffect_ID);
                break;

            default:
                BuffActive = true;
                CurTickTime = 0.0f;
                CurLifeTime = 0.0f;
                MaxLifeTime = BuffBaseData.MaxTime;
                if (BuffBaseData.AddMaxTime != 0)
                    MaxLifeTime += Level * BuffBaseData.AddMaxTime;

                bool LoopEffect = true;
                float EffectDir = 1.0f;
                bool bGroundEffect = false;
                switch (BuffBaseData.BUFF_KIND)
                {
                    case BUFF_KIND.FREEZE:
                    case BUFF_KIND.BERSERKER_ATT_UP:
                    case BUFF_KIND.SACRIFICE:
                    case BUFF_KIND.SKILL_SHIELD:
                    case BUFF_KIND.HARD_TANKING:
                    case BUFF_KIND.FAST:
                        bGroundEffect = true;
                        break;

                    case BUFF_KIND.AIRBORN:
                        pBasePawn.SetAirborne(pAttacker);
                        MaxLifeTime = 2.0f;
                        break;

                    case BUFF_KIND.REFLECT:
                        bGroundEffect = true;
                        EffectDir = -1.0f;
                        break;
                }

                bool bHeadEffect = false;

                switch (BuffBaseData.BUFF_KIND)
                {
                    case BUFF_KIND.STUN:
                        bHeadEffect = true;
                        break;
                }


                if (nActiveEffect_ID != -1)
                {
                    AttachEffectElement = BasePawnData.SetAttachEffectToPawn(nActiveEffect_ID, LoopEffect, bGroundEffect, bHeadEffect, EffectDir);
                }

                HitEffect_ID = nHitEffect_ID;
                if (HitEffect_ID != -1)
                {
                    BattleMng.pEffectPoolMng.SetBattleEffect(BasePawnData.GetPawnPosition_Center(), HitEffect_ID);
                }

                if (BuffBaseData.BUFF_KIND == BUFF_KIND.DOT_DAMAGE || BuffBaseData.BUFF_KIND == BUFF_KIND.POISON || BuffBaseData.BUFF_KIND == BUFF_KIND.SACRIFICE)
                {
                    bool tempCri = false;
                    BasePawnData.MakeBattleDamage(pAttacker, BasePawnData, ref AttackerValue_AP, ref tempCri, true);
                }
                break;
        }


        AddBuffSound(BuffBaseData.BUFF_KIND);
    }



	// 갱신처리.
	public void UpdateBuffElement()
    {
        if (BasePawnData == null || BasePawnData.IsDeath())
        {
            BuffActive = false;
            if(AttachEffectElement != null)
                BattleMng.pEffectPoolMng.SetDisenableEffect(AttachEffectElement);
            return;
        }

        if (BattleMng.eBattleStateKind != BATTLE_STATE_KIND.BATTLE)
            return;


        //즉시버프 제외한 다른 버프들은 여기서 처리.
        switch (BuffBaseData.BUFF_KIND)
        {
            case BUFF_KIND.DOT_DAMAGE:
            case BUFF_KIND.POISON:
                CurTickTime += Time.deltaTime;

                if (CurTickTime >= BuffBaseData.TickDelay)
                {
                    CurTickTime = 0.0f;
                    BasePawnData.GetDotDamage(AttackerValue_AP, CurValue, HitEffect_ID);
                }

                CurLifeTime += Time.deltaTime;
                if (CurLifeTime >= MaxLifeTime)
                    ReleaseBuffElement();
                break;

            case BUFF_KIND.SACRIFICE:
                CurTickTime += Time.deltaTime;

                if (CurTickTime >= BuffBaseData.TickDelay)
                {
                    CurTickTime = 0.0f;
                    if (BasePawnData.CurHP >= BasePawnData.MaxHP * 0.2f)
                        BasePawnData.GetDotDamage(BasePawnData.MaxHP, Mathf.Abs(CurValue), HitEffect_ID);
                }

                CurLifeTime += Time.deltaTime;
                if (CurLifeTime >= MaxLifeTime)
                    ReleaseBuffElement();
                break;

            case BUFF_KIND.HEALING_CONSIST:
            case BUFF_KIND.TOTEM_HEALING_CONSIST:
                CurTickTime += Time.deltaTime;

                if (CurTickTime >= BuffBaseData.TickDelay)
                {
                    CurTickTime = 0.0f;
                    BasePawnData.GetDotHeal(CurValue, HitEffect_ID);
                }

                CurLifeTime += Time.deltaTime;
                if (CurLifeTime >= MaxLifeTime)
                    ReleaseBuffElement();
                break;

            default:
                CurLifeTime += Time.deltaTime;
                if (CurLifeTime >= MaxLifeTime)
                    ReleaseBuffElement();
                break;

        }
	}


    public void ReleaseBuffElement()
    {
        CurLifeTime = 0.0f;
        BuffActive = false;
        if (AttachEffectElement != null)
        {
            BattleMng.pEffectPoolMng.SetDisenableEffect(AttachEffectElement);
            AttachEffectElement = null;
        }
    }









    private void AddBuffSound(BUFF_KIND eBuffKind)
    {
        switch (eBuffKind)
        {
            case BUFF_KIND.FAST:
                Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_SPEED_UP);
                break;

            case BUFF_KIND.FREEZE:
                Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_FREEZE);
                break;
        }
        
    }
    
}
