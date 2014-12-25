using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float speed = 30;

	public GameObject destroyEffect;


	void Start () {
	
	}
	
	void FixedUpdate () {
	
		transform.position += transform.forward * speed * Time.fixedDeltaTime;

	}

	void OnCollisionEnter (Collision col) {

		if (destroyEffect)
			Instantiate(destroyEffect, col.contacts[0].point, Quaternion.LookRotation(transform.position, col.contacts[0].normal)); 


		Destroy(this.gameObject);
	
	}
	
}
