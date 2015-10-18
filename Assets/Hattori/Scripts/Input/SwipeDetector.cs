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


	public float heightTreshold;
	public float speedTreshold;

	public List<Vector3> dragPoints;
	public float dragSpeed {
		get {
			if(dragDistance == 0 || dragDuration == 0) {
				return 0;
			}
			return dragDistance / dragDuration;
		}
	}
	public float dragSpeedWindow = 0.1f;
	public float dragDistance {
		get {
			float dist = 0;
			if(this.dragPoints == null || this.dragPoints.Count < 2) {
				return 0;
			}
			for(int i = 0; i < this.dragPoints.Count - 1; i++) {
				dist += Vector3.Distance(dragPoints[i], dragPoints[i + 1]);
			}

			return dist;
		}
	}
	public float dragDuration;

	public bool drawLine = false;
	public bool simplifiedDraw = false;
	public bool flyFromCenter = false;

	protected Vector3 previousDragLocation;
	protected bool isDragging;

	public void StartDrag() {
		speedTreshold = thrower.speedValueRange.x;

		if (dragPoints == null) {
			dragPoints = new List<Vector3>();
		}
		dragPoints.Clear ();

		if (points == null) {
			points = new List<GameObject>();
		}
		for (int i = 0; i < points.Count; i++) {
			Destroy (points [i]);
		}
		points.Clear();

		isDragging = true;
		thrower.shuriken.transform.position = ScreenToWorld (Input.mousePosition);
		previousDragLocation = thrower.shuriken.transform.position;
		dragPoints.Add (previousDragLocation);
		points.Add (Instantiate (pointPrefab, previousDragLocation, Quaternion.identity) as GameObject);
		StartCoroutine(RemoveDragSegment (dragSpeedWindow, Time.deltaTime));

		dragDuration = 0;
	}

	public void Drag(Vector3 position, float deltaTime) { 
		if (!isDragging) {
			return;
		}
		dragDuration += deltaTime;

		thrower.DragShuriken(position);

		previousDragLocation = position;
		dragPoints.Add (previousDragLocation);
		points.Add (Instantiate (pointPrefab, previousDragLocation, Quaternion.identity) as GameObject);

		StartCoroutine(RemoveDragSegment (dragSpeedWindow, deltaTime));

//		swipe.LogError (dragSpeed.ToString());

//		Debug.Log (dragPoints [dragPoints.Count - 1]. - dragPoints [dragPoints.Count - 2].y);
		if(dragPoints[dragPoints.Count - 1].z > heightTreshold) {
			if (dragSpeed > speedTreshold) {
//				Debug.Log(dragSpeed + " " + dragDistance + " " + dragDuration);

				int pointIndex = dragPoints.Count - 3;
				while(pointIndex >= 0 && GetAngleBetweenSegments(dragPoints[dragPoints.Count - 1], dragPoints[dragPoints.Count - 2], 
				                                                 dragPoints[pointIndex + 1], dragPoints[pointIndex]) < maxDragAngle) {
					pointIndex--;
				}

				pointIndex++;

				swipe.StartSwipe(dragPoints[pointIndex]);
				swipe.AddPoint(dragPoints[dragPoints.Count - 1], dragDuration);
				swipe.speed = dragSpeed;
				swipe.EndSwipe();

				StopDrag();
			} else {
				StopDrag();
			}
		}
	}

	public float maxDragAngle = 10;
	protected float GetAngleBetweenSegments(Vector3 firstSegmentStart, Vector3 firstSegmentEnd, 
	                                        Vector3 secondSegmentStart, Vector3 secondSegmentEnd) {
		float angle = Vector3.Angle((firstSegmentEnd - firstSegmentStart), (secondSegmentEnd - secondSegmentStart));
//		Debug.Log (angle);
		return angle;
	}

	public void StopDrag() {
		if (!isDragging) {
			return;
		}

		isDragging = false;
		if (thrower.shuriken != null) {
			thrower.shuriken.transform.localPosition = Vector3.zero;
		}
	}

	protected void DrawDragPoints(Color color) {

	}
	
	protected IEnumerator RemoveDragSegment(float delay, float duration) {
		yield return new WaitForSeconds (delay);
		if (dragPoints != null && dragPoints.Count > 1) {
			dragPoints.RemoveAt (0);
		}
		dragDuration -= duration;
	}

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
//				swipe.StartSwipe(Input.mousePosition);
				StartDrag();
			} else {
//				swipe.LogError("Out of range");
			}
			return;
		} 

		if (Input.GetMouseButton (0)) {
			//swipe.AddPoint(Input.mousePosition, Time.deltaTime);

			Drag(ScreenToWorld(Input.mousePosition), Time.deltaTime);
//			if(cutoffLength > 0 && GetSwipeWorldLength(swipe) > cutoffLength) {
//				swipe.EndSwipe();
//			} else if(cutoffDuration > 0 && swipe.duration > cutoffDuration) {
//				swipe.EndSwipe();
//			}

			return;
		}

		if (Input.GetMouseButtonUp (0)) {
			StopDrag();
//			swipe.EndSwipe();
			return;
		}
	}

	public void OnSwipeEnded(Swipe swipe) {
		Debug.Log ("End swipe " + swipe.points.Count);
		if (drawLine && !simplifiedDraw) {
			Debug.Log("Long draw");
			DrawSwipe (swipe, Color.blue);
		}
		swipe.Simplify ();
		if (drawLine && simplifiedDraw) {
			Debug.Log("Short draw");
			DrawSwipe (swipe, Color.blue);
		}
		thrower.OnSwipeEnded (swipe.GetDeviationAngle () * swipe.GetDirection(), swipe.length, swipe.duration, swipe.speed);
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

//		for (int i = 0; i < points.Count; i++) {
//			Destroy(points[i]);	
//		}
//		points.Clear ();

		line.SetVertexCount (swipe.points.Count);
		for(int i = 0; i < swipe.points.Count; i++) {
			line.SetPosition(i, swipe.points[i]);
//			MakePoint(swipe.points[i]);
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
	public float speed;
	
	public float length {
		get {
			//			return Vector3.Distance (detector.ScreenToWorld (points [0]), detector.ScreenToWorld (points [points.Count - 1]));
//			Debug.Log(points[0] + " " + points[points.Count - 1] + " " + Vector3.Distance(points[0], points[points.Count - 1]));
			return Vector3.Distance(points[0], points[points.Count - 1]);
		}
	}




	protected bool isActive = false;

	public void StartSwipe(Vector3 point) {
		if (points == null) {
			points = new List<Vector3>();
		}
		points.Clear ();
		duration = 0;
//		Debug.Log ("Swipe started");
		isActive = true;
		AddPoint (point, 0);

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

		if (points.Count > 0) {
			Debug.Log ("Gonna add " + point + " at : " + Vector3.Distance (point, points [points.Count - 1]));
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

//		Debug.Log(points [points.Count - 1] - points [0]);
		float angle = Vector3.Angle ((points [points.Count - 1] - points [0]), Vector3.forward);
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