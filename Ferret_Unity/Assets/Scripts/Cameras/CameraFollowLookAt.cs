using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowLookAt : MonoBehaviour {

#region Singleton
	public static CameraFollowLookAt Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of CameraFollowLookAt");
		}
	}
#endregion //Singleton

	public Transform m_mainObjectToFollow;

	Transform m_mainObject;
	public Transform MainObject{
			get{
					return m_mainObject;
			}
			set{
					m_mainObject = value;
			}
	}
	
	Vector3 m_offset;


	void Start(){
		m_offset = transform.localPosition;
		MainObject = m_mainObjectToFollow;
	}

	// void LateUpdate(){
	// 	transform.position = m_mainObjectToFollow.transform.position + m_offset;
	// }

	public void UpdateCameraFollowLookAt(){
		transform.position = m_mainObjectToFollow.transform.position + m_offset;
	}

}
