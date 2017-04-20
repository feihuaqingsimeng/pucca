using System.Collections;

public class DetectARScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame

    public override IEnumerator Preprocess()
    {
        Kernel.uiManager.Close(UI.HUD);
        UIDetectAR UIDetectARMng = (UIDetectAR)Kernel.uiManager.Open(UI.DetectAR);
        UIDetectARMng.InitAR();

        return base.Preprocess();
    }
}
