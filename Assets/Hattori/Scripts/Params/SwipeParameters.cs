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

	public SwipeDetector detector;

	void Awake() {
		GetParameters ();
	}

	public void GetParameters() {
		minDistance.text = detector.swipe.minDistance.ToString();
		maxAngle.text = detector.swipe.maxAngle.ToString();
		maxDeviation.text = detector.swipe.maxDeviation.ToString();

		debugDraw.isOn = detector.swipe.drawLine;
		simplifiedDraw.isOn = detector.swipe.simplifiedDraw;
		swipeFromCenter.isOn = detector.swipe.flyFromCenter;
	}

	public void UpdateParameters() {
	
		float.TryParse (minDistance.text, out detector.swipe.minDistance);
		float.TryParse (maxAngle.text, out detector.swipe.maxAngle);
		float.TryParse (maxDeviation.text, out detector.swipe.maxDeviation);

		detector.swipe.drawLine = debugDraw.isOn;
		detector.swipe.simplifiedDraw = simplifiedDraw.isOn;
		detector.swipe.flyFromCenter = swipeFromCenter.isOn;
	}

	public GameObject panel;

	public void Toggle() {
		panel.SetActive (!panel.activeSelf);
	}
}
