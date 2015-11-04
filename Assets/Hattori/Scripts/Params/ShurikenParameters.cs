using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShurikenParameters : MonoBehaviour {

	public InputField minScale;
	public InputField flyTime;
	public InputField rotateSpeed;

	public Toggle expEase;
	public Toggle sineEase;
	public Toggle linearEase;
	public Toggle quadEase;
	public Toggle cubeEase;
	public Toggle quartEase;

	public InputField easeAmplitude;
	public InputField easePeriod;
	public InputField ninjaSpeed;	
	public InputField playerSpeed;

	public ShurikenThrower thrower;
	public CharacterMovement enemy;
	public CharacterMovement player;

	public Toggle aimWithSpeed;

	public InputField minSpeedData;
	public InputField maxSpeedData;
	public InputField minHeightData;
	public InputField maxHeightData;
	public InputField minSpeedValue;
	public InputField maxSpeedValue;
	public InputField minHeightValue;
	public InputField maxHeightValue;

	public InputField maxTiltAngle;

	void Awake() {
		GetParameters ();
	}
	
	public void GetParameters() {
		minScale.text = thrower.minScale.ToString();
		flyTime.text = thrower.flyTime.ToString();
		rotateSpeed.text = thrower.rotateSpeed.ToString();

		expEase.isOn = thrower.easeType == DG.Tweening.Ease.OutExpo;
		sineEase.isOn = thrower.easeType == DG.Tweening.Ease.OutSine;
		linearEase.isOn = thrower.easeType == DG.Tweening.Ease.Linear;
		quadEase.isOn = thrower.easeType == DG.Tweening.Ease.OutQuad;
		cubeEase.isOn = thrower.easeType == DG.Tweening.Ease.OutCubic;
		quartEase.isOn = thrower.easeType == DG.Tweening.Ease.OutQuart;

		easeAmplitude.text = thrower.easeAmplitude.ToString();
		easePeriod.text = thrower.easePeriod.ToString();
		ninjaSpeed.text = enemy.moveSpeed.ToString ();
		playerSpeed.text = player.moveSpeed.ToString ();

		aimWithSpeed.isOn = thrower.aimWithSpeed;
		
		minSpeedData.text = thrower.speedDataRange.x.ToString ();
		maxSpeedData.text = thrower.speedDataRange.y.ToString ();
		minHeightData.text = thrower.heightDataRange.x.ToString ();
		maxHeightData.text = thrower.heightDataRange.y.ToString ();
		minSpeedValue.text = thrower.speedValueRange.x.ToString ();
		maxSpeedValue.text = thrower.speedValueRange.y.ToString ();
		minHeightValue.text = thrower.heightAngleRange.x.ToString ();
		maxHeightValue.text = thrower.heightAngleRange.y.ToString ();

		maxTiltAngle.text = thrower.tiltAngleRange.y.ToString ();
	}
	
	public void UpdateParameters() {
		
		float.TryParse (minScale.text, out thrower.minScale);
		float.TryParse (flyTime.text, out thrower.flyTime);
		float.TryParse (rotateSpeed.text, out thrower.rotateSpeed);

		if (expEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.InExpo;
		}
		if (sineEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.InSine;
		}
		if (linearEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.Linear;
		}
		if (quadEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.InQuad;
		}
		if (cubeEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.InCubic;
		}
		if (quartEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.InQuart;
		}


		float.TryParse (easeAmplitude.text, out thrower.easeAmplitude);
		float.TryParse (easePeriod.text, out thrower.easePeriod);
		float.TryParse (ninjaSpeed.text, out enemy.moveSpeed);
		float.TryParse (playerSpeed.text, out player.moveSpeed);

		thrower.aimWithSpeed = aimWithSpeed.isOn;

		float.TryParse (minSpeedData.text, out thrower.speedDataRange.x);
		float.TryParse (maxSpeedData.text, out thrower.speedDataRange.y);
		float.TryParse (minHeightData.text, out thrower.heightDataRange.x);
		float.TryParse (maxHeightData.text, out thrower.heightDataRange.y);
		float.TryParse (minSpeedValue.text, out thrower.speedValueRange.x);
		float.TryParse (maxSpeedValue.text, out thrower.speedValueRange.y);
		float.TryParse (minHeightValue.text, out thrower.heightAngleRange.x);
		float.TryParse (maxHeightValue.text, out thrower.heightAngleRange.y);

		float.TryParse (maxTiltAngle.text, out thrower.tiltAngleRange.y);
	}
}
