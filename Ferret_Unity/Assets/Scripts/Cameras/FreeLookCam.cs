using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

	public Orbit[] m_orbits;
	[System.Serializable] public class Orbit {
		public string m_title = "Close orbit";
		public CinemachineFreeLook.Orbit m_topOrbit;
		public CinemachineFreeLook.Orbit m_middleOrbit;
		public CinemachineFreeLook.Orbit m_botOrbit;
	}

	CinemachineFreeLook m_freeLookCam;

	void Start(){
		m_freeLookCam = GetComponent<CinemachineFreeLook>();
		SwitchOrbitCamera(0);
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			SwitchOrbitCamera(1);
		}
	}

	public void SwitchOrbitCamera(int orbitNb){
		m_freeLookCam.m_Orbits[0].m_Height = m_orbits[orbitNb].m_topOrbit.m_Height;
		m_freeLookCam.m_Orbits[0].m_Radius = m_orbits[orbitNb].m_topOrbit.m_Radius;

		m_freeLookCam.m_Orbits[1].m_Height = m_orbits[orbitNb].m_middleOrbit.m_Height;
		m_freeLookCam.m_Orbits[1].m_Radius = m_orbits[orbitNb].m_middleOrbit.m_Radius;

		m_freeLookCam.m_Orbits[2].m_Height = m_orbits[orbitNb].m_botOrbit.m_Height;
		m_freeLookCam.m_Orbits[2].m_Radius = m_orbits[orbitNb].m_botOrbit.m_Radius;
	}

}
