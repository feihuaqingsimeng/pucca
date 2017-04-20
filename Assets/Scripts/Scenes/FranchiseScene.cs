using UnityEngine;
using System.Collections;

public class FranchiseScene : SceneObject
{
    public override IEnumerator Preprocess()
    {
        while (!Kernel.entry.franchise.m_bSettingComplet)
            yield return null;

        if (Kernel.uiManager)
            Kernel.uiManager.Open(UI.Franchise);

        yield return base.Preprocess();
    }
}
