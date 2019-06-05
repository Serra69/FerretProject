using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowLookAt : MonoBehaviour {

	[SerializeField] Transform m_objectToFollow;
	
	Vector3 m_offset;

    void Start(){
		m_offset = transform.localPosition;
	}

	void LateUpdate(){
		transform.position = m_objectToFollow.transform.position + m_offset;
	}

	public void UpdateCameraFollowLookAt(){
		transform.position = m_objectToFollow.transform.position + m_offset;
	}

}
