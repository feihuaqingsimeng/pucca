using UnityEngine;
using System.Collections;

public class BattleScene : SceneObject
{

    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Close(UI.HUD);
            Kernel.uiManager.Open(UI.Battle);
            Kernel.uiManager.Open(UI.BattleStart);
        }

        Kernel.soundManager.PlaySound(SOUND.BGM_BATTLE_0, true);

        return base.Preprocess();
    }

}
