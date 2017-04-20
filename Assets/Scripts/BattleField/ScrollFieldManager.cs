using UnityEngine;
using System.Collections;

public class ScrollFieldManager : MonoBehaviour
{
    public Transform    TargetTransform;

    public float        ScrollSpeed_Far;        //원경.
    public float        FirstPos_Far;
    public GameObject   FieldObj_Far;

    public float        ScrollSpeed_Middle;     //중경.
    public float        FieldLength_Middle;
    public float        FirstPos_Middle;
    public GameObject[] FieldObjList_Middle;

    public float        ScrollSpeed_Near;      //근경.
    public float        FieldLength_Near;
    public float        FirstPos_Near;
    public GameObject[] FieldObjList_Near;


    public float        ScrollSpeed_Field;      //필드.
    public float        FieldLength_Field;
    public float        FirstPos_Field;
    public GameObject[] FieldObjList_Field;

    [HideInInspector]
    public  float       FieldMoveRange;
    [HideInInspector]
    public  float       TargetFirstPostionX;

	void Awake ()
    {
	}


    public void InitScrollFieldManager(Transform pTarget)
    {
        if (pTarget == null)
            return;

        TargetTransform = pTarget;

        //원경.
        FieldObj_Far.transform.localPosition = new Vector3(FirstPos_Far, 0.0f, 0.0f);

        //중경.
        for (int idx = 0; idx < FieldObjList_Middle.Length; idx++)
        {
            FieldObjList_Middle[idx].transform.localPosition = new Vector3(FirstPos_Middle + (FieldLength_Middle * idx), 0.0f, 0.0f);
        }

        //근경.
        for (int idx = 0; idx < FieldObjList_Near.Length; idx++)
        {
            FieldObjList_Near[idx].transform.localPosition = new Vector3(FirstPos_Near + (FieldLength_Near * idx), 0.0f, 0.0f);
        }

        //필드.
        for (int idx = 0; idx < FieldObjList_Field.Length; idx++)
        {
            FieldObjList_Field[idx].transform.localPosition = new Vector3(FirstPos_Field + (FieldLength_Field * idx), 0.0f, 0.0f);
        }


        FieldMoveRange = 0;
        TargetFirstPostionX = TargetTransform.position.x;
    }

	
	// Update is called once per frame
	void Update ()
    {
        if (TargetTransform == null)
            return;

        FieldMoveRange = TargetTransform.position.x - TargetFirstPostionX;

        //원경.
        FieldObj_Far.transform.localPosition = new Vector3(FirstPos_Far + (FieldMoveRange * ScrollSpeed_Far), 0.0f, 0.0f);


        //중경은 카메라 좌표를 기준으로 계속 뒤로 붙이면서 약간 늦게 따라간다.

        //중경의 늦춰진 이동속도.
        float FieldMoveRange_Middle = FieldMoveRange * ScrollSpeed_Middle;
        float MiddleScrollRange = FieldMoveRange - FieldMoveRange_Middle;
        int AddFieldCount_Middle = (int)(MiddleScrollRange / (FieldLength_Middle * FieldObjList_Middle.Length));

        for (int idx = 0; idx < FieldObjList_Middle.Length; idx++)
        {
            //기본은 시작위치에서 이동한만큼.
            float BaseScrollPos = (FirstPos_Middle + FieldMoveRange_Middle) + (FieldLength_Middle * FieldObjList_Middle.Length) * AddFieldCount_Middle;
            float Pos_X = BaseScrollPos + (FieldLength_Middle * idx);

            //이동한 거리 - 총 길이에서..
            float MoveFieldLength = MiddleScrollRange % (FieldLength_Middle * FieldObjList_Middle.Length);
            if ((idx + 1) * FieldLength_Middle < MoveFieldLength)
                Pos_X += FieldLength_Middle * FieldObjList_Middle.Length;

            FieldObjList_Middle[idx].transform.localPosition = new Vector3(Pos_X, 0.0f, 0.0f);
        }


        //근경은 카메라 좌표를 기준으로 계속 뒤로 붙이면서 약간 늦게 따라간다.

        //근경의 늦춰진 이동속도.
        if (FieldObjList_Near.Length > 0)       //없을수도 있으니 체크.
        {
            float FieldMoveRange_Near = FieldMoveRange * ScrollSpeed_Near;
            float NearScrollRange = FieldMoveRange - FieldMoveRange_Near;
            int AddFieldCount_Near = (int)(NearScrollRange / (FieldLength_Near * FieldObjList_Near.Length));

            for (int idx = 0; idx < FieldObjList_Near.Length; idx++)
            {
                //기본은 시작위치에서 이동한만큼.
                float BaseScrollPos = (FirstPos_Near + FieldMoveRange_Near) + (FieldLength_Near * FieldObjList_Near.Length) * AddFieldCount_Near;
                float Pos_X = BaseScrollPos + (FieldLength_Near * idx);

                //이동한 거리 - 총 길이에서..
                float MoveFieldLength = NearScrollRange % (FieldLength_Near * FieldObjList_Near.Length);
                if ((idx + 1) * FieldLength_Near < MoveFieldLength)
                    Pos_X += FieldLength_Near * FieldObjList_Near.Length;

                FieldObjList_Near[idx].transform.localPosition = new Vector3(Pos_X, 0.0f, 0.0f);
            }
        }



        //필드는 카메라 좌표를 기준으로 계속 뒤로 붙이면서 정지되어있는 상태.
        int AddFieldCount_Field = (int)(FieldMoveRange / (FieldLength_Field * FieldObjList_Field.Length));
        for (int idx = 0; idx < FieldObjList_Field.Length; idx++)
        {
            float BaseScrollPos = (FieldLength_Field * FieldObjList_Field.Length) * AddFieldCount_Field;
            float Pos_X = BaseScrollPos + (FieldLength_Field * idx);

            //이동한 거리 - 총 길이에서..
            float MoveFieldLength = FieldMoveRange % (FieldLength_Field * FieldObjList_Field.Length);
            if ((idx + 1) * FieldLength_Field < MoveFieldLength)
                Pos_X += FieldLength_Field * FieldObjList_Field.Length;

            FieldObjList_Field[idx].transform.localPosition = new Vector3(Pos_X, 0.0f, 0.0f);
        }
	}
}
