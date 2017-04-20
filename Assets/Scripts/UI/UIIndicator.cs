using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIIndicator : UIObject
{
    public  Image   IndicatorImage;
    private float   LoadingTime = 0;
    public  float   ImageRotationZ;
    public  float   RotateTime = 0.25f;

    protected override void Start() 
    {
        LoadingTime = Time.time;
    }

    protected override void Update()
    {
        if (IndicatorImage.gameObject.activeInHierarchy == false)
            return;

        if (Time.time - LoadingTime > RotateTime)
        {
            LoadingTime = Time.time;
            float pRotationz = IndicatorImage.transform.rotation.z + ImageRotationZ;

            pRotationz = Mathf.Clamp(pRotationz, 0, 360);
            IndicatorImage.transform.Rotate(0, 0, pRotationz);
        }
    }
}
