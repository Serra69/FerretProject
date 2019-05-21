using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCam : MonoBehaviour {

#region Singleton
	public static FreeLookCam Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of FreeLookCam");
		}
	}
#endregion //Singleton

}
