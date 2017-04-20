using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddBuffEffect : MonoBehaviour
{
    public  float   AreaRange = 0.5f;

    private BattleManager   BattleMng;
    private BattlePawn      AttackerPawn;
    private bool            TargetIsEnemyTeam;


    public void InitAddBuffEffect(float fRange, BattlePawn Attacker, bool TargetEnemyTeam)
    {
        AreaRange = fRange;
        AttackerPawn = Attacker;
        TargetIsEnemyTeam = TargetEnemyTeam;
    }


    public void SetAddBuff(BattlePawn Target, SkillType eSkillType)
    {
        List<BattlePawn> TargetTeamList = null;
        if (TargetIsEnemyTeam)
            TargetTeamList = AttackerPawn.BattleMng.EnemyPawnList;
        else
            TargetTeamList = AttackerPawn.BattleMng.HeroPawnList;


        for (int idx = 0; idx < TargetTeamList.Count; idx++)
        {
            if (TargetTeamList[idx].IsDeath())
                continue;

            float Length = BattleUtil.GetDistance_X(Target.transform.position.x, TargetTeamList[idx].PawnTransform.position.x);
            if(Length < AreaRange)
            {
                TargetTeamList[idx].AddBuff(AttackerPawn, eSkillType);
            }
        }

    }

}
