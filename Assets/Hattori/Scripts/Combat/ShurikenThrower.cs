using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShurikenThrower : MonoBehaviour {

	protected Shuriken shuriken;
	public GameObject shurikenPrefab;

	protected Vector3 initialPosition;

	public Transform beacon;
	public float minScale = 0.2f;
	public float flyTime = 1f;
	public float rotateSpeed = 0.2f;

	public Ease easeType;
	public float easeAmplitude = 1;
	public float easePeriod = 0;




	void Awake() {
		initialPosition = transform.position;
		SpawnShuriken ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.T)) {
			ThrowShuriken (beacon.position, minScale, flyTime);
		}
	}

	public void ThrowShurikenWithAngle(float angle) {
		if (UpdateAimPosition (angle)) {
			ThrowShuriken (beacon.position, minScale, flyTime);
		}
	}

	public void ThrowShuriken(Vector3 destination, float scale, float time) {
		if (shuriken != null) {
			shuriken.Launch (destination, time, scale, rotateSpeed, easeType, easeAmplitude, easePeriod);
		}
		shuriken = null;

		Invoke ("SpawnShuriken", 0.1f);
	}

	protected void SpawnShuriken() {
		if (shuriken == null) {
			shuriken = (Instantiate(shurikenPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Shuriken>(); 
			shuriken.transform.parent = transform;
		}
		shuriken.collider.SetCollidersActive (false);
	}


	public LayerMask borders;
	public bool UpdateAimPosition(float angle) {
		float wallDistance = 0;
		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.up, out hit, Mathf.Infinity, borders)) {
			wallDistance = hit.distance;
		} else {
			Debug.LogError("No wall detected");
			return false;
		}
	
		float xOffset = Mathf.Atan (angle * Mathf.Deg2Rad) * wallDistance;
		beacon.transform.position = hit.point + new Vector3 (xOffset, 0, 0);
		return true;
	}
}
