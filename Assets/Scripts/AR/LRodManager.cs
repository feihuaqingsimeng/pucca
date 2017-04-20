using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LRodManager : MonoBehaviour
{
    public Transform    LRod_Left;
    public Transform    LRod_Right;

    public float        RodMoveAngle_Min = 0.0f;
    public float        RodMoveAngle_Max = 60.0f;

    public float        CurAngle;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (CurAngle <= RodMoveAngle_Min)
            CurAngle = RodMoveAngle_Min;
        if (CurAngle >= RodMoveAngle_Max)
            CurAngle = RodMoveAngle_Max;

        LRod_Left.localRotation = Quaternion.Euler(0.0f, 0.0f, CurAngle);
        LRod_Right.localRotation = Quaternion.Euler(0.0f, 0.0f, -CurAngle);

	}


    public void HideLRod()
    {
        LRod_Left.transform.parent.gameObject.SetActive(false);
        LRod_Right.transform.parent.gameObject.SetActive(false);
    }

    public void ShowLRod()
    {
        LRod_Left.transform.parent.gameObject.SetActive(true);
        LRod_Right.transform.parent.gameObject.SetActive(true);
    }

}
