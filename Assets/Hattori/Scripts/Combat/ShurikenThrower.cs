using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShurikenThrower : MonoBehaviour {

	public Shuriken shuriken;
	public GameObject shurikenPrefab;

	protected Vector3 initialPosition;

	public Transform beacon;
	public float minScale = 0.2f;
	public float maxScale = 1f;
	public float flyTime = 1f;
	public float rotateSpeed = 0.2f;

	public Ease easeType;
	public float easeAmplitude = 1;
	public float easePeriod = 0;

	public bool aimWithSpeed;
	
	void Awake() {
		initialPosition = transform.position;
		SpawnShuriken ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.T)) {
			ThrowShuriken (beacon.position, flyTime, 0, 0);
		}
	}

	public void DragShuriken(Vector3 position) {
		if (shuriken != null) {
			shuriken.transform.position = position;
		}
	}

	public void DropShuriken() {
		if (shuriken != null) {
			shuriken.collider.SetCollidersActive(false);
			shuriken.transform.DOScale(Vector3.zero, 0.75f);
			Destroy(shuriken, 1);
		}
	}

	public void OnSwipeEnded(float angle, float distance, float duration, float speed) {
//		Debug.Log ("Throw to " + angle + " dist : " + distance + " dur : " + duration + " spd : " + distance / duration);
		float tiltAngle = GetTiltAngle (angle);
		Vector2 offset = Vector2.zero;
		float speedValue = 1;
		float speedPercentage = 0;

		if (aimWithSpeed) {
			offset = UpdateOffset (angle, speed);
			speedPercentage = GetLengthPercentage (distance);
		} else {
			offset = UpdateOffset (angle, distance);
			speedPercentage = GetSpeedPercentage (speed);
		}

		speedValue = GetSpeedValue (speedPercentage);
//		Debug.Log (distance / duration);

		speed = Mathf.Clamp (speed, speedValueRange.x, speedValueRange.y);

//		ThrowShuriken (beacon.position, flyTime * speedValue, tiltAngle, offset.y);
		ThrowShuriken (beacon.position, speed, tiltAngle, 0);

	}




	public void ThrowShuriken(Vector3 destination, float speed, float tiltAngle, float yOffset) {
		if (shuriken != null) {
			shuriken.minScale = minScale;
			shuriken.rotateSpeed = rotateSpeed;
			shuriken.easeType = easeType;

			shuriken.Launch (destination, speed, tiltAngle, yOffset);
		}
		shuriken = null;

		Invoke ("SpawnShuriken", 0.1f);
	}

	protected void SpawnShuriken() {
		if (shuriken == null) {
			shuriken = (Instantiate(shurikenPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Shuriken>(); 
			shuriken.transform.parent = transform;
			shuriken.transform.localScale = new Vector3(maxScale, shuriken.transform.localScale.y, maxScale);
		}
		shuriken.collider.SetCollidersActive (false);
	}

	public Vector2 heightDataRange;
	public Vector2 heightAngleRange;

	public float GetLengthPercentage(float height) {
//		Debug.Log ("Length * " + height);
		height = Mathf.Clamp (height, heightDataRange.x, heightDataRange.y);
//		Debug.Log ("Length ^ " + height);
		
		float percentage = (height - heightDataRange.x) / (heightDataRange.y - heightDataRange.x);
		return percentage;
	}

	public float GetHeightOffset(float percentage) {
//		Debug.Log ("Height % " + percentage);
		return heightAngleRange.x + (heightAngleRange.y - heightAngleRange.x) * percentage;
	}

	public Vector2 tiltDataRange;
	public Vector2 tiltAngleRange;
	
	public float GetTiltAngle(float angle) {
		float sign = Mathf.Sign (angle);
		angle = Mathf.Clamp(Mathf.Abs (angle), tiltDataRange.x, tiltDataRange.y);
		float percentage = (angle - tiltDataRange.x) / (tiltDataRange.y - tiltDataRange.x);
		float tilt = (tiltAngleRange.x + (tiltAngleRange.y - tiltAngleRange.x) * percentage) * sign; 
		return tilt;
	}
	
	public Vector2 speedDataRange;
	public Vector2 speedValueRange;

	public float GetSpeedPercentage(float speed) {
//		Debug.Log ("SwipeSpeed * " + speed);
		speed = Mathf.Clamp (speed, speedDataRange.x, speedDataRange.y);
//		Debug.Log ("SwipeSpeed ^ " + speed);
		float percentage = (speed - speedDataRange.x) / (speedDataRange.y - speedDataRange.x);
		return percentage;
	}

	public float GetSpeedValue(float percentage) {
//		Debug.Log ("Speed % " + percentage);
		float value = (speedValueRange.x + (speedValueRange.y - speedValueRange.x) * percentage); 
		return value;
	}

	public LayerMask borders;

	public Vector2 UpdateOffset(float angle, float heightData) {
		RaycastHit hit = GetWallHit ();
		float wallDistance = hit.distance;
		if (wallDistance == 0) {
			return Vector2.zero;
		}

		Vector2 offset = new Vector2(GetOffsetX (angle, wallDistance), GetOffsetY (heightData));
		UpdateAimPosition (hit.point, offset);

		return offset;
	}

	public void UpdateAimPosition(Vector3 hitPoint, Vector2 offset) {
		beacon.transform.position = hitPoint + new Vector3 (offset.x, offset.y, 0);
	}

	protected RaycastHit GetWallHit() {
		RaycastHit hit;
		if (!Physics.Raycast (shuriken.transform.position, Vector3.forward, out hit, Mathf.Infinity, borders)) {
			Debug.LogError("No wall detected");
		} 
//		Debug.DrawRay (transform.position, Vector3.forward * 2000, Color.white, 4);
		return hit;
	}

	protected float GetOffsetX(float angle, float wallDistance) {
		return Mathf.Atan (angle * Mathf.Deg2Rad) * wallDistance;
	}

	protected float GetOffsetY(float heightData) {
		float percentage = 0;
		if (aimWithSpeed) {
			percentage = GetSpeedPercentage (heightData);
		} else {
			percentage = GetLengthPercentage(heightData);
		}
		heightData = GetHeightOffset (percentage);
		return heightData;
	}
}
