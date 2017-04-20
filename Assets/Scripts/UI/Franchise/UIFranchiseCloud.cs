using UnityEngine;
using System.Collections;

public class UIFranchiseCloud : MonoBehaviour 
{
    private RectTransform   MoveRectTrans;
    private GameObject      MoveTarget;
    private float           MoveSpeed;

    private Vector2         BasePos;
    private float           EndPos;

    private bool            isSettingComplet;

    public void SetBasePos(float endXPos)
    {
        if (MoveTarget == null)
            MoveTarget = this.gameObject;

        if (MoveRectTrans == null)
            MoveRectTrans = MoveTarget.GetComponent<RectTransform>();

        MoveSpeed   = 60;
        EndPos      = endXPos;
        BasePos = new Vector2(-EndPos, MoveRectTrans.anchoredPosition.y);

        isSettingComplet = true;
    }

	private void Update () 
    {
        if (!isSettingComplet)
            return;

        Vector2 TargetPos = MoveRectTrans.anchoredPosition;
        MoveRectTrans.anchoredPosition = new Vector2(TargetPos.x + (MoveSpeed * Time.deltaTime), TargetPos.y);

        if(TargetPos.x >= EndPos)
            MoveRectTrans.anchoredPosition = BasePos;
	}
}
