using UnityEngine;
using System.Collections;

public class DelayActive : MonoBehaviour
{
	public float acvate_time = 3.0f;
	public float destroy_time = 2.0f;
	void Start()
	{
		if (gameObject.activeInHierarchy)
			gameObject.SetActive(false);

		Invoke("playanim",acvate_time);
	}

	void playanim()
	{        
		gameObject.SetActive(true);
		Destroy (this.gameObject, destroy_time);
	}
}