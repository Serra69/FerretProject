using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameraType : MonoBehaviour {

#region Singleton
	public static SwitchCameraType Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of SwitchCameraType");
		}
	}
#endregion Singleton

	[Header("Speeds")]
	[SerializeField] float m_changePositionSpeed = 1;
	[SerializeField] float m_changeRotationSpeed = 1;

	[Header("Curve")]
	[SerializeField] AnimationCurve m_positionCurve;
	[SerializeField] AnimationCurve m_rotationCurve;

	PlayerManager m_playerManager;
	FreeLookCamManager m_freeLookCamManager;
	Camera m_camera;
	AudioListener m_audioListener;
	FixeCamera m_lastFixeCamera;
	CameraPivot m_pivotManager;

	void Start(){
		m_playerManager = PlayerManager.Instance;
		m_freeLookCamManager = FreeLookCamManager.Instance;
		m_camera = GetComponent<Camera>();
		m_audioListener = GetComponent<AudioListener>();
		m_pivotManager = CameraPivot.Instance;
	}

	public void SetLastFixeCamera(FixeCamera fixeCamera){
		m_lastFixeCamera = fixeCamera;
	}

	public void SwitchCamera(Transform newTrans, bool toCameraBrain, FixeCamera fixeCam){
		if(!toCameraBrain){
			transform.position = m_freeLookCamManager.m_cameraBrain.position;
			transform.rotation = m_freeLookCamManager.m_cameraBrain.rotation;
		}else{
			transform.position = fixeCam.transform.position;
			transform.rotation = fixeCam.transform.rotation;
		}
		m_camera.enabled = true;

		if(!m_playerManager.m_playerDebugs.m_useAudioListenerOnFerret){
			m_audioListener.enabled = true;
		}

		m_freeLookCamManager.On_AnotherCamIsActivated(true);
		StopAllCoroutines();
		StartCoroutine(SwitchingCamera(transform.position, newTrans, transform.rotation, newTrans));
	}

	IEnumerator SwitchingCamera(Vector3 fromPosition, Transform toPosition, Quaternion fromRotation, Transform toRotation){

		m_pivotManager.ChangeCamera(transform);

		m_camera.enabled = true;

		if(!m_playerManager.m_playerDebugs.m_useAudioListenerOnFerret){
			m_audioListener.enabled = true;
		}

		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(transform.position != toPosition.position){
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, toPosition.position);
			moveFracJourney += (Time.deltaTime) * m_changePositionSpeed / moveJourneyLength;
			transform.position = Vector3.Lerp(fromPosition, toPosition.position, m_positionCurve.Evaluate(moveFracJourney));

			// MoveRotation
			rotateJourneyLength = Quaternion.Dot(fromRotation, toRotation.rotation);
			rotateFracJourney += (Time.deltaTime) * m_changeRotationSpeed / moveJourneyLength;
			transform.rotation = Quaternion.Lerp(fromRotation, toRotation.rotation, m_rotationCurve.Evaluate(rotateFracJourney));

			yield return null;
		}

		m_camera.enabled = false;

		if(!m_playerManager.m_playerDebugs.m_useAudioListenerOnFerret){
			m_audioListener.enabled = false;
		}

		m_lastFixeCamera.On_SwitchCameraIsFinished();
		m_freeLookCamManager.On_AnotherCamIsActivated(false);
	}

}
