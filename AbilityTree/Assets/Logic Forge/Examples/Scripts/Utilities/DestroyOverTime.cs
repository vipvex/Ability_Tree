using UnityEngine;
using System.Collections;

public class DestroyOverTime : MonoBehaviour {

	public float lifeTime = 1;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(TimedDestruction());
	}

	IEnumerator TimedDestruction ()
	{

		yield return new WaitForSeconds (lifeTime);
		Destroy(gameObject);

	}

}
