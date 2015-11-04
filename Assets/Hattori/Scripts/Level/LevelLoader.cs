using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

	public HittableObject[] objects;

	// Use this for initialization
	void Start () {
		objects = GetComponentsInChildren<HittableObject>();
		for (int i = 0; i < objects.Length; i++) {
			objects[i].Init();
		}
	}

}
