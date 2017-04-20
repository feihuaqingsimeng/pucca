using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTestModule : MonoBehaviour
{
    public  bool            BattleTestMode;

    public  int             HeroLeader = 5;
    public  TestPawnData[]  HeroTeam;

    public  int             EnemyLeader = 5;
    public  TestPawnData[]  EnemyTeam;

    public  float           Stat_Compensate;    //약화값.

    public  bool            SuperMode;
    public  bool            DummyEnemyMode;


    void Awake()
    {
        if (BattleTestMode)
            DontDestroyOnLoad(this);
    }
}


[System.Serializable]
public class TestPawnData
{
    public  int Index = 0;
    public  int Level_Pawn;
    public  int Level_Skill;
    public  int Level_Weapon;
    public  int Level_Armor;
    public  int Level_Accessory;
}
