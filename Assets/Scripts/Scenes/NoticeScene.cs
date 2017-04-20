using UnityEngine;
using System.Collections;

public class NoticeScene : SceneObject 
{
    public override IEnumerator Preprocess()
    {
        Kernel.entry.notice.TestNoticePacket();

        while (!Kernel.entry.notice.m_bSettingComplet)
            yield return null;

        if (Kernel.uiManager)
        {
            UINotice notice = Kernel.uiManager.Get<UINotice>(UI.Notice, true, false);

            if (notice != null)
            {
                notice.CreateItems();
                Kernel.uiManager.Open(UI.Notice);
            }
        }

        yield return base.Preprocess();
    }
}
