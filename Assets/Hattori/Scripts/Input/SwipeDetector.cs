using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SwipeDetector : MonoBehaviour {

	public Swipe swipe;
	public Camera camera;

	public ShurikenThrower thrower;

	public Text errorText;

	// Update is called once per frame
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
			swipe.StartSwipe(Input.mousePosition);
			return;
		} 

		if (Input.GetMouseButton (0)) {
			swipe.AddPoint(Input.mousePosition, Time.deltaTime);
			return;
		}

		if (Input.GetMouseButtonUp (0)) {
//			swipe.line.SetColors(Color.blue, Color.blue);
			swipe.EndSwipe();
			return;
		}
	}

	public Vector3 ScreenToWorld(Vector3 screen) {
		Vector3 world = screen;
//		world.x = screen.x;
//		world.z = screen.y;
//		world.y = 0;
		world = camera.ScreenToWorldPoint (world);
		world.z = 0;
//		screen.x = world.x;
//		screen.y = world.z;
//		screen.z = 0;
		return world;
	}

	public Vector3 WorldToScreen(Vector3 world) {
		Vector3 screen = world;
		screen.z = 0;
		screen = camera.WorldToScreenPoint (screen);
		return screen;
	}

	protected List<GameObject> points;
	public void MakePoint(Vector3 position) {
		points.Add(Instantiate (swipe.pointPrefab, position, Quaternion.identity) as GameObject);
	}
}

[System.Serializable]
public class Swipe {

	public SwipeDetector detector;

	protected List<Vector3> points;
	protected Vector2 bottomPoint {
		get {
			return detector.WorldToScreen(detector.thrower.transform.position);
		}
	}
	public float duration { get; protected set; }

	public LineRenderer line;
	public GameObject pointPrefab;
	public float minDistance = 10f;
	public float maxAngle = 35;
	public float maxDeviation = 45;
	public float maxDistancePercentage = 0.05f;

	protected bool isActive = false;
	public bool drawLine = false;
	public bool simplifiedDraw = false;
	public bool flyFromCenter = false;

	public void AddPoint(Vector3 point, float deltaTime) {
		if (!isActive) {
			return;
		}

		if (points.Count == 0 || Vector3.Distance (point, points [points.Count - 1]) >= minDistance) {
			points.Add(point);
			float angle = 0;
			if (points.Count > 2) {
				angle = GetAngleBetweenSegments(points[0], points[1], points[points.Count - 2], points[points.Count - 1]);
			}

			if(angle > maxAngle) {
				points.RemoveAt(points.Count - 1);
//				line.SetColors(Color.red, Color.red);
//				Debug.Log("Stop");
				EndSwipe();
			}
		}



		duration += deltaTime;
	}

	protected float GetAngleBetweenSegments(Vector2 firstSegmentStart, Vector2 firstSegmentEnd, 
	                                        Vector2 secondSegmentStart, Vector2 secondSegmentEnd) {
		float angle = Vector2.Angle((firstSegmentEnd - firstSegmentStart), (secondSegmentEnd - secondSegmentStart));
		return angle;
	}

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

		if (flyFromCenter) {
			points[0] = bottomPoint;
		}

		if (drawLine && !simplifiedDraw) {
			line.SetColors (Color.blue, Color.blue);
			DrawTrajectory ();
		}

		Simplify ();

		if (isValid ()) {
			line.SetColors (Color.green, Color.green);
			float sign = points[points.Count - 1].x <= points[0].x ? -1 : 1;
			detector.thrower.ThrowShurikenWithAngle(GetDeviationAngle() * sign);

		} else {
			line.SetColors (Color.red, Color.red);
		}

		if (drawLine && simplifiedDraw) {
			DrawTrajectory ();
		}

		isActive = false;
	}

	// Test

	protected void DrawTrajectory() {
		line.SetVertexCount (points.Count);
		for(int i = 0; i < points.Count; i++) {
			line.SetPosition(i, detector.ScreenToWorld(points[i]));
			detector.MakePoint(detector.ScreenToWorld(points[i]));
		}
	}

	protected void Simplify() {
		while (this.points.Count > 2) {
			points.RemoveAt(1);
		}
	}

	protected float GetDeviationAngle() {
		float angle = Vector2.Angle ((points [points.Count - 1] - points [0]), Vector2.up);
		return angle;
	}

	protected float GetStartPointDistance() {
		if(points.Count > 0) {
			return Mathf.Abs(points[0].x - bottomPoint.x);
		}
		return Mathf.Infinity;
	}

	protected float GetMaxDistance(float angle, float height) {
		if (angle > 90 || height == 0) {
			return Screen.width * maxDistancePercentage;
		}

		float dist = Mathf.Atan (angle * Mathf.Deg2Rad) * height;
//		Debug.Log ("H : " + height + " A: " + angle + " D: " + dist);
		return dist;
	}

	protected bool isValid() {
		if (points [points.Count - 1].y < points [0].y) {
			LogError("Wrong direction");
			return false;
		}

		float angle = GetDeviationAngle ();

		if (angle > maxDeviation) {
			LogError("Angle too big");
			return false;
		}

		if (GetStartPointDistance() > GetMaxDistance(maxDeviation, points[0].y)) {
			LogError("Distance too big");
			return false;
		}

//		float dir = points[points.Count - 1].x <= points[0].x ? -1 : 1;
//		float sign = 
//		if(Mathf.Sign(dir) != Mathf.Sign(points[0].x - this.bottomPoint.x)) {
//			Debug.Log(dir + " | " + (points[0].x - this.bottomPoint.x));
//			Debug.Log("Wrong direction");
//			return false;
//		}

		LogError (string.Empty);

		return true;
	}

	protected void LogError(string error) {
		detector.errorText.text = error;
	}
}