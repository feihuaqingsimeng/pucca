using Common.Packet;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIDetectAR : UIObject
{
    public  GameObject      AimObj;

    public  Text            FieldNameText;
    public  Text            MessageText;

    public  Text            AngleText_H;

    public  GameObject      FindGauge;
    public  RectTransform   FindGauge_Inner;
    public  float           MaxGaugeLength;

    public  float           GaugeAddSpeed;
    public  float           CurGaugeValue;

    public  Button          ExitButton;

    public  Image           TimeCircle;


    public void InitAR()
    {
        GameObject ARManager = GameObject.Find("ARManager");
        if (ARManager == null)
            return;

        ARManager pARMng = ARManager.GetComponent<ARManager>();
        pARMng.InitDetectAR(this);
        ExitButton.onClick.AddListener(pARMng.ExitDetectAR);

    }

}
