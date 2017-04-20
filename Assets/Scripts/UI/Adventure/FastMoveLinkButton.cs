using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FastMoveLinkButton : MonoBehaviour
{
    private FastMoveManager FastMoveMng;

    private int             AreaIndex;
    public  Button          LinkButton;
    public  GameObject      EnabledIcon;
    public  GameObject      DisabledIcon;
    public  GameObject      SelectOutLine;


    public void InitLinkButton(FastMoveManager pManager, int nAreaIndex)
    {
        FastMoveMng = pManager;
        AreaIndex = nAreaIndex;

        LinkButton.onClick.AddListener(PressLinkButton);
    }


    public void UpdateLinkButton(bool Selected, bool Enabled)
    {
        //선택시 테두리.
        SelectOutLine.SetActive(Selected);

        //아이콘.
        EnabledIcon.SetActive(Enabled);
    
        if(DisabledIcon != null)
            DisabledIcon.SetActive(!Enabled);
    }



    public void PressLinkButton()
    {
        Kernel.entry.adventure.SelectAreaIndex = AreaIndex;
        FastMoveMng.UpdateFastMoveLink();
    }


}
