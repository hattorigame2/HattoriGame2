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
	}

	public void UpdateParameters() {
	
		float.TryParse (minDistance.text, out detector.minDistance);
		float.TryParse (maxAngle.text, out detector.maxAngle);
		float.TryParse (maxDeviation.text, out detector.maxDeviation);

		detector.drawLine = debugDraw.isOn;
		detector.simplifiedDraw = simplifiedDraw.isOn;
		detector.flyFromCenter = swipeFromCenter.isOn;

		float.TryParse (maxSwipeTime.text, out detector.cutoffDuration);
	}

	public GameObject panel;

	public void Toggle() {
		panel.SetActive (!panel.activeSelf);
	}
}
