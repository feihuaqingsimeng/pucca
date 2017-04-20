using UnityEngine;
using System.Collections;

public class TitleScene : SceneObject
{
    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Open(UI.Title);
        }

        SoundManager.Instance.PlayTitleSound();

        return base.Preprocess();
    }
}
