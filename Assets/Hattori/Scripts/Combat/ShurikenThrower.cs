using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShurikenThrower : MonoBehaviour {

	public Shuriken shuriken;
	public GameObject shurikenPrefab;

	protected Vector3 initialPosition;

	public Transform beacon;
	public float minScale = 0.2f;
	public float flyTime = 1f;
	public float rotateSpeed = 0.2f;

	public Ease easeType;
	public float easeAmplitude = 1;
	public float easePeriod = 0;

	public Vector3 shurikenPosition;

	public bool aimWithSpeed;

	void Awake() {
		initialPosition = transform.position;
		SpawnShuriken ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.T)) {
			ThrowShuriken (beacon.position, flyTime, 0, 0);
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			shuriken.transform.position = transform.position + shurikenPosition;
			shuriken.visual.transform.localScale = Vector3.one * 0.8f;
			shuriken.tiltX.transform.localEulerAngles = new Vector3(90, 0, 0);
			shuriken.visual.transform.localEulerAngles = Vector3.zero;
		}

//		if (Input.GetKeyDown (KeyCode.L)) {
//			shuriken.MoveToLaunchPoint (this);
//		}
	}

	public void ForceLaunchShuriken() {
		ThrowShuriken (beacon.position, flyTime, 0, 0);
	}

	public void OnSwipeEnded(float angle, float distance, float duration) {
		Debug.Log ("Throw to " + angle + " dist : " + distance + " dur : " + duration + " spd : " + distance / duration);
		float tiltAngle = GetTiltAngle (angle);
		Vector2 offset = Vector2.zero;
		float speed = distance / duration;
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

		ThrowShuriken (beacon.position, flyTime * speedValue, tiltAngle, offset.y);
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
			shuriken = (Instantiate(shurikenPrefab, transform.position + shurikenPosition, Quaternion.identity) as GameObject).GetComponent<Shuriken>(); 
			shuriken.transform.parent = transform;

			shuriken.launchPoint = launchPoint;
			shuriken.lpSpeed = lpSpeed;
			shuriken.lpAngle = lpAngle;
			shuriken.lpScale = lpScale;
			shuriken.lpEase = lpEase;
			shuriken.lpRotAngle = lpRotAngle;
			shuriken.lpScaleDecrease = lpScaleDecrease;
		}
		shuriken.collider.SetCollidersActive (false);
	}

	public Vector2 heightDataRange;
	public Vector2 heightAngleRange;

	public float GetLengthPercentage(float height) {
		Debug.Log ("Length * " + height);
		height = Mathf.Clamp (height, heightDataRange.x, heightDataRange.y);
		Debug.Log ("Length ^ " + height);
		
		float percentage = (height - heightDataRange.x) / (heightDataRange.y - heightDataRange.x);
		return percentage;
	}

	public float GetHeightOffset(float percentage) {
		Debug.Log ("Height % " + percentage);
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
		Debug.Log ("SwipeSpeed * " + speed);
		speed = Mathf.Clamp (speed, speedDataRange.x, speedDataRange.y);
		Debug.Log ("SwipeSpeed ^ " + speed);
		float percentage = (speed - speedDataRange.x) / (speedDataRange.y - speedDataRange.x);
		return percentage;
	}

	public float GetSpeedValue(float percentage) {
		Debug.Log ("Speed % " + percentage);
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
		if (!Physics.Raycast (transform.position, Vector3.forward, out hit, Mathf.Infinity, borders)) {
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

	public Transform launchPoint;
	public float lpSpeed;
	public float lpAngle;
	public float lpScale;
	public Ease lpEase;
	public float lpRotAngle;
	public float lpScaleDecrease;
}
