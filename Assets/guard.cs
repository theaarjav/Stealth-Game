using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guard : MonoBehaviour {

	public static event System.Action onPlayerSpotted;

	public Transform pathHolder;
	public float speed = 5;
	public float waitTime = 0.3f;
	public float angularspeed = 90;
	public float timeToRun = 0.5f;

	Transform player;

	float playerVisibleTimer;
	public Light guardlight;
	public float viewdistance;
	float viewAngle;

	public LayerMask viewMask;
	Color originalspotlightcolor;
	// Use this for initialization
	void OnDrawGizmos(){
		Vector3 startPos = pathHolder.GetChild (0).position;
		Vector3 prevPos = startPos;

		foreach (Transform waypoint in pathHolder) {
			Gizmos.DrawSphere (waypoint.position, 0.3f);
			Gizmos.DrawLine (prevPos, waypoint.position);
			prevPos = waypoint.position;

		}
		Gizmos.DrawLine (prevPos, startPos);

		Gizmos.color = Color.red;
		Gizmos.DrawRay (transform.position, transform.forward*viewdistance);

	}
	void Start () {
		originalspotlightcolor = guardlight.color;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		viewAngle = guardlight.spotAngle;

		Vector3[] waypoints = new Vector3[pathHolder.childCount];
		for (int i = 0; i < waypoints.Length; i++) {
			waypoints [i] = pathHolder.GetChild (i).position;
			waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
		}
		StartCoroutine (guardPath(waypoints));

	}

	bool PlayerSpotted(){
		if (Vector3.Distance (transform.position, player.position) < viewdistance) {
			Vector3 dirToPlayer = (player.position - transform.position).normalized;
			float spotAngle = Vector3.Angle (transform.forward, dirToPlayer);
			if (spotAngle < viewAngle / 2f) {
				if (!Physics.Linecast (transform.position, player.position, viewMask)) {
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	IEnumerator guardPath(Vector3[] wayp){
		transform.position = wayp [0];
		int targetWaypointIndex = 1;
		Vector3 targetWayp = wayp [targetWaypointIndex];
		while (true) {
			transform.position = Vector3.MoveTowards (transform.position, targetWayp, speed* Time.deltaTime);
			if (transform.position == targetWayp) {
				targetWaypointIndex = (targetWaypointIndex + 1) % wayp.Length;
				targetWayp = wayp [targetWaypointIndex];
				yield return new WaitForSeconds (waitTime);

			}
			yield return StartCoroutine (guardRotate(targetWayp));
			yield return null; 
		}
	}
	IEnumerator guardRotate (Vector3 target){
		Vector3 dirToTarget = (target - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2 (dirToTarget.z, dirToTarget.x) * Mathf.Rad2Deg;

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y,targetAngle))>0.0005f) {
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, angularspeed * Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
		}
	}
	// Update is called once per frame
	void Update () {
		if (PlayerSpotted()) {
			playerVisibleTimer += Time.deltaTime;
			//guardlight.color = Color.red;
		} else {
			playerVisibleTimer -= Time.deltaTime;
			//guardlight.color = originalspotlightcolor;
		}
		playerVisibleTimer = Mathf.Clamp (playerVisibleTimer, 0, timeToRun);
		guardlight.color = Color.Lerp (originalspotlightcolor, Color.red, playerVisibleTimer / timeToRun);
	
		if (playerVisibleTimer >= timeToRun) {
			if (onPlayerSpotted != null) {
				onPlayerSpotted ();
			}
		}
	}
}
