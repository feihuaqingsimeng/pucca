using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraAsBackground : MonoBehaviour
{
    private RawImage TargetImage;
    private WebCamTexture CamTexture;
    private AspectRatioFitter arf;
    private bool PauseMode;

    // Use this for initialization
    void Awake()
    {
        arf = GetComponent<AspectRatioFitter>();

        TargetImage = GetComponent<RawImage>();
        CamTexture = new WebCamTexture();
        if (CamTexture.deviceName == "no camera available.")
            return;

        CamTexture.requestedFPS = 12;
        TargetImage.texture = CamTexture;
        CamTexture.Play();

        PauseMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamBackground();
    }

    public void UpdateCamBackground()
    {
        if (PauseMode)
            return;

        if (CamTexture.width < 100)
            return;

        float cwNeeded = -CamTexture.videoRotationAngle;
        if (CamTexture.videoVerticallyMirrored)
            cwNeeded += 180.0f;

        TargetImage.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, cwNeeded);


        float videoRatio = (float)CamTexture.width / (float)CamTexture.height;
        arf.aspectRatio = videoRatio;


        if (CamTexture.videoVerticallyMirrored)
            TargetImage.uvRect = new Rect(1.0f, 0.0f, -1.0f, 1.0f);
        else
            TargetImage.uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    }



    public void ShowCamBackground()
    {
        CamTexture.Play();
        PauseMode = false;
    }

    public void PauseCamBackground()
    {
        CamTexture.Stop();
        PauseMode = true;
    }

}
