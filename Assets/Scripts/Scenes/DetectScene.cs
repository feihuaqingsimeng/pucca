using System.Collections;
using UnityEngine;

public class DetectScene : SceneObject
{

    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Open(UI.HUD);
            Kernel.uiManager.Open(UI.Detect);

            yield return new WaitForSeconds(2f);
            if(Kernel.entry.detect.onUpdateIslandInfo != null)
                Kernel.entry.detect.onUpdateIslandInfo();
        }

        yield return base.Preprocess();
    }
}
