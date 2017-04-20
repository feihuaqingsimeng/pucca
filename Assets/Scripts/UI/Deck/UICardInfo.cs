using UnityEngine;
using UnityEngine.UI;

public class UICardInfo : UIObject
{
    public UICardSkillInfo m_CardSkillInfo;
    public UICardCharInfo m_CardCharInfo;
    public UICardEquipInfo m_CardEquipInfo;

    public GameObject m_AllMaxLevelImg;
    private bool allMaxLevel
    {
        set
        {
            m_AllMaxLevelImg.SetActive(value);
        }
    }
    
    long m_CID;

    // Use this for initialization

    // Update is called once per frame

    public long cid
    {
        get
        {
            return m_CID;
        }

        set
        {
            if (m_CID != value)
            {
                m_CID = value;

                m_CardSkillInfo.cid = m_CID;
                m_CardCharInfo.cid = m_CID;
                m_CardEquipInfo.cid = m_CID;
                CheckAllLevelMax();
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Kernel.entry.character.onLevelUpCallback += CheckAllLevelMax;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_CID = 0;

        Kernel.entry.character.onLevelUpCallback -= CheckAllLevelMax;
    }

    private void CheckAllLevelMax()
    {
        bool allMax = true;

        if (!m_CardSkillInfo.isMaxLevel)
            allMax = false;

        if (!m_CardCharInfo.isMaxLevel)
            allMax = false;

        if (!m_CardEquipInfo.CheckAllEquipMaxLevel())
            allMax = false;

        allMaxLevel = allMax;
    }




    //튜토리얼용.
    protected override void Update()
    {
        base.Update();

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 402)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }
    }
}
