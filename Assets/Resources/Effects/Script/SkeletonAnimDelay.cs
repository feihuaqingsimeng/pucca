using UnityEngine;
using System.Collections;

public class SkeletonAnimDelay : MonoBehaviour
{

	public float delay_time = 1.0f;
	void Start()
	{
		Invoke ("active_anim", delay_time);
	}

	void active_anim()
	{
		gameObject.GetComponent<SkeletonAnimation> ().enabled = true;
	}


}