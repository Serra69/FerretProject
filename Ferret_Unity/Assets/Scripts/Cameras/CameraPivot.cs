using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour {

	[SerializeField] Transform m_camera;

	void FixedUpdate(){
		transform.rotation = m_camera.rotation;
	}
	
}
