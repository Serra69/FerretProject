using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookCamManager : FreeLookCameraType {

#region Singleton
	public static FreeLookCamManager Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of FreeLookCamManager");
		}
	}
#endregion //Singleton

	[SerializeField] FreeLookCameraOrbit m_startLookType = FreeLookCameraOrbit.Close;

	public SwitchPreferences m_switchPreferences = new SwitchPreferences();
	[System.Serializable] public class SwitchPreferences {
		public float m_speed = 5;
		public AnimationCurve m_curve;
	}

	public Orbit[] m_orbits;
	[System.Serializable] public class Orbit {
		public string m_title = "Close orbit";
		public CinemachineFreeLook.Orbit m_topOrbit;
		public CinemachineFreeLook.Orbit m_middleOrbit;
		public CinemachineFreeLook.Orbit m_botOrbit;
	}

	CinemachineFreeLook m_freeLookCam;
	AxisState m_saveXAxis;
	AxisState m_dontMoveXAxis;
	AxisState m_saveYAxis;
	AxisState m_dontMoveYAxis;

	void Start(){
		m_freeLookCam = GetComponent<CinemachineFreeLook>();

		m_dontMoveXAxis = m_freeLookCam.m_XAxis;
		m_dontMoveYAxis = m_freeLookCam.m_YAxis;
		m_dontMoveXAxis.m_InputAxisName = "";
		m_dontMoveYAxis.m_InputAxisName = "";

		SetOrbitCamera(m_startLookType);
	}

	void SetOrbitCamera(FreeLookCameraOrbit freeLookCameraOrbit){
		int orbitNb = new int();
		switch(freeLookCameraOrbit){
			case FreeLookCameraOrbit.Close:
				orbitNb = 0;
			break;
			case FreeLookCameraOrbit.Far:
				orbitNb = 1;
			break;
		}
		m_freeLookCam.m_Orbits[0].m_Radius = m_orbits[orbitNb].m_topOrbit.m_Height;
		m_freeLookCam.m_Orbits[0].m_Radius = m_orbits[orbitNb].m_topOrbit.m_Radius;

		m_freeLookCam.m_Orbits[1].m_Radius = m_orbits[orbitNb].m_middleOrbit.m_Height;
		m_freeLookCam.m_Orbits[1].m_Radius = m_orbits[orbitNb].m_middleOrbit.m_Radius;

		m_freeLookCam.m_Orbits[2].m_Radius = m_orbits[orbitNb].m_botOrbit.m_Height;
		m_freeLookCam.m_Orbits[2].m_Radius = m_orbits[orbitNb].m_botOrbit.m_Radius;
	}

	public void SwitchOrbitCamera(FreeLookCameraOrbit freeLookCameraOrbit){

		int orbitNb = new int();

		switch(freeLookCameraOrbit){
			case FreeLookCameraOrbit.Close:
				orbitNb = 0;
			break;
			case FreeLookCameraOrbit.Far:
				orbitNb = 1;
			break;
		}

		StartCoroutine(ChangeOrbitNb(m_freeLookCam.m_Orbits, 0, true, m_freeLookCam.m_Orbits[0].m_Radius, m_orbits[orbitNb].m_topOrbit.m_Height));
		StartCoroutine(ChangeOrbitNb(m_freeLookCam.m_Orbits, 0, false, m_freeLookCam.m_Orbits[0].m_Radius, m_orbits[orbitNb].m_topOrbit.m_Radius));

		StartCoroutine(ChangeOrbitNb(m_freeLookCam.m_Orbits, 1, true, m_freeLookCam.m_Orbits[1].m_Radius, m_orbits[orbitNb].m_middleOrbit.m_Height));
		StartCoroutine(ChangeOrbitNb(m_freeLookCam.m_Orbits, 1, false, m_freeLookCam.m_Orbits[1].m_Radius, m_orbits[orbitNb].m_middleOrbit.m_Radius));

		StartCoroutine(ChangeOrbitNb(m_freeLookCam.m_Orbits, 2, true, m_freeLookCam.m_Orbits[2].m_Radius, m_orbits[orbitNb].m_botOrbit.m_Height));
		StartCoroutine(ChangeOrbitNb(m_freeLookCam.m_Orbits, 2, false, m_freeLookCam.m_Orbits[2].m_Radius, m_orbits[orbitNb].m_botOrbit.m_Radius));
	}
	IEnumerator ChangeOrbitNb(CinemachineFreeLook.Orbit[] orbit, int intOfOrbit, bool isHeight, float from, float to){
		
		float moveJourneyLength;
		float moveFracJourney = new float();

		if(isHeight){
			while(orbit[intOfOrbit].m_Height != to){

				moveJourneyLength = from > to ? from - to : to - from;
				moveFracJourney += (Time.deltaTime) * m_switchPreferences.m_speed / moveJourneyLength;
				from = Mathf.Lerp(from, to, m_switchPreferences.m_curve.Evaluate(moveFracJourney));
				
				orbit[intOfOrbit].m_Height = from;
				yield return null;
			}
		}else{
			while(orbit[intOfOrbit].m_Radius != to){

				moveJourneyLength = from > to ? from - to : to - from;
				moveFracJourney += (Time.deltaTime) * m_switchPreferences.m_speed / moveJourneyLength;
				from = Mathf.Lerp(from, to, m_switchPreferences.m_curve.Evaluate(moveFracJourney));
				
				orbit[intOfOrbit].m_Radius = from;
				yield return null;
			}
		}
	}

	public void ResetXInput(bool b){
		if(b){
			m_freeLookCam.m_XAxis = m_saveXAxis;
		}else{
			m_saveXAxis = m_freeLookCam.m_XAxis;
			m_dontMoveXAxis.Value = m_saveXAxis.Value;
			m_freeLookCam.m_XAxis = m_dontMoveXAxis;
		}
	}
	public void ResetYInput(bool b){
		if(b){
			m_freeLookCam.m_YAxis = m_saveYAxis;
		}else{
			m_saveYAxis = m_freeLookCam.m_YAxis;
			m_dontMoveYAxis.Value = m_saveYAxis.Value;
			m_freeLookCam.m_YAxis = m_dontMoveYAxis;
		}
	}

}
