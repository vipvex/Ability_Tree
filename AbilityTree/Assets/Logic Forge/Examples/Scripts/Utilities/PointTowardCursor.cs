using UnityEngine;
using System.Collections;

public class PointTowardCursor : MonoBehaviour 
{

    public Vector3 offset;

	private Vector3 hitPoint;


	public bool followCursor;
	public bool pointTowardsCursor;


    public LayerMask collideWithLayers;

	void FixedUpdate () 
	{

        if (gameObject.active == false)
            return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, collideWithLayers))
		{

			hitPoint = hit.point;

		}

		if (pointTowardsCursor)
		{
			Quaternion rot = Quaternion.LookRotation(new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - transform.position);
			transform.rotation = rot;
		}

		if (followCursor)
		{
            transform.position = new Vector3(hitPoint.x, transform.root.position.y, hitPoint.z) + offset;
		}

	}
}
