using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HittableObject : MonoBehaviour {
	public SpriteRenderer visual;
	public HittableCollider collider;
	public bool isBreakable;
	public bool isEnemy;
	public bool isWeapon;

	public void Init() {
		Vector3 newRotation = collider.gameObject.transform.eulerAngles;
		newRotation.x = 90;
		collider.gameObject.transform.eulerAngles = newRotation;

		collider.owner = this;
		collider.SetCollidersActive (true);
	}

	public virtual void OnHit(HittableCollider other) {
//		Debug.Log (other.name);
	}

	public virtual bool IsBreakable() {
		return isBreakable;
	}

	public virtual bool IsEnemy() {
		return isEnemy;
	}

	public virtual bool IsWeapon() {
		return isWeapon;
	}


}
