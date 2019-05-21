﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTrigger : MonoBehaviour {

	[Header("Debugs")]
	public Debugs m_debugs = new Debugs();
	[System.Serializable] public class Debugs {
		public bool m_cuteButCanModify = true;
	}

	[Header("Parameters")]
	[SerializeField] bool m_playerCanMoveWhenCameraMove = true;
	[SerializeField] bool m_allowStopShowTarget = true;

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
	[SerializeField] Color m_colorGizmos = Color.yellow;

	BoxCollider m_boxColl;
	public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
    }

	PlayerManager m_playerManager;
	CinemachineBrain m_cameraBrain;
	CameraLookAtPosition m_cameraLookAt;
	bool m_isFirstActivated = false;
	bool m_isSecondActivated = false;
	Vector3 m_realLookAtPosition;
	ShowCamera m_showCam;
	Camera m_showCamera;

	bool m_coroutineIsRunning = false;

    void Start(){
		m_playerManager = PlayerManager.Instance;
		m_cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
		BoxColl.isTrigger = true;
		m_cameraLookAt = CameraLookAtPosition.Instance;

		m_realLookAtPosition = Vector3.Lerp(transform.position, m_targetPos.position, m_distanceRange);

		m_showCam = ShowCamera.Instance;
		m_showCamera = m_showCam.GetComponent<Camera>();
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player") && !m_isFirstActivated){
			m_isFirstActivated = true;

			if(!m_playerCanMoveWhenCameraMove){
				m_playerManager.On_CinematicIsLaunch(true);
			}

			StartCoroutine(MoveCoroutToTarget());
		}
	}
	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player") && !m_isSecondActivated && m_allowStopShowTarget){
			m_isSecondActivated = true;

			if(!m_playerCanMoveWhenCameraMove){
				m_playerManager.On_CinematicIsLaunch(true);
			}

			StartCoroutine(MoveCoroutToPlayer());
		}
	}

	IEnumerator MoveCoroutToTarget(){
		m_coroutineIsRunning = true;
		if(!m_debugs.m_cuteButCanModify){
			m_showCam.SetCameraPos();
			EnableCamera(true);
			yield return new WaitForSeconds(0.05f);
			EnableCameraBrain(false);
		}

		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_cameraLookAt.transform.position != m_realLookAtPosition){
			moveJourneyLength = Vector3.Distance(m_cameraLookAt.transform.position, m_realLookAtPosition);
			moveFracJourney += (Time.deltaTime) * m_speedToTarget / moveJourneyLength;
			m_cameraLookAt.transform.position = Vector3.Lerp(m_cameraLookAt.transform.position, m_realLookAtPosition, m_curveToTarget.Evaluate(moveFracJourney));
			yield return null;
		}
		m_coroutineIsRunning = false;
		StartCoroutine(ShowTarget());
	}
	IEnumerator ShowTarget(){
		yield return new WaitForSeconds(m_timeToShowTarget);
		StartCoroutine(MoveCoroutToPlayer());
	}
	IEnumerator MoveCoroutToPlayer(){
		m_coroutineIsRunning = true;
		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_cameraLookAt.transform.localPosition != m_cameraLookAt.StartPosition){
			moveJourneyLength = Vector3.Distance(m_cameraLookAt.transform.localPosition, m_cameraLookAt.StartPosition);
			moveFracJourney += (Time.deltaTime) * m_speedToPlayer / moveJourneyLength;
			m_cameraLookAt.transform.localPosition = Vector3.Lerp(m_cameraLookAt.transform.localPosition, m_cameraLookAt.StartPosition, m_curveToPlayer.Evaluate(moveFracJourney));
			yield return null;
		}

		if(!m_debugs.m_cuteButCanModify){
			EnableCameraBrain(true);
			yield return new WaitForSeconds(0.05f);
			EnableCamera(false);
			On_ShowPointIsFinished();
		}
		m_coroutineIsRunning = false;
	}

	void On_ShowPointIsFinished(){
		if(!m_playerCanMoveWhenCameraMove){
			m_playerManager.On_CinematicIsLaunch(false);
		}
	}

	void EnableCameraBrain(bool b){
		m_cameraBrain.enabled = b;
	}

	void EnableCamera(bool b){
		m_showCamera.enabled = b;
	}

	void OnDrawGizmos(){
		Gizmos.color = m_colorGizmos;
		if(!m_showGizmos){
			return;
		}
		if(BoxColl != null){
			Gizmos.DrawWireCube(transform.position + BoxColl.center, BoxColl.size);
			if(m_targetPos != null){
				Gizmos.DrawWireSphere(m_targetPos.position, 0.2f);
				m_realLookAtPosition = Vector3.Lerp(transform.position + BoxColl.center, m_targetPos.position, m_distanceRange);
				Gizmos.DrawLine(transform.position + BoxColl.center, m_realLookAtPosition);

				Gizmos.DrawSphere(m_realLookAtPosition, 0.2f);

				Gizmos.DrawLine(ReturnDot(0.05f), ReturnDot(0.15f));
				Gizmos.DrawLine(ReturnDot(0.25f), ReturnDot(0.35f));
				Gizmos.DrawLine(ReturnDot(0.45f), ReturnDot(0.55f));
				Gizmos.DrawLine(ReturnDot(0.65f), ReturnDot(0.75f));
				Gizmos.DrawLine(ReturnDot(0.85f), ReturnDot(0.95f));
			}
		}
	}

	Vector3 ReturnDot(float f){
		return Vector3.Lerp(m_realLookAtPosition, m_targetPos.position, f);
	}

}
