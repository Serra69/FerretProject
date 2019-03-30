using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour {

#region Singleton
	public static SwitchCamera Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}else{
			Debug.LogError("Two instance of SwitchCamera");
			//Destroy(gameObject);
		}
	}
#endregion Singleton

	[Header("Curve")]
	[SerializeField] AnimationCurve m_positionCurve;

	[Header("Positions")]
	[SerializeField] Transform m_firstPersonTrans;
	[SerializeField] Transform m_thirdPersonTrans;
	
	[Header("Speeds")]
	[SerializeField] float m_changePositionSpeed = 1;
	[SerializeField] float m_changeRotationSpeed = 1;

	[Header("Cameras")]
	[SerializeField] Camera m_firstPersonCam;
	[SerializeField] Camera m_thirdPersonCam;

	Camera m_camera;
	CinemachineBrain m_cameraBrain;
	PlayerManager m_playerManager;
	float m_moveJourneyLength;
	float m_moveFracJourney;
	bool m_thirdPersonMode = true;
	bool m_canChangePosition = true;

	bool m_cameraBrainType = true;
	Transform m_cameraBrainParent;

	void Start(){
		m_camera = GetComponent<Camera>();
		m_camera.enabled = false;
		m_cameraBrain = m_thirdPersonCam.GetComponent<CinemachineBrain>();
		m_playerManager = m_firstPersonCam.GetComponentInParent<PlayerManager>();
		m_cameraBrainParent = m_cameraBrain.GetComponentInParent<CameraSettings>().transform;
	}

	void Update(){
		//SwitchCameraWithButton();
		if(m_thirdPersonMode){
			m_playerManager.WhenCameraIsCloseToTheFerret(Vector3.Distance(m_playerManager.transform.position, m_cameraBrain.transform.position));
		}
	}

	public void SwitchCameraType(){
		if(m_canChangePosition){
			if(m_thirdPersonMode){
				StartCoroutine(MoveCamera(m_thirdPersonTrans.position, m_firstPersonTrans));
			}else{
				StartCoroutine(MoveCamera(m_firstPersonTrans.position, m_thirdPersonTrans));
			}
		}
	}

	void SwitchCameraWithButton(){
		if(m_canChangePosition && Input.GetButtonDown("Fire1")){
			if(m_thirdPersonMode){
				StartCoroutine(MoveCamera(m_thirdPersonTrans.position, m_firstPersonTrans));
			}else{
				StartCoroutine(MoveCamera(m_firstPersonTrans.position, m_thirdPersonTrans));
			}
		}
	}

	IEnumerator MoveCamera(Vector3 fromPosition, Transform toPosition){
		
		ChangeCameraBrainType();

		transform.rotation = m_thirdPersonTrans.rotation;

		m_camera.enabled = true;
		m_canChangePosition = false;

		while(transform.position != toPosition.position){

			// MovePosition
			m_moveJourneyLength = Vector3.Distance(fromPosition, toPosition.position);
			m_moveFracJourney += (Time.deltaTime) * m_changePositionSpeed / m_moveJourneyLength;
			transform.position = Vector3.Lerp(fromPosition, toPosition.position, m_positionCurve.Evaluate(m_moveFracJourney));

			yield return null;
		}
		m_canChangePosition = true;
		
		ChangePersonMod();

		m_moveFracJourney = 0;
		m_firstPersonTrans.rotation = transform.rotation;

		m_camera.enabled = false;

		ChangeCameraBrainType();
	}

	void ChangePersonMod(){
		m_thirdPersonMode =! m_thirdPersonMode;

		if(m_thirdPersonMode){
			m_firstPersonCam.depth = 0;
			m_thirdPersonCam.depth = 1;
		}else{
			m_firstPersonCam.depth = 1;
			m_thirdPersonCam.depth = 0;
		}
	}

	void ChangeCameraBrainType(){
		m_cameraBrainType =! m_cameraBrainType;

		if(m_cameraBrainType){
			m_cameraBrain.enabled = true;
			m_cameraBrain.transform.SetParent(m_cameraBrainParent);
		}else{
			m_cameraBrain.enabled = false;
			m_cameraBrain.transform.SetParent(m_playerManager.transform);
		}
	}

}

/* 
---------------------------------------
-------------- 30/03/19 ---------------
---------------------------------------
-----Fonctionne si on bouge pas ! -----
---------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour {

	[Header("Curve")]
	[SerializeField] AnimationCurve m_positionCurve;
	[SerializeField] AnimationCurve m_rotateCurve;

	[Header("Positions")]
	[SerializeField] Transform m_firstPersonTrans;
	[SerializeField] Transform m_thirdPersonTrans;
	
	[Header("Speeds")]
	[SerializeField] float m_changePositionSpeed = 1;
	[SerializeField] float m_changeRotationSpeed = 1;

	[Header("Cameras")]
	[SerializeField] Camera m_firstPersonCam;
	[SerializeField] Camera m_thirdPersonCam;

	Camera m_camera;

	float m_moveJourneyLength;
	float m_moveFracJourney;
	bool m_thirdPersonMode = true;
	bool m_canChangePosition = true;

	void Awake(){
		m_camera = GetComponent<Camera>();
		m_camera.enabled = false;
	}

	void Update(){
		if(m_canChangePosition && Input.GetButtonDown("Fire1")){
			if(m_thirdPersonMode){
				StartCoroutine(MoveCamera(m_thirdPersonTrans.position, m_firstPersonTrans.position));
			}else{
				StartCoroutine(MoveCamera(m_firstPersonTrans.position, m_thirdPersonTrans.position));
			}
		}
	}

	IEnumerator MoveCamera(Vector3 fromPosition, Vector3 toPosition){

		transform.rotation = m_thirdPersonTrans.rotation;

		m_camera.enabled = true;
		m_canChangePosition = false;

		while(transform.position != toPosition){

			// MovePosition
			m_moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			m_moveFracJourney += (Time.deltaTime) * m_changePositionSpeed / m_moveJourneyLength;
			transform.position = Vector3.Lerp(fromPosition, toPosition, m_positionCurve.Evaluate(m_moveFracJourney));

			yield return null;
		}
		m_canChangePosition = true;
		m_thirdPersonMode =! m_thirdPersonMode;

		if(m_thirdPersonMode){
			m_firstPersonCam.depth = 0;
			m_thirdPersonCam.depth = 1;
		}else{
			m_firstPersonCam.depth = 1;
			m_thirdPersonCam.depth = 0;
		}

		m_moveFracJourney = 0;

		m_firstPersonTrans.rotation = transform.rotation;

		m_camera.enabled = false;
	}

}*/