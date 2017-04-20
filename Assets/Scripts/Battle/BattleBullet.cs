using UnityEngine;
using System.Collections;



public class BattleBullet : MonoBehaviour
{
    private BATTLE_BULLET_TYPE      eBulletType;

    [HideInInspector]
    public  bool                    ActiveBullet = false;

    private bool                    ParabolaShot = false;

    public void InitBullet(BattleManager pBattleMng, BattlePawn pBasePawn, BATTLE_BULLET_TYPE eType)
    {
        eBulletType = eType;
        switch (eBulletType)
        {
            case BATTLE_BULLET_TYPE.HORIZON:
                if (pBasePawn.CardIndex == 11)
                    ParabolaShot = false;
                else
                    ParabolaShot = true;

                gameObject.GetComponent<ThrowObject>().InitThrowObject(pBattleMng, this, pBasePawn);
                break;

            case BATTLE_BULLET_TYPE.MAGIC_TARGET_ATT:
            case BATTLE_BULLET_TYPE.MAGIC_TARGET_HEAL:
                gameObject.GetComponent<TargetMagic>().InitTargetMagic(pBattleMng, this, pBasePawn, eBulletType);
                break;
        }
    }



    public void InitSkillBullet_ThrowHorizon(BattleManager pBattleMng, BattlePawn pBasePawn, BattleSkillManager pBattleSkillMng, SkillType eSkillType)
    {
        eBulletType = BATTLE_BULLET_TYPE.HORIZON;
        gameObject.GetComponent<ThrowObject>().InitThrowObject_Skill(pBattleMng, this, pBasePawn, pBattleSkillMng, eSkillType);
    }





    public void ShotBullet(Vector3 ThrowPos, BattlePawn pTargetPawn)
    {
        switch (eBulletType)
        {
            case BATTLE_BULLET_TYPE.HORIZON:
                gameObject.GetComponent<ThrowObject>().SetThrow(ParabolaShot, ThrowPos, pTargetPawn, SkillType.Active); //노멀투척인데...
                ActiveBullet = true;
                break;
        }
    }


    public void CastingMagic(BattlePawn TargetPawn, SkillType eSkillType = SkillType.Active)    //노멀매직....
    {
        switch (eBulletType)
        {
            case BATTLE_BULLET_TYPE.MAGIC_TARGET_ATT:
                gameObject.GetComponent<TargetMagic>().SetCastingMagic(TargetPawn, false, eSkillType);
                ActiveBullet = true;
                break;

            case BATTLE_BULLET_TYPE.MAGIC_TARGET_HEAL:
                gameObject.GetComponent<TargetMagic>().SetCastingMagic(TargetPawn, true, eSkillType);
                ActiveBullet = true;
                break;
        }

    }



    public void PauseBullet()
    {
        switch (eBulletType)
        {
            case BATTLE_BULLET_TYPE.HORIZON:
                gameObject.GetComponent<ThrowObject>().PauseThrowTrailEffect();
                break;
        }
    }

    public void ResumeBullet()
    {
        switch (eBulletType)
        {
            case BATTLE_BULLET_TYPE.HORIZON:
                gameObject.GetComponent<ThrowObject>().ResumeThrowTrailEffect();
                break;
        }
    }



    public void ReleaseBullet()
    {
        ActiveBullet = false;
    }


}
