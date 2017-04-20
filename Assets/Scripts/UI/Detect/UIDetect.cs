using Common.Packet;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIDetect : UIObject
{
    public  DetectIslandInfo[]  IslandList;


    protected override void Awake()
    {
        base.Awake();

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 700)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }

        UpdateIslandList();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        Kernel.entry.detect.onUpdateIslandInfo += UpdateIslandList;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Kernel.entry.detect.onUpdateIslandInfo -= UpdateIslandList;
    }


    public void UpdateIslandList()
    {
        for(int idx = 0; idx < IslandList.Length; idx++)
        {
            IslandList[idx].InitIslandInfo();
        }
    }


}
