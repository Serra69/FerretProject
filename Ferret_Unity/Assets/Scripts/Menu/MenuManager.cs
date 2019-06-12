using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	[Header("Time to iddle")]
	[SerializeField] float m_miniTimeToSecondIdle = 7.5f;
	[SerializeField] float m_maxTimeToSecondIdle = 15f;

	[Header("Animations")]
	[SerializeField] Animator m_ferretAnimator;
	[SerializeField] Animator m_fadeAnimator;

	[Header("GameObject")]
	[SerializeField] float m_firstTimeToEndFade = 1;
	[SerializeField] GameObject[] m_firstObjectToEnableDisable;
	[Space]
	[SerializeField] float m_secondTimeToEndFade = 2;
	[SerializeField] GameObject[] m_secondObjectToEnableDisable;

	[Header("Change camera")]
	[SerializeField] Transform m_cameraToMove;
	[Space]
	[SerializeField] Transform m_mainPosition;
	[SerializeField] Transform m_secondPosition;
	[Space]
	[SerializeField] float m_positionSpeed = 5;
	[SerializeField] float m_rotationSpeed = 5;
	[Space]
	[SerializeField] AnimationCurve m_positionCurve;
	[SerializeField] AnimationCurve m_rotationCurve;
	[Space]
	[SerializeField] GameObject[] m_mainCanvas;
	[SerializeField] GameObject[] m_secondCanvas;

	PlayerManager m_playerManager;
	bool m_inMainPosition = true;

	void Start(){
		m_playerManager = PlayerManager.Instance;
		StartCoroutine(WaitIdle());
	}
	
	IEnumerator WaitIdle(){
		float random = Random.Range(m_miniTimeToSecondIdle, m_maxTimeToSecondIdle);
		yield return new WaitForSeconds(random);
		m_ferretAnimator.SetTrigger("Iddle");
		StartCoroutine(WaitIdle());
	}

	public void Play(){
		StartCoroutine(PlayCorout());
	}
	IEnumerator PlayCorout(){
		m_fadeAnimator.SetTrigger("Fade");
		yield return new WaitForSeconds(m_firstTimeToEndFade);
		m_playerManager.ChangeState(0);

		for (int i = 0, l = m_firstObjectToEnableDisable.Length; i < l; ++i){
			m_firstObjectToEnableDisable[i].SetActive(false);
		}
		
		yield return new WaitForSeconds(m_secondTimeToEndFade);
		for (int i = 0, l = m_secondObjectToEnableDisable.Length; i < l; ++i){
			m_firstObjectToEnableDisable[i].SetActive(false);
		}
	}

	public void ChangeCameraPosition(){
		m_inMainPosition =! m_inMainPosition;
		if(m_inMainPosition){
			StartCoroutine(ChangeCameraPositionCorout(m_secondPosition.position, m_mainPosition.position, m_secondPosition.rotation, m_mainPosition.rotation, m_mainCanvas, m_secondCanvas));
		}else{
			StartCoroutine(ChangeCameraPositionCorout(m_mainPosition.position, m_secondPosition.position, m_mainPosition.rotation, m_secondPosition.rotation, m_secondCanvas, m_mainCanvas));
		}
	}

	IEnumerator ChangeCameraPositionCorout(Vector3 fromPosition, Vector3 toPosition, Quaternion fromRotation, Quaternion toRotation, GameObject[] objectsToEnable, GameObject[] objectsToDisable){

		// m_canChangePosition = false;
		
		for (int i = 0, l = objectsToDisable.Length; i < l; ++i){
			objectsToDisable[i].SetActive(false);
		}

		float m_moveJourneyLength = 0;
		float m_moveFracJourney = 0;
		float m_rotateFracJourney = 0;

		while(m_cameraToMove.position != toPosition){

			// MovePosition
			m_moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			m_moveFracJourney += (Time.deltaTime) * m_positionSpeed / m_moveJourneyLength;
			m_cameraToMove.position = Vector3.Lerp(fromPosition, toPosition, m_positionCurve.Evaluate(m_moveFracJourney));

			// MoveRotation
			m_rotateFracJourney += (Time.deltaTime) * m_rotationSpeed / m_moveJourneyLength;
			m_cameraToMove.rotation = Quaternion.Slerp(fromRotation, toRotation, m_rotationCurve.Evaluate(m_rotateFracJourney));

			yield return null;
		}
		
		for (int i = 0, l = objectsToEnable.Length; i < l; ++i){
			objectsToEnable[i].SetActive(true);
		}

	}

}
