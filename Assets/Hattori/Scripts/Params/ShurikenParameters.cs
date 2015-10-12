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
	public InputField easeAmplitude;
	public InputField easePeriod;
	public InputField ninjaSpeed;	
	public InputField playerSpeed;

	public ShurikenThrower thrower;
	public CharacterMovement enemy;
	public CharacterMovement player;

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

		easeAmplitude.text = thrower.easeAmplitude.ToString();
		easePeriod.text = thrower.easePeriod.ToString();
		ninjaSpeed.text = enemy.moveSpeed.ToString ();
		playerSpeed.text = player.moveSpeed.ToString ();
	}
	
	public void UpdateParameters() {
		
		float.TryParse (minScale.text, out thrower.minScale);
		float.TryParse (flyTime.text, out thrower.flyTime);
		float.TryParse (rotateSpeed.text, out thrower.rotateSpeed);

		if (expEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.OutExpo;
		}
		if (sineEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.OutSine;
		}
		if (linearEase.isOn) {
			thrower.easeType = DG.Tweening.Ease.Linear;
		}

		float.TryParse (easeAmplitude.text, out thrower.easeAmplitude);
		float.TryParse (easePeriod.text, out thrower.easePeriod);
		float.TryParse (ninjaSpeed.text, out enemy.moveSpeed);
		float.TryParse (playerSpeed.text, out player.moveSpeed);
	}
}
