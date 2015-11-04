using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HittableEnemy : HittableObject {


	public override void OnHit (HittableCollider other)
	{
		if (other.owner.IsWeapon ()) {
			visual.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
		}
	}


}
