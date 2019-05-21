using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCamera : MonoBehaviour {
		
#region Singleton
	public static ShowCamera Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of ShowCamera");
		}
	}
#endregion //Singleton

	[SerializeField] Transform m_objectToFollow;
	[SerializeField] Transform m_objectToLookAt;

	void LateUpdate(){
		// transform.position = m_objectToFollow.transform.position;
		transform.LookAt(m_objectToLookAt);
	}

	public void SetCameraPos(){
		transform.position = m_objectToFollow.transform.position;
	}

}
