using System.Collections;

public class AchieveScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame

    public override IEnumerator Preprocess()
    {
        Kernel.uiManager.Open(UI.Achieve);

        return base.Preprocess();
    }
}
