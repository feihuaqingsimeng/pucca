using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMileageInfo : MonoBehaviour
{
    public Text             MileageCount;
    public RectTransform    MileageGauge;
    public float            GaugeWidth;

	void OnEnable ()
    {
        MileageCount.text = Kernel.entry.account.winPoint.ToString();

        float GaugeValue = Kernel.entry.account.winPoint * GaugeWidth / 10.0f;
        if (GaugeValue <= 0.0f)
            GaugeValue = 0.0f;
        if (GaugeValue >= GaugeWidth)
            GaugeValue = GaugeWidth;
        MileageGauge.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GaugeValue);
    }

}
