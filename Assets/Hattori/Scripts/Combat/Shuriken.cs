using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Shuriken : HittableObject {

	public float minScale;
	public Ease easeType;
	public float rotateSpeed;

	public Transform tiltX;
	public Transform tiltY;
	
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


		float time = Vector3.Distance (transform.position, target) / speed;

		transform.DOMove (target, time).SetEase (easeType);
		transform.DOScale (minScale, time).SetEase (easeType);

//		Debug.Log ("Launch : " + target + " spd : " + speed + " tilt : " + tiltAngle + " yOffset : " + yOffset);
		yOffset /= minScale;
		visual.transform.DOLocalRotate (new Vector3 (0, 0, -360), rotateSpeed, RotateMode.FastBeyond360).SetLoops (-1, LoopType.Restart);
		visual.transform.DOLocalMove(new Vector3(0, yOffset, 0), time).SetEase(easeType);
		tiltY.DOLocalRotate (new Vector3 (0, tiltAngle, 0), time * 0.25f, RotateMode.Fast);

		collider.SetCollidersActive (true);
	}
}
