using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour {

#region Singleton
	public static CameraPivot Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of CameraPivot");
		}
	}
#endregion //Singleton

	public Transform m_mainCamera;

	Transform m_actualCamera;

	void Start(){
		ChangeCamera(m_mainCamera);
	}

	void FixedUpdate(){
		transform.rotation = m_actualCamera.rotation;
	}

	public void UpdateCameraPivot(){
		transform.rotation = m_actualCamera.rotation;
	}

	public void ChangeCamera(Transform newCameraTrans){
		m_actualCamera = newCameraTrans;
	}

	public void ResetWithMainCamera(){
		m_actualCamera = m_mainCamera;
	}

}
