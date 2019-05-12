using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour {

	public static FirstPersonCamera Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
			//DontDestroyOnLoad(gameObject);
		}else{
			Debug.LogError("Two instance of FirstPersonCamera");
			//Destroy(gameObject);
		}
	}

	[Header("Sensitivity")]
    [SerializeField] float Xsensitivity = 1.0f;
    [SerializeField] float Ysensitivity = 1.0f;

	[Header("Clamp values")]
	[SerializeField] [Range(-90, 0)] float minClamp = -45;
	[SerializeField] [Range(0, 90)] float maxClamp = 45;

	[Header("Positions")]
	[SerializeField] Transform m_topPosition;
	[SerializeField] Transform m_botPosition;
	[SerializeField, Range(0,1)] float m_smoothSpeed = 0.5f;

	[Header("In 1st person when climb")]
	[SerializeField] Transform m_lookAtPoint;
	[SerializeField] Vector3 m_pointOffset = new Vector3(0, 0.5f, 0.5f);
	[SerializeField] float m_movePointDistance = 1f;
	[SerializeField, Range(0, 1)] float m_smoothLerp = 0.5f;

	Vector3 m_startPosition;
	Quaternion m_startRotation;
	Vector2 mouseLook;
    Vector2 smoothV;
	float xAxisCLamp = 0;
    Transform playerTrans;
	PlayerManager m_playerManager;
	Vector3 m_inputDirection = Vector3.zero;


    void Start()
    {
		m_startPosition = transform.localPosition;
		m_startRotation = transform.localRotation;
        playerTrans = transform.parent.GetComponentInParent<PlayerManager>().transform;
		m_playerManager = PlayerManager.Instance;
    }

    public void RotateCamera(bool whenClimb = false)
    {
		// float mouseX = Input.GetAxisRaw("Mouse X") * Xsensitivity * Time.deltaTime;
		// float mouseY = Input.GetAxisRaw("Mouse Y") * Ysensitivity * Time.deltaTime;

		float mouseX = Input.GetAxisRaw("CameraX") * Xsensitivity * Time.deltaTime;
		float mouseY = Input.GetAxisRaw("CameraY") * Ysensitivity * Time.deltaTime;

		xAxisCLamp += mouseY;
		
		if(xAxisCLamp > maxClamp){
			xAxisCLamp = maxClamp;
			mouseY = 0;
			ClampXAxisRotationToValue(- maxClamp);
		}else if(xAxisCLamp < minClamp){
			xAxisCLamp = minClamp;
			mouseY = 0;
			ClampXAxisRotationToValue(360 - minClamp);
		}
		
		if(!whenClimb){
			transform.Rotate(Vector3.left * mouseY);
			playerTrans.Rotate(Vector3.up * mouseX);
		}else{
			m_inputDirection.x = Input.GetAxis("CameraX") * m_movePointDistance;
			m_inputDirection.y = Input.GetAxis("CameraY") * m_movePointDistance;

			Vector3 desiredPos = transform.position + m_inputDirection + m_pointOffset;
			Vector3 smoothedPosition = Vector3.Lerp(m_lookAtPoint.position, desiredPos, m_smoothLerp);
			m_lookAtPoint.transform.position = smoothedPosition;
			transform.LookAt(m_lookAtPoint);
		}
    }

	public void ResetCameraOrientation(){
		transform.localPosition = m_startPosition;
		transform.localRotation = m_startRotation; 
	}

	void ClampXAxisRotationToValue(float value){
		Vector3 eulerRotation = transform.eulerAngles;
		eulerRotation.x = value;
		transform.eulerAngles = eulerRotation;
	}

	void LateUpdate(){
		Vector3 desiredPosition;
		if(m_playerManager.m_states.m_crawl.m_isCrawling){
			desiredPosition = new Vector3(transform.position.x, m_botPosition.position.y, transform.position.z);
		}else{
			desiredPosition = new Vector3(transform.position.x, m_topPosition.position.y, transform.position.z);
		}
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_smoothSpeed);
		transform.position = smoothedPosition;
	}
	
}
