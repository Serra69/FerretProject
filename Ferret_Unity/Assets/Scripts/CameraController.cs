using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

	[SerializeField] Transform m_target;
	[SerializeField] float m_minViewAngle = 45;
	[SerializeField] float m_maxViewAngle = 45;
	[SerializeField] float m_XRotateSpeed = 5;
	[SerializeField] float m_YRotateSpeed = 5;
	[SerializeField] Transform m_pivot;

	//public bool m_invertY;

	private Vector3 m_offset;

	void Start(){
		m_target = GameObject.FindGameObjectWithTag("Player").transform;
		if(m_target == null){
			Debug.LogError("I can't find a Player in the actual scene!");
		}
		m_offset = m_target.position - transform.position;

		m_pivot.position = m_target.position;
		//m_pivot.parent = m_target;
		m_pivot.parent = null;

		//Cursor.lockState = CursorLockMode.Locked; // The cursor become invisible
	}

	void LateUpdate(){

		m_pivot.position = m_target.position;

		// Get the X position of the mouse & rotate the target
		float horizonal = Input.GetAxis("AltHorizontal") * m_XRotateSpeed;
		m_pivot.Rotate(0, horizonal, 0);

		// Get the Y position of th emouse & rotate the pivot
		float vertical = Input.GetAxis("AltVertical") * m_YRotateSpeed;
		m_pivot.Rotate(-vertical, 0, 0);

		//Debug.Log("Horizontal = " + horizonal + " & Vertical = " + vertical);

		/*if(m_invertY){
			m_pivot.Rotate(vertical, 0, 0);
		}else{
			m_pivot.Rotate(-vertical, 0, 0);
		}*/

		// Limit up/down camera rotation
		/*if(m_pivot.rotation.eulerAngles.x > m_maxViewAngle && m_pivot.rotation.eulerAngles.x < 180f){
			Debug.Log("I replace the camera");
			m_pivot.rotation = Quaternion.Euler(m_maxViewAngle, m_pivot.rotation.eulerAngles.y, m_pivot.rotation.eulerAngles.z);
		}
		if(m_pivot.rotation.eulerAngles.x > 180 && m_pivot.rotation.eulerAngles.x < 360f + m_minViewAngle){
			Debug.Log("I replace the camera");
			m_pivot.rotation = Quaternion.Euler(360f + m_minViewAngle, m_pivot.rotation.eulerAngles.y, m_pivot.rotation.eulerAngles.z);
		}*/

		// Move the camera based on the current rotation of the target & the original offset
		float desiredYAngle = m_pivot.eulerAngles.y;
		float desiredXAngle = m_pivot.eulerAngles.x;
		Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
		transform.position = m_target.position - (rotation * m_offset);

		//transform.position = m_target.position - m_offset;

		if(transform.position.y < m_target.position.y){
			transform.position = new Vector3(transform.position.x, m_target.position.y + 0.5f, transform.position.z);
		}

		transform.LookAt(m_target);
	}

}
