using UnityEngine;
using System.Collections;

public class HittableCollider : MonoBehaviour {

	public Collider[] colliders;
	public HittableObject owner;

	protected void FindColliders() {
		colliders = GetComponents<Collider>(); 
	}

	public void SetCollidersActive(bool active) {
		if (colliders == null) {
			FindColliders();
		}

		for (int i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = active;
		}
	}

	void OnTriggerEnter(Collider other) {
		var hit = other.GetComponent<HittableCollider> ();
		if (owner != null && hit != null) {
			owner.OnHit(hit);
		}
	}
}
