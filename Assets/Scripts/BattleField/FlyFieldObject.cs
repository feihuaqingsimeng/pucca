using UnityEngine;
using System.Collections;

public class FlyFieldObject : MonoBehaviour
{
    public  GameObject      TargetFieldObject;
    private Transform       TargetTransform;

    private GameObject      BattleCamera;
    private Transform       MainCameraTransform;

    public  float           GameScreenWidth;

    public  bool            LookDirectionMove;  //보는방향과 이동방향 같음.
    public  float           MoveSpeed_Min;
    public  float           MoveSpeed_Max;

    private float           MoveSpeed_Current;

    public  bool            UpDownMode;
    public  float           UpDownLimit_High;
    public  float           UpDownLimit_Low;
    public  float           UpDownSpeed_Min;
    public  float           UpDownSpeed_Max;

    private float           UpDownSpeed_Current;

    private float           UpDownDir;


    private bool            ActiveMode;
    private float           LookDir;
    private float           MoveDir;

    private bool            WaitMoveMode;
    private float           WaitTime_Current;
    private float           WaitTime_Max;


    private float           BasePos_X;
    private float           BasePos_Y;
    private float           EndPos_X;
    private float           EndPos_Y;
    private float           CurPos_X;
    private float           CurPos_Y;


    void Start()
    {
        WaitMoveMode = true;
        WaitTime_Max = 1.0f;
    }
    
	
	// Update is called once per frame
	void Update ()
    {
        if (BattleCamera == null)
            BattleCamera = GameObject.Find("BattleCamera");

        if (TargetFieldObject == null)
            return;

        if (TargetTransform == null)
            TargetTransform = TargetFieldObject.transform;

        if (MainCameraTransform == null)
            MainCameraTransform = BattleCamera.transform;

	
        //대기.
        if(WaitMoveMode)
        {
            WaitTime_Current += Time.deltaTime;
            if(WaitTime_Current >= WaitTime_Max)
            {
                WaitMoveMode = false;
                WaitTime_Current = 0.0f;

                //오브젝트 활성화.
                TargetFieldObject.SetActive(true);


                //시작위치.
                if (Random.Range(0, 10) < 5)
                {
                    BasePos_X = MainCameraTransform.position.x - GameScreenWidth/2;
                    MoveDir = 1.0f;
                    if(LookDirectionMove)
                        LookDir = 1.0f;
                    else
                        LookDir = -1.0f;
                }
                else
                {
                    BasePos_X = MainCameraTransform.position.x + GameScreenWidth/2;

                    MoveDir = -1.0f;

                    if (LookDirectionMove)
                        LookDir = -1.0f;
                    else
                        LookDir = 1.0f;
                }


                if (UpDownMode)
                {
                    BasePos_Y = Random.Range(UpDownLimit_Low, UpDownLimit_High);
                    EndPos_Y = Random.Range(UpDownLimit_Low, UpDownLimit_High);

                    if(BasePos_Y < EndPos_Y)
                        UpDownDir = -1.0f;
                    else
                        UpDownDir = 1.0f;

                    UpDownSpeed_Current = Random.Range((int)UpDownSpeed_Min, (int)UpDownSpeed_Max); 
                }
                else
                {
                    BasePos_Y = Random.Range(UpDownLimit_Low, UpDownLimit_High);
                    UpDownSpeed_Current = 0.0f;
                }

                //이동속도.
                MoveSpeed_Current = (float)Random.Range((int)MoveSpeed_Min, (int)MoveSpeed_Max);

                //방향.
                TargetTransform.localScale = new Vector3(LookDir, 1.0f, 1.0f);

                //위치 변경.
                TargetTransform.position = new Vector3(BasePos_X, BasePos_Y, TargetTransform.position.z);

                CurPos_X = BasePos_X;
            }
            return;
        }

        //X축 최대치.
        if (MoveDir > 0.0f)
            EndPos_X = MainCameraTransform.position.x + GameScreenWidth/2;
        else
            EndPos_X = MainCameraTransform.position.x - GameScreenWidth/2;


        //축 계산.
        CurPos_X += (MoveSpeed_Current * Time.deltaTime) * MoveDir;
        if(UpDownSpeed_Current > 0.0f)
            CurPos_Y += (UpDownSpeed_Current * Time.deltaTime) * UpDownDir;


        //X축.
        bool MoveEnd = false;
        if(MoveDir > 0.0f)
        {
            if(CurPos_X >= EndPos_X)
                MoveEnd = true;
        }
        else
        {
            if(CurPos_X <= EndPos_X)
                MoveEnd = true;
        }

        if(MoveEnd)
        {
            TargetFieldObject.SetActive(false);
            WaitMoveMode = true;
            WaitTime_Current = 0.0f;
            WaitTime_Max = Random.Range(1, 5);
            return;
        }


        //Y축.
        bool UpDownEnd = false;
        if (UpDownMode)
        {
            if (UpDownDir > 0.0f && CurPos_Y >= EndPos_Y)
                UpDownEnd = true;
            else if (UpDownDir < 0.0f && CurPos_Y <= EndPos_Y)
                UpDownEnd = true;
        }


        if (UpDownEnd)
        {
            EndPos_Y = Random.Range(UpDownLimit_Low, UpDownLimit_High);

            if (BasePos_Y < EndPos_Y)
                UpDownDir = -1.0f;
            else
                UpDownDir = 1.0f;

            UpDownSpeed_Current = Random.Range((int)UpDownSpeed_Min, (int)UpDownSpeed_Max); 
        }



        //이동.
        TargetTransform.position = new Vector3(CurPos_X, CurPos_Y, TargetTransform.position.z);
	}
}
