using UnityEngine;
using System.Collections;

public class AwakePlaySpine : MonoBehaviour
{
    public  SkeletonAnimation   SpineAnimation;

    void OnEnable ()
    {
        PlaySpineAnimation();
	}

    void PlaySpineAnimation()
    {
        SpineAnimation.Reset();
    }




}
