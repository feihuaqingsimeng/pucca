using UnityEngine;
using System.Collections;

public class ComboPointInfo : MonoBehaviour 
{
    public  GameObject      ComboIcon;


    public void ShowComboPoint(bool bShow)
    {
        ComboIcon.SetActive(bShow);
    }
}
