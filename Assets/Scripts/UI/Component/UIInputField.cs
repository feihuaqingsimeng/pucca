using UnityEngine.UI;
using UnityEngine;

public class UIInputField : InputField
{
    [SerializeField]
    int m_LineLimit;

    public int lineLimit
    {
        get
        {
            return m_LineLimit;
        }

        set
        {
            if (m_LineLimit != value)
            {
                m_LineLimit = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        onValidateInput += delegate(string text, int charIndex, char addedChar)
        {
            if (m_LineLimit > 0 && addedChar.Equals('\n'))
            {
                int lineCount = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i].Equals('\n'))
                    {
                        lineCount++;
                    }
                }

                if (m_LineLimit.Equals(lineCount + 1))
                {
                    return '\0';
                }
            }

            return (characterLimit > 0) && (characterLimit < charIndex) ? '\0' : addedChar;
        };
    }

    // Use this for initialization

    // Update is called once per frame

}
