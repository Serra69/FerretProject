using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtPosition : MonoBehaviour {

#region Singleton
	public static CameraLookAtPosition Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of CameraLookAtPosition");
		}
	}
#endregion //Singleton

	Vector3 m_startPosition;
	public Vector3 StartPosition
    {
        get
        {
            return m_startPosition;
        }
    }

    void Start(){
		m_startPosition = transform.localPosition;
	}

}
