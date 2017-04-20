using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISupportCheck : UIObject 
{
    public Text m_Title;
    public Text m_Dec;

    protected override void OnEnable()
    {
        base.OnEnable();

        SetUI();
    }

    public void SetUI()
    {
        m_Title.text = Languages.ToString(TEXT_UI.MAKING_SUPPORT);
        m_Dec.text = Languages.ToString(TEXT_UI.PUCCA_PROJECT_SUPPORT_GNEXT);
    }
}
