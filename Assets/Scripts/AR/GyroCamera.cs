using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GyroCamera : MonoBehaviour
{
    private Gyroscope   GyroInfo;
    private bool        GyroSupported;
    private Quaternion  rotFix;

    private bool        PauseMode;

	// Use this for initialization
	void Start ()
    {
        GyroSupported = SystemInfo.supportsGyroscope;

        
        GameObject camParent = new GameObject("CamParent");
        camParent.transform.position = transform.position;
        transform.parent = camParent.transform;

        if (GyroSupported)
        {
            GyroInfo = Input.gyro;
            GyroInfo.enabled = true;
//            camParent.transform.rotation = Quaternion.Euler(90.0f, 180.0f, 0.0f);
//            rotFix = new Quaternion(0.0f, 0.0f, 1.0f, 0.0f);
        }
	}
	
	// Update is called once per frame
    void Update()
    {
        if (PauseMode)
            return;

        if (GyroInfo == null)
        {
            KeyMoveUpdate();
            return;
        }

        gyroupdate();
/*
        transform.localRotation = GyroInfo.attitude;
 */ 
	}


    float Angle_X;
    float Angle_Y;

    bool PressKey_L;
    bool PressKey_R;
    bool PressKey_U;
    bool PressKey_D;

    void KeyMoveUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            PressKey_L = true;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            PressKey_R = true;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            PressKey_D = true;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            PressKey_U = true;

        if (Input.GetKeyUp(KeyCode.LeftArrow))
            PressKey_L = false;
        if (Input.GetKeyUp(KeyCode.RightArrow))
            PressKey_R = false;
        if (Input.GetKeyUp(KeyCode.DownArrow))
            PressKey_D = false;
        if (Input.GetKeyUp(KeyCode.UpArrow))
            PressKey_U = false;


        if(PressKey_L)
            Angle_Y -= 10.0f * Time.deltaTime;
        if(PressKey_R)
            Angle_Y += 10.0f * Time.deltaTime;
        if(PressKey_U)
            Angle_X -= 10.0f * Time.deltaTime;
        if(PressKey_D)
            Angle_X += 10.0f * Time.deltaTime;
        


        transform.rotation = Quaternion.Euler(Angle_X, Angle_Y, 0);
    }


    Quaternion transquat;

    void gyroupdate()
    {
        // 기본 자이로는 오브젝트 관찰 기준이라 카메라 기준으로 변경 
        //쿼터니언 하나 만들고 
        transquat = Quaternion.identity;
        transquat.w = GyroInfo.attitude.w;
        
        //x, y축의 값을 반대로 뒤집음 
        transquat.x = -GyroInfo.attitude.x;
        transquat.y = -GyroInfo.attitude.y;
        transquat.z = GyroInfo.attitude.z;


        // 변경된 쿼터니언을 안드로이드 자이로 기본 축 수정과 함께 카메라에 적용. 스크립트는 카메라에. 
        transform.rotation = Quaternion.Euler(90, 0, 0) * transquat;
    }


    public void StartGyroCamera()
    {
        PauseMode = false;
    }

    public void PauseGyroCamera()
    {
        PauseMode = true;
    }

}
