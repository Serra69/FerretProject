using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour {

	[SerializeField] Transform m_thirdCameraTrans;

	void Update(){
		transform.rotation = m_thirdCameraTrans.rotation;
	}
	
}
