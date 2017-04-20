using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextLevelMaxEffect : TextGradientUtility 
{
    private int m_nMaxValue;
    public int MaxValue
    {
        set
        {
            m_nMaxValue = value;
        }
    }

    private int m_nValue;
    public int Value
    {
        set
        {
            m_nValue = value;

            SetMaxEffect();
        }
    }

    private Text     m_Text;
    public  Outline  m_OutLine;
    public  Shadow   m_Shadow;

    [SerializeField]
    public  Color32    m_Normal_MainColor;
    [SerializeField]
    public  Color32    m_Normal_OutLineColor;
    [SerializeField]
    public  Color32    m_Normal_ShadowColor;

    [SerializeField]
    public  Color32    m_Max_MainTopColor;
    [SerializeField]
    public  Color32    m_Max_MainBottomColor;
    [SerializeField]
    public  Color32    m_Max_OutLineColor;
    [SerializeField]
    public  Color32    m_Max_ShadowColor;

#if UNITY_EDITOR
    protected override void Reset()
    {
        Offset = -0.2f;

        m_Normal_MainColor      = Color.white;
        m_Normal_OutLineColor   = Color.black;
        m_Normal_ShadowColor    = Color.black;

        m_Max_MainTopColor      = GetColor(255,255,107,255);
        m_Max_MainBottomColor   = GetColor(255,111,0,255);
        m_Max_OutLineColor      = GetColor(51,18,0,255);
        m_Max_ShadowColor       = GetColor(92,29,0,255);
    }

    private Color32 GetColor(byte r, byte g, byte b, byte a)
    {
        Color32 newColor = new Color32();

        newColor.r = r;
        newColor.g = g;
        newColor.b = b;
        newColor.a = a;

        return newColor;
    }
#endif

    protected override void Awake()
    {
        if (m_Text == null)
            m_Text = GetComponent<Text>();
    }

    private void SetMaxEffect()
    {
        bool isMax = m_nMaxValue <= m_nValue;

        m_OutLine.effectColor = isMax ? m_Max_OutLineColor  : m_Normal_OutLineColor;
        m_Shadow.effectColor =  isMax ? m_Max_ShadowColor   : m_Normal_ShadowColor;

        StartColor = isMax ? m_Max_MainTopColor : m_Normal_MainColor;
        EndColor = isMax ? m_Max_MainBottomColor : m_Normal_MainColor;
    }
}
