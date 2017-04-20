using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleUtil : MonoBehaviour
{

    //float 수치간에 거리.
    public static float GetDistance_X(float fBase, float fNext)
    {
        return Mathf.Abs(fNext - fBase);
    }


    public static float FrontPawnPosition(List<BattlePawn> pList)
    {
        float fMaxValue = 0.0f;

        for (int idx = 0; idx < pList.Count; idx++)
        {
            if (pList[idx].IsDeath() || pList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (fMaxValue == 0.0f || pList[idx].transform.position.x >= fMaxValue)
            {
                fMaxValue = pList[idx].transform.position.x;
            }
        }

        return fMaxValue;
    }



}
