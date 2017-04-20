using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager>
{
    private const string STR_BGM_ON_KEY = "BGM_ONOFF_KEY";
    private const string STR_EFS_ON_KEY = "EFS_ONFF_KEY";

    public  AudioSource     BGM_Source;
    public  AudioSource     SFX_source;
    public  Dictionary<string, AudioClip> SoundList = new Dictionary<string, AudioClip>();

    private SOUND           CurPlayBGM;

    private bool m_bBGM_On;
    public bool BGM_On
    {
        get { return m_bBGM_On; }
        set 
        { 
            m_bBGM_On = value;
            BGM_Source.volume = value ? 1.0f : 0.0f;

            int bgmValue = value ? 0 : 1;
            PlayerPrefs.SetInt(STR_BGM_ON_KEY, bgmValue);
            PlayerPrefs.Save();
        }
    }

    private bool m_bSFX_On;
    public bool SFX_On
    {
        get { return m_bSFX_On; }
        set 
        { 
            m_bSFX_On = value;
            SFX_source.volume = value ? 1.0f : 0.0f;

            int sfxValue = value ? 0 : 1;
            PlayerPrefs.SetInt(STR_EFS_ON_KEY, sfxValue);
            PlayerPrefs.Save();
        }
    }


    protected override void Awake()
    {
        base.Awake();

        BGM_Source = gameObject.AddComponent<AudioSource>();
        SFX_source = gameObject.AddComponent<AudioSource>();
        CurPlayBGM = 0;

        SoundVolumInit();
    }

    private void SoundVolumInit()
    {
        // BGM
        if (PlayerPrefs.HasKey(STR_BGM_ON_KEY))
        {
            int key = PlayerPrefs.GetInt(STR_BGM_ON_KEY);
            BGM_On = key == 0;
        }
        else
            BGM_On = true;

        // SFX
        if (PlayerPrefs.HasKey(STR_EFS_ON_KEY))
        {
            int key = PlayerPrefs.GetInt(STR_EFS_ON_KEY);
            SFX_On = key == 0;
        }
        else
            SFX_On = true;

        PlayerPrefs.Save();
    }

    public void LoadSoundData(AssetBundle pAssetBundle)
    {
        SoundList.Clear();
        foreach (DB_Sound.Schema pData in DB_Sound.instance.schemaList)
        {
            AudioClip pAudioClip = pAssetBundle.LoadAsset<AudioClip>(pData.sndName);
            SoundList.Add(pData.sndName, pAudioClip);
        }

        // 어셋번들에서 사운드를 가져오기 전에 필요한 경우 Sound 폴더에 추가 (ex : BGM_Title)
        AudioClip[] uiSoundClip = Resources.LoadAll<AudioClip>("Sound");

        if (uiSoundClip != null)
        {
            for (int i = 0; i < uiSoundClip.Length; i++)
                SoundList.Add(uiSoundClip[i].name, uiSoundClip[i]);
        }
    }

    public void PlayTitleSound()
    {
        AudioClip titleBGM = Resources.Load<AudioClip>("Sound/BGM_TITLE");

        if (titleBGM == null)
            return;

        BGM_Source.loop = true;
        BGM_Source.clip = titleBGM;
        BGM_Source.Play();
    }

    public void PlaySound(SOUND eSound, bool bLoop = false)
    {
        DB_Sound.Schema pData = DB_Sound.Query( DB_Sound.Field.SOUND, eSound);
        AudioClip pAudioClip = null;
        if (SoundList.ContainsKey(pData.sndName))
        {
            pAudioClip = SoundList[pData.sndName];
            if (pAudioClip != null)
            {
                if (pData.SOUND_TYPE == SOUND_TYPE.BGM)
                {
                    if (CurPlayBGM == eSound)
                        return;

                    CurPlayBGM = eSound;
                    BGM_Source.loop = bLoop;
                    BGM_Source.clip = pAudioClip;
                    BGM_Source.Play();
                }
                else
                {
                    SFX_source.PlayOneShot(pAudioClip);
                }
            }
        }
    }

    public void PlayUISound(SOUND eUISound)
    {
        if (SFX_source == null)
            return;

        AudioClip pAudioClip = null;

        DB_Sound.Schema pData = DB_Sound.Query(DB_Sound.Field.SOUND, eUISound);
        if (SoundList.ContainsKey(pData.sndName))
        {
            pAudioClip = SoundList[pData.sndName];

            if (pAudioClip == null)
                return;

            SFX_source.PlayOneShot(pAudioClip);
        }
    }

    public void StopBGM()
    {
        BGM_Source.Stop();
    }

    public AudioClip GetAudioClip(SOUND eSound)
    {
        DB_Sound.Schema pData = DB_Sound.Query( DB_Sound.Field.SOUND, eSound);
        if (SoundList.ContainsKey(pData.sndName))
        {
            return SoundList[pData.sndName];
        }

        return null;
    }
}
