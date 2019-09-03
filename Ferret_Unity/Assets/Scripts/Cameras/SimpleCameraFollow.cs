using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour {

	[SerializeField] Transform m_target;
	[SerializeField] float m_startVectorUp = 1;
	[Space]
	public ChangeVectorUp m_changeVectorUp;
	[System.Serializable] public class ChangeVectorUp {
		public float m_additionnalVectorUp = 3;
		public float m_timeToChange = 1;
		public AnimationCurve m_changeSpeedCurve;
	}

	float m_currentVectorUp;

	void LateUpdate(){
		transform.LookAt(m_target.position + Vector3.up * m_currentVectorUp, Vector3.up);
	}

	public void StartChangeVectorUpValueCorut(){
		m_currentVectorUp = m_startVectorUp;
		StartCoroutine(ChangeVectorUpValue(m_changeVectorUp.m_additionnalVectorUp, m_changeVectorUp.m_timeToChange));
	}

	IEnumerator ChangeVectorUpValue(float newVectorUp, float timeToChange){

		float startVectorUp = m_currentVectorUp;
		float distance = Mathf.Abs(startVectorUp - newVectorUp);
		float vitesse = distance / timeToChange;
		float moveFracJourney = new float();

		while(m_currentVectorUp != newVectorUp){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			m_currentVectorUp = Mathf.Lerp(startVectorUp, newVectorUp, m_changeVectorUp.m_changeSpeedCurve.Evaluate(moveFracJourney));
			yield return null;
		}
	}
	
}
