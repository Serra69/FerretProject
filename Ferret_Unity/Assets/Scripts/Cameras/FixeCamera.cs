using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixeCamera : MonoBehaviour {

	/*[Header("Object to follow")]
	public Transform m_objectToFollow;*/

	[Header("Parameters")]
	public Parameters m_parameters = new Parameters();
	[System.Serializable] public class Parameters {
		[Header("Speeds")]
		[Range(0, 1)] public float m_xSpeed = 0.5f;
		[Range(0, 1)] public float m_ySpeed = 0.5f;

		public float m_yUpPosition = 1;

		[Header("Z speed")]
		[Range(-10, 10)] public float m_minZPos = 0;
		[Range(-10, 10)] public float m_maxZPos = 5;
		[Range(0, 1)] public float m_zSpeed = 0.5f;
	}

	[Header("Positions")]
	public Positions m_positions = new Positions();
	[System.Serializable] public class Positions {
		public Vector2 m_maxTop;
		public Vector2 m_maxRight;
		public Vector2 m_maxBot;
		public Vector2 m_maxLeft;
	}	

	[Header("Triggers")]
	public Triggers m_triggers = new Triggers();
	[System.Serializable] public class Triggers {
		public SwitchDimensionCameraTrigger[] m_enterTriggers;
		public SwitchDimensionCameraTrigger[] m_exitTriggers;
		public SwitchDimensionCameraTrigger[] m_changeYTriggers;
	}

	[Header("Gizmos")]
	public Gizmo m_gizmos = new Gizmo();	
	[System.Serializable] public class Gizmo {
		public bool m_show = true;
		public Color m_enterColor = Color.yellow;
		public Color m_exitColor = Color.magenta;
		public Color m_changeYColor = Color.grey;
	}

	Vector3 m_offset;
	bool m_playerIsInTriggers = false;
	FreeLookCamManager m_cameraManager;
	CameraPivot m_pivotManager;
	SwitchCameraType m_switchCamera;
	PlayerManager m_playerManager;
	Camera m_camera;
	AudioListener m_audioListener;
	FreeLookCamManager m_freeLookCamManager;

	float m_actualZPos = 5;
	bool m_isMaxZoomed = false;

    void Start(){
		m_offset = transform.position;

		for (int i = 0, l = m_triggers.m_enterTriggers.Length; i < l; ++i){
			m_triggers.m_enterTriggers[i].FixeCamera = this;
		}
		for (int i = 0, l = m_triggers.m_exitTriggers.Length; i < l; ++i){
			m_triggers.m_exitTriggers[i].FixeCamera = this;
		}
		for (int i = 0, l = m_triggers.m_changeYTriggers.Length; i < l; ++i){
			m_triggers.m_changeYTriggers[i].FixeCamera = this;
		}

		m_cameraManager = FreeLookCamManager.Instance;
		m_pivotManager = CameraPivot.Instance;
		m_switchCamera = SwitchCameraType.Instance;
		m_playerManager = PlayerManager.Instance;
		m_camera = GetComponent<Camera>();
		m_audioListener = GetComponent<AudioListener>();
		m_freeLookCamManager = FreeLookCamManager.Instance;

		m_actualZPos = m_parameters.m_maxZPos;
	}
	
	void LateUpdate(){
		// if(m_cameraCanMove){
			float playerXRange = Mathf.InverseLerp(m_offset.x - m_positions.m_maxLeft.x, m_offset.x - m_positions.m_maxRight.x, m_playerManager.transform.position.x);
			float playerYRange = Mathf.InverseLerp(m_offset.y + m_positions.m_maxBot.y, m_offset.y + m_positions.m_maxTop.y, m_playerManager.transform.position.y + m_parameters.m_yUpPosition);

			float desiredXPosition = Mathf.Lerp(m_offset.x - m_positions.m_maxLeft.x, m_offset.x - m_positions.m_maxRight.x, playerXRange);
			float desiredYPosition = Mathf.Lerp(m_offset.y + m_positions.m_maxBot.y, m_offset.y + m_positions.m_maxTop.y, playerYRange);

			float xCurve = Mathf.Lerp(transform.position.x, desiredXPosition, m_parameters.m_xSpeed);
			float yCurve = Mathf.Lerp(transform.position.y, desiredYPosition, m_parameters.m_ySpeed);
			float zCurve = Mathf.Lerp(transform.position.z, m_offset.z - m_actualZPos, m_parameters.m_zSpeed);

			transform.position =  new Vector3(xCurve, yCurve, zCurve);
		// }
	}

	public void OnPlayerEnterInEnterTrigger(){
		if(!m_playerIsInTriggers){
			m_playerIsInTriggers = true;
			// Debug.Log("Player enter in triggers");
			On_PlayerEnterInTriggers(true);
		}
	}

	public void CheckIfPlayerIsInTriggers(){

		int trueTriggersNb = 0;
		for (int i = 0, l = m_triggers.m_exitTriggers.Length; i < l; ++i){
			if(m_triggers.m_exitTriggers[i].PlayerIsInTrigger){
				trueTriggersNb ++;
			}
		}
		if(trueTriggersNb == 0){
			if(m_playerIsInTriggers){
				// Debug.Log("Player exit from triggers");
				m_playerIsInTriggers = false;
				On_PlayerEnterInTriggers(false);
			}
		}
	}

	void On_PlayerEnterInTriggers(bool playerEnter){
		if(playerEnter){
			m_playerManager.CanHideFerret = false;

			m_cameraManager.ResetXInput(false);
			m_cameraManager.ResetYInput(false);

			m_switchCamera.SetLastFixeCamera(this);
			m_switchCamera.SwitchCamera(transform, false, this);
		}else{
			m_camera.enabled = false;

			if(!m_playerManager.m_playerDebugs.m_useAudioListenerOnFerret){
				m_audioListener.enabled = false;
			}

			m_switchCamera.SetLastFixeCamera(this);
			m_switchCamera.SwitchCamera(m_cameraManager.m_cameraBrain, true, this);
		}
	}
	
	public void On_SwitchCameraIsFinished(){
		if(m_playerIsInTriggers){
			m_pivotManager.ChangeCamera(transform);
			m_camera.enabled = true;

			if(!m_playerManager.m_playerDebugs.m_useAudioListenerOnFerret){
				m_audioListener.enabled = true;
			}
			
			m_freeLookCamManager.On_AnotherCamIsActivated(true);
		}else{
			m_cameraManager.ResetXInput(true);
			m_cameraManager.ResetYInput(true);
			m_pivotManager.ResetWithMainCamera();
			m_playerManager.CanHideFerret = true;
		}
	}

	public void On_ChangeZPosition(){
		m_isMaxZoomed =! m_isMaxZoomed;
		if(m_isMaxZoomed){
			m_actualZPos = m_parameters.m_maxZPos;
		}else{
			m_actualZPos = m_parameters.m_minZPos;
		}
	}

	void OnDrawGizmos(){
		if(!m_gizmos.m_show){
			return;
		}
		Gizmos.color = m_gizmos.m_enterColor;
		for (int i = 0, l = m_triggers.m_enterTriggers.Length; i < l; ++i){
			BoxCollider col;
			col = m_triggers.m_enterTriggers[i].GetComponent<BoxCollider>();
			Gizmos.DrawWireCube(col.transform.position + col.center, col.size);
		}

		Gizmos.color = m_gizmos.m_exitColor;
		for (int i = 0, l = m_triggers.m_exitTriggers.Length; i < l; ++i){
			BoxCollider col;
			col = m_triggers.m_exitTriggers[i].GetComponent<BoxCollider>();
			Gizmos.DrawWireCube(col.transform.position + col.center, col.size);
		}

		Gizmos.color = m_gizmos.m_changeYColor;
		for (int i = 0, l = m_triggers.m_changeYTriggers.Length; i < l; ++i){
			BoxCollider col;
			col = m_triggers.m_changeYTriggers[i].GetComponent<BoxCollider>();
			Gizmos.DrawWireCube(col.transform.position + col.center, col.size);
		}
	}

}
