using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTrigger : MonoBehaviour {

	[Header("Target")]
	[SerializeField] float m_timeToShowTarget = 2;
	[SerializeField] Transform m_targetPos;

	[Header("Move")]
	[SerializeField, Range(0, 1)] float m_distanceRange = 0.5f;
	[SerializeField] float m_speedToTarget = 5;
	[SerializeField] AnimationCurve m_curveToTarget;
	[SerializeField] float m_speedToPlayer = 5;
	[SerializeField] AnimationCurve m_curveToPlayer;

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_colorGizmos = Color.magenta;

	BoxCollider m_boxColl;
	public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
    }

	Cinemachine.CinemachineBrain m_cameraBrain;
	CameraLookAtPosition m_cameraLookAt;
	bool m_isActivated = false;
	Vector3 m_realLookAtPosition;

    void Start(){
		m_cameraBrain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
		BoxColl.isTrigger = true;
		m_cameraLookAt = CameraLookAtPosition.Instance;

		m_realLookAtPosition = Vector3.Lerp(transform.position, m_targetPos.position, m_distanceRange);
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player") && !m_isActivated){
			m_isActivated = true;
			StartCoroutine(MoveCoroutToTarget());
		}
	}

	IEnumerator MoveCoroutToTarget(){
		EnableCameraBrain(true);
		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_cameraLookAt.transform.position != m_realLookAtPosition){
			moveJourneyLength = Vector3.Distance(m_cameraLookAt.transform.position, m_realLookAtPosition);
			moveFracJourney += (Time.deltaTime) * m_speedToTarget / moveJourneyLength;
			m_cameraLookAt.transform.position = Vector3.Lerp(m_cameraLookAt.transform.position, m_realLookAtPosition, m_curveToTarget.Evaluate(moveFracJourney));
			yield return null;
		}
		StartCoroutine(ShowTarget());
	}
	IEnumerator ShowTarget(){
		yield return new WaitForSeconds(m_timeToShowTarget);
		StartCoroutine(MoveCoroutToPlayer());
	}
	IEnumerator MoveCoroutToPlayer(){
		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_cameraLookAt.transform.position != m_cameraLookAt.StartPosition){
			moveJourneyLength = Vector3.Distance(m_cameraLookAt.transform.position, m_cameraLookAt.StartPosition);
			moveFracJourney += (Time.deltaTime) * m_speedToPlayer / moveJourneyLength;
			m_cameraLookAt.transform.position = Vector3.Lerp(m_cameraLookAt.transform.position, m_cameraLookAt.StartPosition, m_curveToPlayer.Evaluate(moveFracJourney));
			yield return null;
		}
		EnableCameraBrain(false);
	}

	void EnableCameraBrain(bool b){
		m_cameraBrain.enabled = b;
	}

	void OnDrawGizmos(){
		Gizmos.color = m_colorGizmos;
		if(!m_showGizmos){
			return;
		}
		if(BoxColl != null){
			Gizmos.DrawWireCube(transform.position + BoxColl.center, BoxColl.size);
			if(m_targetPos != null){
				m_realLookAtPosition = Vector3.Lerp(transform.position, m_targetPos.position, m_distanceRange);
				Gizmos.DrawLine(transform.position + BoxColl.center, m_realLookAtPosition);
			}
		}
	}

}
