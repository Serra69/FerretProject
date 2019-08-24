using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FollowPathCreator))]
public class FollowPathEditor : Editor {

	[SerializeField] public Color[] m_colors = new Color[5];

	FollowPathCreator m_followPathCreator;

	void OnEnable(){
		if(m_followPathCreator == null){
			m_followPathCreator = (FollowPathCreator)target;
		}
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		// for (int i = 0, l = m_colors.Length; i < l; ++i){
		// 	m_colors[i] = EditorGUILayout.ColorField(m_colors[i]);
		// }
	}

}
