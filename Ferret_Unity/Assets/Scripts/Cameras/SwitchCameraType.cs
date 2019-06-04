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

	FreeLookCamManager m_camManager;
	Camera m_camera;
	FixeCamera m_lastFixeCamera;
	CameraPivot m_pivotManager;

	void Start(){
		m_camManager = FreeLookCamManager.Instance;
		m_camera = GetComponent<Camera>();
		m_camera.enabled = false;
		m_pivotManager = CameraPivot.Instance;
	}

	public void SetLastFixeCamera(FixeCamera fixeCamera){
		m_lastFixeCamera = fixeCamera;
	}

	public void SwitchCamera(Transform newTrans, bool toCameraBrain, FixeCamera fixeCam){
		if(!toCameraBrain){
			transform.position = m_camManager.m_cameraBrain.position;
			transform.rotation = m_camManager.m_cameraBrain.rotation;
		}else{
			transform.position = fixeCam.transform.position;
			transform.rotation = fixeCam.transform.rotation;
		}
		m_camera.enabled = true;
		StopAllCoroutines();
		StartCoroutine(SwitchingCamera(transform.position, newTrans, transform.rotation, newTrans));
	}

	IEnumerator SwitchingCamera(Vector3 fromPosition, Transform toPosition, Quaternion fromRotation, Transform toRotation){

		m_pivotManager.ChangeCamera(transform);

		m_camera.enabled = true;

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
		m_lastFixeCamera.On_SwitchCameraIsFinished();
	}

}
