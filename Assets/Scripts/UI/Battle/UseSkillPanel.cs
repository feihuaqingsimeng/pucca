using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UseSkillPanel : MonoBehaviour 
{
    public  bool        HeroSkillPanel;

    private Animation   Panel_Animation;
    public  Image       Panel_Background;
    public  Image       Pawn_PortraitImage;
    public  Text        SkillText_Name;
    public  Text        SkillDescText_Up;
    public  Text        SkillDescText_Down;

    private Sprite      PanelBackground_Leader;
    private Sprite      PanelBackground_Active;


    void Awake()
    {
        Panel_Animation = gameObject.GetComponent<Animation>();
        PanelBackground_Leader = Resources.Load<Sprite>("Textures/UI/skillbox_leader");
        PanelBackground_Active = Resources.Load<Sprite>("Textures/UI/skillbox_active");
    }


    public void ShowSkillPanel(BattlePawn UsePawnData, SkillType eSkillType)
    {
        //애니메이션 일단 종료.
        Panel_Animation.Stop();

        //얼굴 교체.
        Pawn_PortraitImage.sprite = TextureManager.GetPortraitSprite(UsePawnData.DBData_Base.Index);

        DBStr_Skill.Schema SkillStrData = null;
        //리더.
        if (eSkillType == SkillType.Leader)
            Panel_Background.sprite = PanelBackground_Leader;
        else
            Panel_Background.sprite = PanelBackground_Active;
        SkillStrData = DBStr_Skill.Query(DBStr_Skill.Field.Skill_Index, UsePawnData.SkillManager.GetDB_Skill(eSkillType).Index, DBStr_Skill.Field.SkillType, eSkillType);

        //텍스트.
        SkillText_Name.text = SkillStrData.Skill_Name;
        SkillDescText_Up.text = SkillStrData.BattleText_Top;
        SkillDescText_Down.text = SkillStrData.BattleText_Bottom;

        if(HeroSkillPanel)
            Panel_Animation.Play("UseSkill_Hero_Ani");
        else
            Panel_Animation.Play("UseSkill_Enemy_Ani");
    }


}
