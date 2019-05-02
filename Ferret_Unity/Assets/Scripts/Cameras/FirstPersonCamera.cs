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

	Vector2 mouseLook;
    Vector2 smoothV;
	float xAxisCLamp = 0;
    Transform playerTrans;

    void Start()
    {
        playerTrans = transform.parent.GetComponentInParent<PlayerManager>().transform;
    }

    public void RotateCamera()
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

		transform.Rotate(Vector3.left * mouseY);
		playerTrans.Rotate(Vector3.up * mouseX);
    }

	void ClampXAxisRotationToValue(float value){
		Vector3 eulerRotation = transform.eulerAngles;
		eulerRotation.x = value;
		transform.eulerAngles = eulerRotation;
	}
	
}
