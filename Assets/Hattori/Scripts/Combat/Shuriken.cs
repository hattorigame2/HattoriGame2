using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Shuriken : HittableObject {

	public float minScale;
	public Ease easeType;
	public float rotateSpeed;

	public Transform tiltX;
	public Transform tiltY;

	public float topScale;
	public float bottomScale;
	public float topHeight = 5;
	public float bottomHeight = -5;

	public override void OnHit (HittableCollider other)
	{
//		Debug.Log ("Shuriken hit " + other);
		if (!other.owner.IsBreakable ()) {
			Stop();
		}
	}

	protected void Stop() {
		transform.DOKill ();

		visual.transform.DOScale (Vector3.one * 2, 0.3f).SetEase(Ease.OutExpo);
		visual.DOFade (0, 0.25f).SetEase(Ease.OutExpo);
		this.collider.SetCollidersActive (false);
		Destroy (gameObject, 0.5f);
	}

	public void Launch(Vector3 target, float speed, float tiltAngle, float yOffset) {
//		yOffset = 0;

		float time = Vector3.Distance (transform.position, target) / speed;

		transform.DOMove (target, time).SetEase (easeType);
		transform.DOScale (minScale, time).SetEase (easeType);
		visual.transform.DOScale (Vector3.one * 0.8f, lpScaleDecrease).SetEase(lpEase);

		Debug.Log ("Launch : " + target + " spd : " + speed + " tilt : " + tiltAngle + " yOffset : " + yOffset);
		yOffset /= minScale;
		visual.transform.DOLocalRotate (new Vector3 (0, 0, -270), rotateSpeed, RotateMode.FastBeyond360).SetLoops (-1, LoopType.Restart);
		visual.transform.DOLocalMove(new Vector3(0, yOffset, 0), time).SetEase(easeType);
		tiltY.DOLocalRotate (new Vector3 (0, tiltAngle, 0), time * 0.25f, RotateMode.Fast);

		collider.SetCollidersActive (true);
	}

	public void Update() {

	}


	public Transform launchPoint;
	public float lpSpeed;
	public float lpAngle;
	public float lpScale;
	public Ease lpEase;
	public float lpRotAngle;
	public float lpScaleDecrease;

	void Start() {
		startPos = transform.position;
		startRot = tiltX.transform.localEulerAngles;
	}

//	public void MoveToLaunchPoint(SwipeDetector thrower) {
//		transform.DOMove (launchPoint.position, lpSpeed).SetEase(lpEase).OnComplete(thrower.OnShurikenArrived);
//		visual.transform.DOScale (Vector3.one * lpScale, lpSpeed).SetEase(lpEase);
//		tiltX.transform.DOLocalRotate (new Vector3 (lpAngle, 0, 0), lpSpeed).SetEase(lpEase);
//		visual.transform.DOLocalRotate (new Vector3 (0, 0, lpRotAngle), lpSpeed).SetEase(lpEase);
//	}

	protected Vector3 startPos;
	protected Vector3 startRot;

	public void UpdateShurikenLaunch(float percentage) {
		transform.position = Vector3.Lerp (startPos, launchPoint.position, percentage);
		visual.transform.localScale = Vector3.Lerp (Vector3.one * 0.8f, Vector3.one * lpScale, percentage);
		tiltX.transform.localEulerAngles = Vector3.Lerp (startRot, new Vector3 (lpAngle, 0, 0), percentage);
		visual.transform.localEulerAngles = Vector3.Lerp (Vector3.zero, new Vector3 (0, 0, lpRotAngle), percentage);
	}

}
