using System.Collections;

public class DeckScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame

    protected override void OnDisable()
    {
        // Entry의 모든 Node는 OnApplicationQuit에서 처리하도록 수정해야 합니다.
        if (Kernel.entry.character.isDirty)
        {
            Kernel.entry.character.REQ_PACKET_CG_CARD_EDIT_DECK_INFO_SYN();
        }

        Kernel.entry.character.REQ_PACKET_CG_CARD_CLEAR_NEW_FLAG_SYN();
    }

    public override IEnumerator Preprocess()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Open(UI.Deck);
            Kernel.uiManager.Open(UI.HUD);
            // Preload.
            Kernel.uiManager.Get(UI.CharCardOption, true, false);
            Kernel.uiManager.Get(UI.CardInfo, true, false);
        }

        return base.Preprocess();
    }
}
