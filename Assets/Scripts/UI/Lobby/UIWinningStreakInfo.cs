using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIWinningStreakInfo : MonoBehaviour
{
    public Text     WinningStreakCount;

    void Awake()
    {
        if (Kernel.entry.account.winningStreak <= 1)
            gameObject.SetActive(false);
    }


    void OnEnable()
    {
        string WinCountStr = "<color=#ffc600ff>" + Kernel.entry.account.winningStreak.ToString() + "</color>";

        WinningStreakCount.text = Languages.ToString(TEXT_UI.WIN_CONTINUE_BONUS_INFO, WinCountStr);
    }
}
