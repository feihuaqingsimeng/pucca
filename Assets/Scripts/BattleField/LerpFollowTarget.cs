using UnityEngine;
using System.Collections;

public class LerpFollowTarget : MonoBehaviour
{
    private bool        FixFollowMode;
    public  float       NextPos_X;
    public  float       FollowSpeed;

    void Awake()
    {

    }


    public void SetFollowTarget(float pNextPos_X, bool FixFollow = false)
    {
        NextPos_X = pNextPos_X;
        FixFollowMode = FixFollow;

        if(FixFollow)
            transform.position = new Vector3(NextPos_X, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (FixFollowMode)
            return;

        float CurPos_X = transform.position.x;
        float TargetPos_X = NextPos_X;

        float Distance = TargetPos_X - CurPos_X;
        if (Distance < 0.0f)
        {
            CurPos_X -= FollowSpeed * Time.deltaTime;
            if (CurPos_X <= TargetPos_X)
                CurPos_X = TargetPos_X;
        }
        else if (Distance > 0.0f)
        {
            CurPos_X += FollowSpeed * Time.deltaTime;
            if (CurPos_X >= TargetPos_X)
                CurPos_X = TargetPos_X;
        }
        else
            CurPos_X = TargetPos_X;

        transform.position = new Vector3(CurPos_X, transform.position.y, transform.position.z);
    }

}
