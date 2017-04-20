using UnityEngine;
using System.Collections;

public class NormalShopScene : SceneObject
{
    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
            Kernel.uiManager.Open(UI.NormalShop);

        return base.Preprocess();
    }
}
