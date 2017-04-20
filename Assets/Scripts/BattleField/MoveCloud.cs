using UnityEngine;
using System.Collections;

public class MoveCloud : MonoBehaviour
{
    public  Vector3     MoveDir;
    public  GameObject  MoveTarget;
    public  float       MoveRange;
    public  float       MoveSpeed;

    private Vector3     BasePos;

	void Awake()
    {
        BasePos = new Vector3(MoveTarget.transform.localPosition.x, MoveTarget.transform.localPosition.y, MoveTarget.transform.localPosition.z);
	}
	
	void Update()
    {
        Vector3 TargetPos = MoveTarget.transform.localPosition;
        MoveTarget.transform.localPosition = new Vector3(TargetPos.x + (MoveDir.x * (MoveSpeed * Time.deltaTime)),
                                                    TargetPos.y + (MoveDir.y * (MoveSpeed * Time.deltaTime)), 0.0f);

        float CurDistance = Vector2.Distance(BasePos, MoveTarget.transform.localPosition);
        if (CurDistance >= MoveRange)
        {
            MoveTarget.transform.localPosition = BasePos;
        }

	}
}
