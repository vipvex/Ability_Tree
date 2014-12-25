using UnityEngine;
using System.Collections;

public class TopDownCamera : MonoBehaviour {


	public bool follow = true;
	public Transform target;
	public Vector3 cameraOffset = new Vector3(0, 0, -5);


    public float speedX = 5;
    public float speedY = 5;
    public float zoom = 5;

    public float cameraHeight = 15;

    public float edgeDistance = 30;

	
	void FixedUpdate () 
    {

		if (follow == false)
		{
        
			Vector2 mousePos = Input.mousePosition;

			if (Screen.height - edgeDistance < mousePos.y && mousePos.y > -edgeDistance)
	            Move(Vector3.forward);

			if (edgeDistance > mousePos.y && mousePos.y < Screen.height + edgeDistance)
	            Move(-Vector3.forward);

			if (Screen.width - edgeDistance < mousePos.x && mousePos.x > -edgeDistance)
	            Move(Vector3.right);

			if (edgeDistance > mousePos.x && mousePos.x < Screen.height + edgeDistance)
	            Move(-Vector3.right);

		}

		if (follow && target)
			transform.position = new Vector3(target.position.x + cameraOffset.x, target.position.y + cameraOffset.y, target.position.z + cameraOffset.z);

	}

    void Move(Vector3 dir)
    {
        transform.position += transform.right * dir.x * speedX * Time.fixedDeltaTime;
        transform.position += transform.forward * dir.z * speedY * Time.fixedDeltaTime;
        transform.position += transform.up * dir.y * zoom * Time.fixedDeltaTime;
   	}

}
