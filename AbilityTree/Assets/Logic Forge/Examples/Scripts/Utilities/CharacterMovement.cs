using UnityEngine;
using System.Collections;

public class CharacterMovement : Photon.MonoBehaviour {

    public float moveSpeed;
    public float rotationSpeed;


    public Vector3 movePos;
    public Vector3 rotPos;


    CharacterController characterController;


    public LayerMask raycastMovementLayer;


	void Start () 
    {
        movePos = transform.position;
        characterController = GetComponent<CharacterController>();
        if (!photonView.isMine)
        {
            enabled = false;
            return;
        }
		GameObject.FindGameObjectWithTag("CameraController").GetComponent<TopDownCamera>().target = transform;

	}
	
	void Update () 
    {
		if (UIMenuManager.inMenu)
			return;

        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, raycastMovementLayer))
            {
                if (Input.GetKey(KeyCode.Mouse1))
				{
                    movePos = hit.point;
					rotPos = hit.point;
				}

                if (Input.GetKey(KeyCode.Mouse0))
				{
					movePos = transform.position;
                    rotPos = hit.point;
				}
			}
        }
        
	}

    void FixedUpdate()
    {
		if (Vector3.Distance(movePos, transform.position) > 1.1f)
        	characterController.SimpleMove(Vector3.Normalize(movePos - transform.position) * moveSpeed * Time.fixedDeltaTime);


        Quaternion rot = Quaternion.LookRotation(rotPos - transform.position);
		transform.localEulerAngles = new Vector3(0, rot.eulerAngles.y, 0);
    }
}
