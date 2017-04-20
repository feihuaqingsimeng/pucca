using UnityEngine;
using System.Collections;

public class SecretBusinessScene : SceneObject
{

    public override IEnumerator Preprocess()
    {
        Kernel.entry.secretBusiness.SetParseTableData();

        if (Kernel.uiManager)
        {
            Kernel.uiManager.Get(UI.SecretCardHelp, true, false);
            Kernel.uiManager.Get(UI.SecretCardInfo, true, false);

            Kernel.uiManager.Open(UI.SecretBusiness);
            //Kernel.uiManager.Open(UI.SecretBoxSelect);
            
        }
         

        return base.Preprocess();
    }
}
