using System.Collections;

public class LobbyScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame
    // To Kernel.Update().
    /*
    protected override void Update()
    {
        // 보물찾기 시간 업데이트
        Kernel.entry.treasure.Update();
        Kernel.entry.post.Update();
    }
    */

    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Open(UI.HUD);
            // 임시 처리
            Kernel.uiManager.Open(UI.Shortcut);
            Kernel.uiManager.Close(UI.Shortcut);
            //Kernel.uiManager.Get(UI.Shortcut, true, false); // Preload.
            Kernel.uiManager.Open(UI.Lobby);
            Kernel.uiManager.Get(UI.CardInfo, true, false); // Preload.
        }

        Kernel.soundManager.PlaySound(SOUND.BGM_LOBBY, true);

        return base.Preprocess();
    }
}
