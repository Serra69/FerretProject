using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Vibration : MonoBehaviour {

	[Header("Intensity of vibrations")]
	[SerializeField] [Range(0, 1)] float m_intensityLeft;	// L'intensité de vibration du côté gauche de la manette
	[SerializeField] [Range(0, 1)] float m_intensityRight;	// L'intensité de vibration du côté droit de la manette

	[Header("Time of vibrations")]
	[SerializeField] float m_vibrationTime;	// Le temps de vibration

	void Start(){
		StartCoroutine(VibrationCorout());	// Lorsque le GameObject contenant le script est instancié, on lance la coroutine "VibrationCorout"
	}

	IEnumerator VibrationCorout(){
		GamePad.SetVibration(0, m_intensityLeft, m_intensityRight);	// Lance la vibration de la manette
		yield return new WaitForSeconds(m_vibrationTime);			// Après le temps d'attent "m_vibrationTime"
		GamePad.SetVibration(0, 0, 0);								// Remet la vibration de la manette à 0
		Destroy(this.gameObject);									// Supprime le GameObject
	}

}
