using UnityEngine;
using System.Collections;

public class SimplePosSync : Photon.MonoBehaviour 
{

	private Vector3 nextPos;
	private Quaternion nextRot;


	public float posInterpSpeed = 8;
    public float rotInterpSpeed = 8;


	void Start (){

        nextPos = transform.position;
        nextRot = transform.rotation;

		if (photonView.isMine){
			enabled = false;
			return;
		}

	}

	void FixedUpdate()
	{
        transform.position = Vector3.Lerp(transform.position, nextPos, Time.fixedDeltaTime * posInterpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRot, Time.fixedDeltaTime * rotInterpSpeed);
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            // Network player, receive data 
            nextPos = (Vector3)stream.ReceiveNext();
            nextRot = (Quaternion)stream.ReceiveNext();
        }
	}
}