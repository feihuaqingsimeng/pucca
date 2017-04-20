using UnityEngine;
using System.Collections;


public enum CAM_SHAKE_PRESET
{
    SHAKE_WEAK = 0,
    SHAKE_MIDDLE,
    SHAKE_HARD
}

public class CameraShake : MonoBehaviour
{
    private bool    ShakeMode;
    private float   curShakeTime;
    private float   maxShakeTime;
    private float   ShakePower;
    private float   curShakeDelay;
    private float   ShakeDelay = 0.015f;


    public void SetShake(CAM_SHAKE_PRESET eShakeType)
    {
        float power = 0.0f;
        float time = 0.0f;
        switch(eShakeType)
        {
            case CAM_SHAKE_PRESET.SHAKE_WEAK:
                power = 0.03f;
                time = 0.1f;
                break;

            case CAM_SHAKE_PRESET.SHAKE_MIDDLE:
                power = 0.05f;
                time = 0.2f;
                break;

            case CAM_SHAKE_PRESET.SHAKE_HARD:
                power = 0.08f;
                time = 0.3f;
                break;
        }

        SetShake(power, time);
    }

    public void SetShake(float power, float time)
    {
        ShakeMode = true;
        maxShakeTime = time;
        ShakePower = power;
        curShakeTime = 0.0f;
        curShakeDelay = 0.0f;
    }



	
	void Update ()
    {
        if (!ShakeMode)
            return;

        curShakeTime += Time.deltaTime;
        if (curShakeTime >= maxShakeTime)
        {
            ShakeMode = false;
            transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);
            return;
        }

        curShakeDelay += Time.deltaTime;
        if (curShakeDelay >= ShakeDelay)
        {
            curShakeDelay = 0.0f;
            transform.localPosition = new Vector3(Random.Range(-ShakePower, ShakePower), Random.Range(-ShakePower, ShakePower), -10.0f);
        }
	}
}
