using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SwipeDetector : MonoBehaviour {

	public Swipe swipe;
	public Camera camera;

	public ShurikenThrower thrower;

	public Text errorText;

	public float minDistance = 10f;
	public float maxAngle = 35;
	public float maxDeviation = 45;
	public float maxDistance = 1f;
	public float cutoffDuration = 0;
	public float cutoffLength = 0;

	public bool drawLine = false;
	public bool simplifiedDraw = false;
	public bool flyFromCenter = false;

	
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if(points == null) {
				points = new List<GameObject>();
			}
			for(int i = 0; i < points.Count; i++) {
				Destroy(points[i]);
			}
			points.Clear();

			swipe.detector = this;

			if(IsPointWithinRange(Input.mousePosition)) {
				swipe.StartSwipe(Input.mousePosition);
			} else {
				swipe.LogError("Out of range");
			}
			return;
		} 

		if (Input.GetMouseButton (0)) {
			swipe.AddPoint(Input.mousePosition, Time.deltaTime);

			if(cutoffLength > 0 && GetSwipeWorldLength(swipe) > cutoffLength) {
				swipe.EndSwipe();
			} else if(cutoffDuration > 0 && swipe.duration > cutoffDuration) {
				swipe.EndSwipe();
			}

			return;
		}

		if (Input.GetMouseButtonUp (0)) {
			swipe.EndSwipe();
			return;
		}
	}

	public void OnSwipeEnded(Swipe swipe) {
		if (drawLine && !simplifiedDraw) {
			DrawSwipe (swipe, Color.blue);
		}
		swipe.Simplify ();
		if (drawLine && simplifiedDraw) {
			DrawSwipe (swipe, Color.blue);
		}
		thrower.OnSwipeEnded (swipe.GetDeviationAngle () * swipe.GetDirection(), GetSwipeWorldLength(swipe), swipe.duration);
	}


	public float GetSwipeWorldLength(Swipe swipe) {
//		Debug.Log (swipe.length);
		return Vector3.Distance (ScreenToWorld (swipe.points [0]), ScreenToWorld (swipe.points [swipe.points.Count - 1]));
	}


	public bool IsPointWithinRange(Vector3 startPoint) {
		Vector3 swipeWorld = ScreenToWorld (startPoint);
		swipeWorld.y = thrower.transform.position.y;
		float dist = Vector3.Distance (swipeWorld, thrower.transform.position);
		return dist <= maxDistance;
	}

	public Vector3 ScreenToWorld(Vector3 screen) {
		screen = camera.ScreenToWorldPoint (screen);
		screen.y = 0;
		return screen;
	}

	public Vector3 WorldToScreen(Vector3 world) {
		world.y = 0;
		world = camera.WorldToScreenPoint (world);
		return world;
	}

	protected List<GameObject> points;
	public LineRenderer line;
	public GameObject pointPrefab;

	public void DrawSwipe(Swipe swipe, Color color) {
		if (swipe.points == null || swipe.points.Count < 2) {
			return;
		}

		for (int i = 0; i < points.Count; i++) {
			Destroy(points[i]);	
		}
		points.Clear ();

		line.SetVertexCount (swipe.points.Count);
		for(int i = 0; i < swipe.points.Count; i++) {
			line.SetPosition(i, ScreenToWorld(swipe.points[i]));
			MakePoint(ScreenToWorld(swipe.points[i]));
		}

		line.SetColors (color, color);
	}

	public void MakePoint(Vector3 position) {
		points.Add(Instantiate (pointPrefab, position, Quaternion.identity) as GameObject);
	}

}

[System.Serializable]
public class Swipe {

	public SwipeDetector detector;

	public List<Vector3> points { get; protected set; }
	public float duration { get; protected set; }
	public float speed {
		get {
			return length / duration;
		}
	}
	
	public float length {
		get {
			//			return Vector3.Distance (detector.ScreenToWorld (points [0]), detector.ScreenToWorld (points [points.Count - 1]));
			return Vector2.Distance(points[0], points[points.Count - 1]);
		}
	}




	protected bool isActive = false;

	public void StartSwipe(Vector3 point) {
		if (points == null) {
			points = new List<Vector3>();
		}
		points.Clear ();
		duration = 0;
		AddPoint (point, 0);
		isActive = true;
	}
	
	public void EndSwipe() {
		if (!isActive) {
			return;
		}
		
		isActive = false;

		detector.OnSwipeEnded (this);
	}

	public void AddPoint(Vector3 point, float deltaTime) {
		if (!isActive) {
			return;
		}

		if (points.Count == 0 || Vector3.Distance (point, points [points.Count - 1]) >= detector.minDistance) {
			points.Add(point);
			float angle = 0;
			if (points.Count > 2) {
				angle = GetAngleBetweenSegments(points[0], points[1], points[points.Count - 2], points[points.Count - 1]);
				if(angle > detector.maxAngle) {
					points.RemoveAt(points.Count - 1);
					EndSwipe();
				}
			}
		}
		duration += deltaTime;
	}

	protected float GetAngleBetweenSegments(Vector2 firstSegmentStart, Vector2 firstSegmentEnd, 
	                                        Vector2 secondSegmentStart, Vector2 secondSegmentEnd) {
		float angle = Vector2.Angle((firstSegmentEnd - firstSegmentStart), (secondSegmentEnd - secondSegmentStart));
		return angle;
	}

	public void Simplify() {
		while (this.points.Count > 2) {
			points.RemoveAt(1);
		}
	}

	public float GetDeviationAngle() {
		if(points == null || points.Count < 2) {
			return 0;
		}

		float angle = Vector2.Angle ((points [points.Count - 1] - points [0]), Vector2.up);
		return angle;
	}

	public float GetDirection() {
		if(points == null || points.Count < 2) {
			return 0;
		}

		return points [0].x <= points [points.Count - 1].x ? 1 : -1;
	}

	protected bool isValid() {
		if (points [points.Count - 1].y < points [0].y) {
			LogError("Wrong direction");
			return false;
		}

		LogError (string.Empty);

		return true;
	}

	public void LogError(string error) {
		detector.errorText.text = error;
	}
}