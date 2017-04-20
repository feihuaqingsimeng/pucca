using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonSound : MonoBehaviour
{
    public  Button      ButtonComponent;
    public  SOUND       eSoundKind = SOUND.SND_BUTTON_GOOD_1;

    void Awake()
    {
        ButtonComponent = transform.GetComponent<Button>();
        if (ButtonComponent != null)
            ButtonComponent.onClick.AddListener(PlaySound);
    }


    void PlaySound()
    {
        if (Kernel.soundManager != null)
            Kernel.soundManager.PlaySound(eSoundKind);
    }
	


}
