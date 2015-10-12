using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Shuriken : HittableObject {



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

	public void Launch(Vector3 target, float time, float endScale, float rotateSpeed, Ease easeType, float amplitude = 1, float period = 0) {
		transform.DOMove (target, time).SetEase (easeType, amplitude, period);
		transform.DOScale (endScale, time).SetEase (easeType, amplitude, period);

		visual.transform.DORotate (new Vector3 (0, 0, 360), rotateSpeed, RotateMode.FastBeyond360).SetLoops (-1, LoopType.Restart);
		collider.SetCollidersActive (true);
	}
}
