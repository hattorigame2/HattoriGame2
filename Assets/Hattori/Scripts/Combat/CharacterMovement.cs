using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
	
	public float moveSpeed;
	public Transform left;
	public Transform right;
	
	protected float dir;
	public float changeChance = 0.5f;

	void Start() {
		dir = Random.Range (0, 1f) < 0.5f ? -1 : 1;
		Invoke ("ChangeDir", Random.Range(0.5f, 1f)); 
	}
	
	void Update() {
		transform.Translate(new Vector3(dir * this.moveSpeed * Time.deltaTime, 0, 0));
		
		if(transform.position.x > right.position.x) {
			transform.position = right.position;
			dir = -dir;
		}
		
		if(transform.position.x < left.position.x) {
			transform.position = left.position;
			dir = -dir;
		}
	}
	
	protected void ChangeDir() {
		CancelInvoke("ChangeDir");
		if(Random.Range(0, 1f) < changeChance) {
			dir = -dir;
		}
		Invoke ("ChangeDir", Random.Range(0.05f, 1f)); 
	}
}
