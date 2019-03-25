using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI() {
		FieldOfView fow = (FieldOfView)target;
		Handles.color = Color.white;
		Handles.DrawWireArc (fow.transform.position, Vector3.up, Vector3.forward, 360, fow.m_viewRadius);
		Vector3 viewAngleA = fow.DirFromAngle (-fow.m_viewAngle / 2, false);
		Vector3 viewAngleB = fow.DirFromAngle (fow.m_viewAngle / 2, false);

		Handles.DrawLine (fow.transform.position, fow.transform.position + viewAngleA * fow.m_viewRadius);
		Handles.DrawLine (fow.transform.position, fow.transform.position + viewAngleB * fow.m_viewRadius);

		Handles.color = Color.red;
		if(fow.m_playerTarget != null){
			Handles.DrawLine (fow.transform.position, fow.m_playerTarget.transform.position);
		}
	}

}