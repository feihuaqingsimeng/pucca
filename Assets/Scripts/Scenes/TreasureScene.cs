using UnityEngine;
using System.Collections;

public class TreasureScene : SceneObject {

    // To Kernel.Update().
    /*
    protected override void Update()
    {
        // 보물찾기 시간 업데이트
        Kernel.entry.treasure.Update();
    }
    */

    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
            Kernel.uiManager.Open(UI.Treasure);

        return base.Preprocess();
    }
}
