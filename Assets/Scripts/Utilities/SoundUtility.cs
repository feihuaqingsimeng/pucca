using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//** Button, Toggle, Drop, BGM 등 사운드.
//** GameObject에 add하여 사용.

//** 외부에서 사운드를 교체하기 위함.
public static class SoundDataInfo
{
    public static SoundUtility FindSoundUtility(GameObject obj)
    {
        return obj.GetComponent<SoundUtility>();
    }

    public static void ChangeUISound(SOUND changeSound, GameObject obj)
    {
        SoundUtility soundUtility = FindSoundUtility(obj);

        if(soundUtility != null)
            soundUtility.m_eSoundName = changeSound;
    }

    public static void RevertSound(GameObject obj)
    {
        SoundUtility soundUtility = FindSoundUtility(obj);

        if (soundUtility != null)
            soundUtility.m_eSoundName   = soundUtility.m_eOriSound;
    }

    public static void TimmerSound(float sec, GameObject obj, SOUND sound)
    {
        SoundUtility soundUtility = FindSoundUtility(obj);

        if (soundUtility == null)
            return;

        soundUtility.m_eSoundName = sound;
        soundUtility.Invoke("PlayUISound", sec);
    }

    public static void CancelSound(GameObject obj)
    {
        SoundUtility soundUtility = FindSoundUtility(obj);

        if (soundUtility != null)
            soundUtility.m_eSoundName = SOUND.SND_UI_CANCEL;
    }
}

public class SoundUtility : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public SOUND    m_eOriSound;

    public SOUND    m_eSoundName;

    [SerializeField]
    private bool    m_bIsUIS    = false;

    private bool    m_bIsBGM    = false;

    private void Awake()
    {
        m_bIsUIS = IsPointerClickSound();

        // 클릭 사운드는 기본이 Ok로 세팅
        if (m_bIsUIS && m_eSoundName == 0)
            m_eSoundName = SOUND.SND_UI_OK;

        m_eOriSound     = m_eSoundName;

        if (m_bIsUIS)
            return;

        DB_Sound.Schema pData = DB_Sound.Query(DB_Sound.Field.SOUND, m_eSoundName);
        // 데이터가 없을땐 Lock.
        if (pData == null)
        {
            this.enabled = false;
            return;
        }

        m_bIsBGM = pData.SOUND_TYPE == SOUND_TYPE.BGM ? true : false;

        if (m_bIsBGM && m_eSoundName != null)
            Kernel.soundManager.PlaySound(m_eSoundName, true);
    }

    //** 버튼 형식의 사운드인가?
    private bool IsPointerClickSound()
    {
        Button button = GetComponent<Button>();
        ToggleGroup toggleGroup = GetComponent<ToggleGroup>();
        Toggle toggle = GetComponent<Toggle>();

        return button != null || toggle != null || toggleGroup != null;
    }

    //** UI 사운드 재생
    public void PlayUISound()
    {
        if (m_eSoundName != null)
            Kernel.soundManager.PlayUISound(m_eSoundName);
    }

    //** 클릭시 사운드
    public void OnPointerClick(PointerEventData eventData)
    {
        // BGM이면 리턴.
        if (m_bIsBGM)
            return;

        // 버튼 형식의 Sound가 아니면 리턴.
        if (!m_bIsUIS)
            return;

        PlayUISound();
    }
}
