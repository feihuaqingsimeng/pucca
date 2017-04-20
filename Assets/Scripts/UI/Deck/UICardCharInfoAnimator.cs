using UnityEngine;

public class UICardCharInfoAnimator : MonoBehaviour
{
    public Animator m_Card_levelup_ani;
    public Animator m_Level_txt_Animation;

    // Use this for initialization

    // Update is called once per frame

    public void SetTrigger()
    {
        m_Card_levelup_ani.ResetTrigger("Normal");
        m_Card_levelup_ani.ResetTrigger("Card_levelup_ani");
        m_Level_txt_Animation.ResetTrigger("Normal");
        m_Level_txt_Animation.ResetTrigger("Level_txt_Animation");
        m_Card_levelup_ani.SetTrigger("Card_levelup_ani");
    }

    public void Level_txt_Animation()
    {
        m_Level_txt_Animation.SetTrigger("Level_txt_Animation");
    }
}
