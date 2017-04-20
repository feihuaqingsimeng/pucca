using UnityEngine;
using System.Collections;

public class AdventureScene : SceneObject
{
    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Open(UI.HUD);
            Kernel.uiManager.Open(UI.Adventure);
        }

        Kernel.soundManager.PlaySound(SOUND.BGM_EXPLORE, true);

        return base.Preprocess();
    }

}
