using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwipeParameters : MonoBehaviour {

	public InputField minDistance;
	public InputField maxAngle;
	public InputField maxDeviation;
	public Toggle debugDraw;
	public Toggle simplifiedDraw;
	public Toggle swipeFromCenter;
	public InputField maxSwipeTime;

	public InputField controlAreaRadius;
	public InputField controlAreaHeight;

	public SwipeDetector detector;

	void Awake() {
		GetParameters ();
	}

	public void GetParameters() {
		minDistance.text = detector.minDistance.ToString();
		maxAngle.text = detector.maxAngle.ToString();
		maxDeviation.text = detector.maxDeviation.ToString();

		debugDraw.isOn = detector.drawLine;
		simplifiedDraw.isOn = detector.simplifiedDraw;
		swipeFromCenter.isOn = detector.flyFromCenter;

		maxSwipeTime.text = detector.cutoffDuration.ToString();
		controlAreaHeight.text = (detector.heightTreshold - detector.thrower.transform.position.z).ToString();
		controlAreaRadius.text = detector.maxDistance.ToString();

	}

	public void UpdateParameters() {
	
		float.TryParse (minDistance.text, out detector.minDistance);
		float.TryParse (maxAngle.text, out detector.maxAngle);
		float.TryParse (maxDeviation.text, out detector.maxDeviation);

		detector.drawLine = debugDraw.isOn;
		detector.simplifiedDraw = simplifiedDraw.isOn;
		detector.flyFromCenter = swipeFromCenter.isOn;

		float.TryParse (maxSwipeTime.text, out detector.cutoffDuration);

		float height = 0;
		float.TryParse (controlAreaHeight.text, out height);
		detector.heightTreshold = detector.thrower.transform.position.z + height;

		float.TryParse (controlAreaRadius.text, out detector.maxDistance);

	}

	public GameObject panel;

	public void Toggle() {
		panel.SetActive (!panel.activeSelf);
	}
}
