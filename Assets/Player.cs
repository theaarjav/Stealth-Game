using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public event System.Action OnFinish;
	public float moveSpeed = 7;
	public float smoothMoveTime =  0.1f;
	public float turnSpeed = 8;

	float angle;
	float smoothInpMag;
	float smoothMoveVelocity;

	Vector3 velocity;

	Rigidbody rigidbody;
	bool disabled;

	void Start(){
		rigidbody = GetComponent<Rigidbody> ();
		guard.onPlayerSpotted  += Disable;
	}

	void OnTriggerEnter(Collider hitcollider){
		if (hitcollider.tag == "Finish") {
			Disable ();
			if (OnFinish != null) {
				OnFinish ();
			}
		}	
	}

	void Update () {
		Vector3 inputDir = Vector3.zero;
		if(!disabled){
		 inputDir = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
		}float inputMag = inputDir.magnitude;
		smoothInpMag = Mathf.SmoothDamp (smoothInpMag, inputMag, ref smoothMoveVelocity, smoothMoveTime);

		float targetAngle = Mathf.Atan2 (inputDir.x, inputDir.z)*Mathf.Rad2Deg;
		angle = Mathf.LerpAngle (angle, targetAngle, turnSpeed * Time.deltaTime * inputMag);
		//transform.eulerAngles = Vector3.up * angle;
		velocity = transform.forward * moveSpeed * smoothInpMag;

		transform.Translate (transform.forward * moveSpeed * Time.deltaTime*smoothInpMag, Space.World);
	}
	void Disable(){
		disabled = true;
	}

	void FixedUpdate()
	{
		rigidbody.MoveRotation (Quaternion.Euler (Vector3.up * angle));
		rigidbody.MovePosition (rigidbody.position + velocity * Time.deltaTime);
	}
	void OnDestroy(){
		guard.onPlayerSpotted -= Disable;
	}
}

	