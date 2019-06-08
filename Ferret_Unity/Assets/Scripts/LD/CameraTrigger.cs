using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTrigger : FreeLookCameraType {

	[SerializeField] bool m_activeJusteOneTime = true;

	[Header("Change orbit")]
	[SerializeField] bool m_canChangeOrbit = true;
	public ChangeOrbit m_changeOrbit = new ChangeOrbit();
	[System.Serializable] public class ChangeOrbit {
		public FreeLookCameraOrbit m_newLookType = FreeLookCameraOrbit.Far;
		[Space]
		public bool m_useTriggerExitToResetOrbit = false;
		public FreeLookCameraOrbit m_resetOrbitType = FreeLookCameraOrbit.Far;
	}

	[Header("Look position")]
	[SerializeField] bool m_canLookPosiiton = true;
	public LookPosition m_lookPosition = new LookPosition();
	[System.Serializable] public class LookPosition {
		[Header("Parameters")]
		public bool m_allowStopShowTarget = true;

		[Header("Target")]
		public float m_timeToShowTarget = 2;
		public Transform m_targetPos;

		[Header("Move")]
		[Range(0, 1)] public float m_distanceRange = 0.5f;
		public float m_speedToTarget = 5;
		public AnimationCurve m_curveToTarget;
		public float m_speedToPlayer = 5;
		public AnimationCurve m_curveToPlayer;

	}	

	[Header("Gizmos")]
	public Gizmo m_gizmos = new Gizmo();
	[System.Serializable] public class Gizmo {
		public bool m_showGizmos = true;
		public Color m_colorGizmos = Color.yellow;
	}

	BoxCollider m_boxColl;
	public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
    }

	PlayerManager m_playerManager;
	FreeLookCamManager m_freeLookCamManager;
	CameraLookAtPosition m_cameraLookAt;
	bool m_isActivated = false;
	Vector3 m_realLookAtPosition;
	bool m_coroutineIsRunning = false;
	FollowPlayer m_followPlayer;

    void Start(){
		m_freeLookCamManager = FreeLookCamManager.Instance;

		m_playerManager = PlayerManager.Instance;
		BoxColl.isTrigger = true;
		m_cameraLookAt = CameraLookAtPosition.Instance;

		if(m_lookPosition.m_targetPos != null)
			m_realLookAtPosition = Vector3.Lerp(transform.position, m_lookPosition.m_targetPos.position, m_lookPosition.m_distanceRange);

		m_followPlayer = FollowPlayer.Instance;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player") && !m_isActivated){

			if(m_activeJusteOneTime){
				m_isActivated = true;
			}

			if(m_canLookPosiiton){
				StartCoroutine(MoveCoroutToTarget());
			}

			if(m_canChangeOrbit){
				m_freeLookCamManager.SwitchOrbitCamera(m_changeOrbit.m_newLookType);
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			if(m_canChangeOrbit && m_changeOrbit.m_useTriggerExitToResetOrbit){
				m_freeLookCamManager.SwitchOrbitCamera(m_changeOrbit.m_resetOrbitType);
			}
		}
	}

	public void On_SkeletonMoved(){
		StartCoroutine(MoveCoroutToTarget());
	}

	IEnumerator MoveCoroutToTarget(){
		m_freeLookCamManager.ResetXInput(false);
		m_freeLookCamManager.ResetYInput(false);
		m_coroutineIsRunning = true;
		m_followPlayer.FollowLookAtPoint = false;

		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_cameraLookAt.transform.position != m_realLookAtPosition){
			moveJourneyLength = Vector3.Distance(m_cameraLookAt.transform.position, m_realLookAtPosition);
			moveFracJourney += (Time.deltaTime) * m_lookPosition.m_speedToTarget / moveJourneyLength;
			m_cameraLookAt.transform.position = Vector3.Lerp(m_cameraLookAt.transform.position, m_realLookAtPosition, m_lookPosition.m_curveToTarget.Evaluate(moveFracJourney));
			yield return null;
		}
		m_coroutineIsRunning = false;
		StartCoroutine(ShowTarget());
	}
	IEnumerator ShowTarget(){
		yield return new WaitForSeconds(m_lookPosition.m_timeToShowTarget);
		StartCoroutine(MoveCoroutToPlayer());
	}
	IEnumerator MoveCoroutToPlayer(){
		m_coroutineIsRunning = true;
		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_cameraLookAt.transform.localPosition != m_cameraLookAt.StartPosition){
			moveJourneyLength = Vector3.Distance(m_cameraLookAt.transform.localPosition, m_cameraLookAt.StartPosition);
			moveFracJourney += (Time.deltaTime) * m_lookPosition.m_speedToPlayer / moveJourneyLength;
			m_cameraLookAt.transform.localPosition = Vector3.Lerp(m_cameraLookAt.transform.localPosition, m_cameraLookAt.StartPosition, m_lookPosition.m_curveToPlayer.Evaluate(moveFracJourney));
			yield return null;
		}

		m_followPlayer.ReturnToPlayer();

		m_coroutineIsRunning = false;
		m_freeLookCamManager.ResetXInput(true);
		m_freeLookCamManager.ResetYInput(true);
	}

	void OnDrawGizmos(){
		Gizmos.color = m_gizmos.m_colorGizmos;
		if(!m_gizmos.m_showGizmos){
			return;
		}
		if(BoxColl != null){
			Gizmos.DrawWireCube(transform.position + BoxColl.center, BoxColl.size);
			if(m_lookPosition.m_targetPos != null && m_canLookPosiiton){
				Gizmos.DrawWireSphere(m_lookPosition.m_targetPos.position, 0.2f);
				m_realLookAtPosition = Vector3.Lerp(transform.position + BoxColl.center, m_lookPosition.m_targetPos.position, m_lookPosition.m_distanceRange);
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
		return Vector3.Lerp(m_realLookAtPosition, m_lookPosition.m_targetPos.position, f);
	}

}
