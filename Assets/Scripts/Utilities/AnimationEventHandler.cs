using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField]
    Animator m_Animator;

    public Animator animator
    {
        get
        {
            return m_Animator;
        }
    }

    public delegate void OnAnimationEventCallback(string value);
    public OnAnimationEventCallback onAnimationEventCallback;

    void Reset()
    {
        if (m_Animator == null)
        {
            m_Animator = GetComponent<Animator>();
        }
    }

    // Use this for initialization

    // Update is called once per frame

    public void OnAnimationEvent(string value)
    {
        if (onAnimationEventCallback != null)
        {
            onAnimationEventCallback(value);
        }
    }
}
